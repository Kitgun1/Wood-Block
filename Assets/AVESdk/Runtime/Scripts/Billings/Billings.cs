using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Billings
{
    private static BillingsSettings _settings;
    private static List<CatalogProduct> _products;
    private static bool _debugMode;

    public static bool IsInitialized { get; private set; } = false;
    public static IEnumerable<CatalogProduct> CatalogProducts { get; private set; }

    public static async UniTask Initialize(bool debugMode = false)
    {
        _debugMode = debugMode;

        try
        {
            _settings = Resources.Load<BillingsSettings>("BillingSettings");
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }

        if (_settings == null)
        {
            Debug.LogError("[Billings] BillingsSettings failed to load! Skipping catalog initialization to prevent hang.");
            _products = new List<CatalogProduct>();
            CatalogProducts = _products;
            IsInitialized = true;
            return;
        }

        var utcs = new UniTaskCompletionSource();

        _settings.GetAllCatalog(
            (dictionary) => 
            { 
                InitializeCatalogProducts(dictionary, 
                    () => 
                    {
                        IsInitialized = true;
                        utcs.TrySetResult();
                    }, 
                    (error) => 
                    {
                        Debug.LogError(error);
                        utcs.TrySetResult();
                    });
            }, 
            (error) => 
            {
                Debug.LogError(error);
                utcs.TrySetResult();
            }
        );

        // Enforce a timeout of 3000ms to prevent game boot hanging if the platform SDK fails to respond
        var timeoutTask = UniTask.Delay(3000);
        var completedTaskIndex = await UniTask.WhenAny(utcs.Task, timeoutTask);

        if (completedTaskIndex == 1)
        {
            Debug.LogWarning("[Billings] Initialization timed out! Falling back to local settings.");
            _settings.GetAllCatalog(
                (dictionary) => InitializeFromLocalSettings(dictionary),
                (error) => { _products = new List<CatalogProduct>(); CatalogProducts = _products; }
            );
            IsInitialized = true;
        }
    }
    public static void PurchaseProduct(string id,Action<string> onSuccessCallback,Action<string> onErrorCallback = null)
    {
        if (IsInitialized)
        {
            if (PlatformSDK.Payments != null && PlatformSDK.Payments.IsSupported)
            {
                PlatformSDK.Payments.Purchase(id, isSuccess => 
                {
                    if (isSuccess)
                        onSuccessCallback?.Invoke(id);
                    else
                        onErrorCallback?.Invoke("The purchase was unsuccessful");
                });
            }
            else
            {
                Debug.Log($"[Mock/Editor] Purchase product succeeded for ID: {id}");
                onSuccessCallback?.Invoke(id);
            }
        }
        else
        {
            onErrorCallback?.Invoke("Billings wasn't initialized!");
        }
    }
    public static void ConsumeProduct(string id,Action onSuccessCallback,Action<string> onErrorCallback = null)
    {
        if (IsInitialized)
        {
            if (PlatformSDK.Payments != null && PlatformSDK.Payments.IsSupported)
            {
                PlatformSDK.Payments.Consume(id, isSuccess => 
                {
                    if (isSuccess)
                        onSuccessCallback?.Invoke();
                    else
                        onErrorCallback?.Invoke("The purchase confirmation was unsuccessful");
                });
            }
            else
            {
                Debug.Log($"[Mock/Editor] Consume product succeeded for ID: {id}");
                onSuccessCallback?.Invoke();
            }
        }
        else
        {
            onErrorCallback?.Invoke("Billings wasn't initialized!");
        }
    }

    private static void InitializeCatalogProducts(
        SerializedDictionary<string, ProductSetting> products,
        Action onSuccessCallback,
        Action<string> onErrorCallback)
    {
        // 1. Always load all products from local settings as the baseline
        InitializeFromLocalSettings(products);

        if (PlatformSDK.Payments != null && PlatformSDK.Payments.IsSupported)
        {
            PlatformSDK.Payments.GetCatalog((isSuccess, loadedCatalog) =>
            {
                if (isSuccess && loadedCatalog != null)
                {
                    int updatedItemsCount = 0;

                    foreach (var item in loadedCatalog)
                    {
                        if (item.TryGetValue("id", out string id))
                        {
                            string price = "";
                            if (item.TryGetValue("price", out string pVal) && !string.IsNullOrEmpty(pVal))
                            {
                                price = pVal;
                            }
                            else if (item.TryGetValue("priceValue", out string pvVal) && !string.IsNullOrEmpty(pvVal))
                            {
                                price = pvVal;
                            }
                            else if (item.TryGetValue("price_value", out string pv_Val) && !string.IsNullOrEmpty(pv_Val))
                            {
                                price = pv_Val;
                            }
                            else if (item.TryGetValue("amount", out string aVal) && !string.IsNullOrEmpty(aVal))
                            {
                                price = aVal;
                            }

                            // Update price for the existing catalog product if it exists
                            var existingProduct = _products.Find(x => x.ID == id);
                            if (existingProduct != null)
                            {
                                existingProduct.Price = price;
                                updatedItemsCount++;
                            }
                        }
                    }

                    if (_debugMode)
                        Debug.Log($"Products catalog prices updated from platform! Updated {updatedItemsCount} items!");
                }

                onSuccessCallback?.Invoke();
            });
        }
        else
        {
            onSuccessCallback?.Invoke();
        }
    }

    private static void InitializeFromLocalSettings(SerializedDictionary<string, ProductSetting> products)
    {
        _products = new List<CatalogProduct>();
        int successfullyAddedItemsCount = 0;

        foreach (var pair in products)
        {
            string id = pair.Key;
            var product = pair.Value;
            if (product != null)
            {
                _products.Add(new CatalogProduct(id, "100", product.RuTitle, product.EnTitle, product.Image, product.TitleLocalizationKey));
                successfullyAddedItemsCount++;
            }
        }

        CatalogProducts = _products;

        if (_debugMode)
            Debug.Log($"[Mock/Editor] Products catalog was initialized from local settings! Successfully added {successfullyAddedItemsCount} items!");
    }
}
