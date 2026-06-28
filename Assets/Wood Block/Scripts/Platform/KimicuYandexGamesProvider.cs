using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Kimicu.YandexGames;
using Kimicu.YandexGames.Utils;

public class KimicuYandexGamesProvider : IPlatformProvider, IAdsProvider, IStorageProvider, IPaymentsProvider
{
    public IAdsProvider Ads => this;
    public IStorageProvider Storage => this;
    public IPaymentsProvider Payments => this;

    public string Language => Kimicu.YandexGames.YandexGamesSdk.Language;

    public bool IsSupported => Kimicu.YandexGames.Billing.Initialized;

    public event Action<bool> OnPauseStateChanged;
    public event Action<bool> OnAudioStateChanged;
    public event Action OnAdStarted;
    public event Action OnAdCompleted;

    private static readonly string[] SaveKeysList = new string[]
    {
        "items",
        "selectedBGId",
        "selectedSkinId",
        "musicVolume",
        "soundsVolume",
        "levels",
        "levelQuests",
        "bestScore"
    };

    public async UniTask Initialize()
    {
        Debug.Log("[KimicuYandexGamesProvider] Starting initialization...");

        // 1. Initialize YandexGamesSdk
        var sdkTcs = new UniTaskCompletionSource();
        Coroutines.StartRoutine(Kimicu.YandexGames.YandexGamesSdk.Initialize(() => {
            Debug.Log("[KimicuYandexGamesProvider] YandexGamesSdk initialized!");
            sdkTcs.TrySetResult();
        }));
        await sdkTcs.Task;

        // 2. Initialize WebApplication focus/pause listener
        Kimicu.YandexGames.WebApplication.Initialize(isPaused => {
            DeferFocusPauseChange(isPaused).Forget();
        });

        // 3. Initialize Cloud Save
        // NOTE: Kimicu's OnGetCloudErrorCallback does NOT set Cloud.Initialized = true,
        // so if cloud data is corrupted/missing the coroutine hangs forever.
        // We guard with a timeout so the game can always continue with local PlayerPrefs.
        var cloudTcs = new UniTaskCompletionSource();
        Coroutines.StartRoutine(Kimicu.YandexGames.Cloud.Initialize(() => {
            Debug.Log("[KimicuYandexGamesProvider] Cloud Save initialized!");
            SyncCloudToPlayerPrefs();
            cloudTcs.TrySetResult();
        }));

        var cloudResult = await UniTask.WhenAny(
            cloudTcs.Task,
            UniTask.Delay(System.TimeSpan.FromSeconds(5))
        );

        if (cloudResult != 0)
        {
            Debug.LogWarning("[KimicuYandexGamesProvider] Cloud Save timed out or failed — continuing with local PlayerPrefs.");
        }

        // 4. Initialize Billing
        var billingTcs = new UniTaskCompletionSource();
        IEnumerator InitializeBilling()
        {
            yield return Kimicu.YandexGames.Billing.Initialize();
            Debug.Log("[KimicuYandexGamesProvider] Billing initialized!");
            billingTcs.TrySetResult();
        }
        Coroutines.StartRoutine(InitializeBilling());
        await billingTcs.Task;

        // 5. Initialize Advertisement
        Kimicu.YandexGames.Advertisement.Initialize();
        Debug.Log("[KimicuYandexGamesProvider] Advertisement initialized!");

        Debug.Log("[KimicuYandexGamesProvider] Initialization complete!");
    }

    private void SyncCloudToPlayerPrefs()
    {
        if (!Kimicu.YandexGames.Cloud.Initialized) return;
        foreach (var key in SaveKeysList)
        {
            if (Kimicu.YandexGames.Cloud.HasKey(key))
            {
                object rawVal = Kimicu.YandexGames.Cloud.GetValue<object>(key);
                if (rawVal != null)
                {
                    string jsonString;
                    if (rawVal is string strVal)
                    {
                        if (string.IsNullOrEmpty(strVal))
                        {
                            continue;
                        }
                        if (IsValidJson(strVal))
                        {
                            jsonString = strVal;
                        }
                        else
                        {
                            jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(strVal);
                        }
                    }
                    else
                    {
                        jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(rawVal);
                    }
                    if (jsonString != "null")
                    {
                        PlayerPrefs.SetString(key, jsonString);
                    }
                }
            }
        }
        PlayerPrefs.Save();
    }

