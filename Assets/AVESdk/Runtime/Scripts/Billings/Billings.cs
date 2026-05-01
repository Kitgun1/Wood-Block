using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Playgama;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            _settings = await Addressables.LoadAssetAsync<BillingsSettings>("BillingsSettings");
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }

        _settings.GetAllCatalog((dictionary) => { InitializeCatlogProducts(dictionary, Debug.LogError);}, Debug.LogError);

        IsInitialized = true;
    }
    public static void PurchaseProduct(string id,Action<string> onSeccessCallback,Action<string> onErrorCallback = null)
    {
        if (IsInitialized)
        {
            bool success = false;
            Bridge.payments.Purchase(id,(isSuccess, result) => { success = isSuccess; });

            if (success)
                onSeccessCallback?.Invoke(id);
            else
                onErrorCallback?.Invoke("The purchase was unsuccessful");
        }
        else
        {
            onErrorCallback?.Invoke("Billings wasnt initialized!");
        }
    }
    public static void ConsumeProduct(string id,Action onSeccessCallback,Action<string> onErrorCallback = null)
    {
        if (IsInitialized)
        {
            bool success = false;
            Bridge.payments.ConsumePurchase(id, (isSuccess, result) => { success = isSuccess; });

            if (success)
                onSeccessCallback?.Invoke();
            else
                onErrorCallback?.Invoke("The purchase confirmation was unsuccessful");
        }
        else
        {
            onErrorCallback?.Invoke("Billings wasnt initialized!");
        }
    }

    private static void InitializeCatlogProducts(SerializedDictionary<string,ProductSetting> products,Action<string> onErrorCallback)
    {
        List<Dictionary<string, string>> catalog = null;
        bool success = false;
        int successfullyAddedItemsCount = 0;

        if (Bridge.payments.isSupported)
        {
            Bridge.payments.GetCatalog((isSuccess, loadedCatalog) => { success = isSuccess; catalog = loadedCatalog; });

            if (success)
            {
                foreach (var item in catalog)
            {
                if (products.TryGetValue(item["id"], out var product))
                {
                    if (Bridge.platform.language == "ru")
                    {
                        _products.Add(new CatalogProduct(item["id"], item["price"], product.RuTitle, product.Image));
                    }
                    else
                    {
                        _products.Add(new CatalogProduct(item["id"], item["price"], product.EnTitle, product.Image));
                    }
                    successfullyAddedItemsCount++;
                }
            }

            if (_debugMode)
                Debug.Log($"Products catalog was initialized! Successfully was added {successfullyAddedItemsCount} items!");
        }
        else
            onErrorCallback?.Invoke("Cant to load catalog from sdk");
    }
        else
            onErrorCallback?.Invoke("Payments doesnt supported");
    }
}