    private bool IsValidJson(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return false;
        str = str.Trim();
        if ((str.StartsWith("{") && str.EndsWith("}")) ||
            (str.StartsWith("[") && str.EndsWith("]")) ||
            (str.StartsWith("\"") && str.EndsWith("\"")))
        {
            try
            {
                Newtonsoft.Json.Linq.JToken.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    public void SendGameReady()
    {
        Kimicu.YandexGames.YandexGamesSdk.GameReady();
    }

    // Ads
    public void ShowInterstitial()
    {
        OnAdStarted?.Invoke();
        Kimicu.YandexGames.Advertisement.ShowInterstitialAd(
            onOpenCallback: null,
            onCloseCallback: () => OnAdCompleted?.Invoke(),
            onErrorCallback: (err) => OnAdCompleted?.Invoke(),
            onOfflineCallback: () => OnAdCompleted?.Invoke()
        );
    }

    public void ShowRewarded(Action onRewarded, Action onClosedOrFailed)
    {
        OnAdStarted?.Invoke();
        bool rewarded = false;

        Kimicu.YandexGames.Advertisement.ShowVideoAd(
            onOpenCallback: null,
            onRewardedCallback: () => {
                rewarded = true;
                onRewarded?.Invoke();
            },
            onCloseCallback: () => {
                if (!rewarded)
                {
                    onClosedOrFailed?.Invoke();
                }
                OnAdCompleted?.Invoke();
            },
            onErrorCallback: (err) => {
                if (!rewarded)
                {
                    onClosedOrFailed?.Invoke();
                }
                OnAdCompleted?.Invoke();
            }
        );
    }

    // Storage
    public void Set(string key, string value)
    {
        Kimicu.YandexGames.Cloud.SetValue(key, value, true);
    }

    public void Delete(string key)
    {
        Kimicu.YandexGames.Cloud.SetValue(key, "", true);
    }

    // Payments
    public void GetCatalog(Action<bool, List<Dictionary<string, string>>> callback)
    {
        if (Kimicu.YandexGames.Billing.CatalogProducts != null)
        {
            var catalogList = new List<Dictionary<string, string>>();
            foreach (var product in Kimicu.YandexGames.Billing.CatalogProducts)
            {
                var dict = new Dictionary<string, string>();
                dict["id"] = product.id;
                dict["price"] = product.price;
                dict["priceValue"] = product.priceValue;
                dict["priceCurrencyCode"] = product.priceCurrencyCode;
                dict["title"] = product.title;
                dict["description"] = product.description;
                dict["imageURI"] = product.imageURI;
                catalogList.Add(dict);
            }
            callback?.Invoke(true, catalogList);
        }
        else
        {
            callback?.Invoke(false, null);
        }
    }

    public void Purchase(string productId, Action<bool> callback)
    {
        Kimicu.YandexGames.Billing.PurchaseProduct(productId,
            success => callback?.Invoke(true),
            error => callback?.Invoke(false)
        );
    }

    public void Consume(string productId, Action<bool> callback)
    {
        Kimicu.YandexGames.Billing.GetPurchasedProducts(response => {
            var purchase = System.Array.Find(response.purchasedProducts, p => p.productID == productId);
            if (purchase != null)
            {
                Kimicu.YandexGames.Billing.ConsumeProduct(purchase.purchaseToken,
                    () => callback?.Invoke(true),
                    error => callback?.Invoke(false)
                );
            }
            else
            {
                callback?.Invoke(false);
            }
        }, error => callback?.Invoke(false));
    }

    private async UniTaskVoid DeferFocusPauseChange(bool isPaused)
    {
        await UniTask.Yield();
        Debug.Log($"[KimicuYandexGamesProvider] Focus/Pause state changed: {isPaused}");
        OnPauseStateChanged?.Invoke(isPaused);
        OnAudioStateChanged?.Invoke(!isPaused);
    }
}
