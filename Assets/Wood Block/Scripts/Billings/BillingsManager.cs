using Kimicu.YandexGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BillingsManager : MonoBehaviour
{
    [SerializeField] private ShopSystem _shopSystem;

    private void Start()
    {
        if (Billing.Initialized)
        {
            Billing.GetPurchasedProducts(response =>
            {
                Debug.Log("Получили купленные товары, за которые мы не выдали награду:");
                var products = response.purchasedProducts.ToArray();

                foreach (var product in products)
                {
                    Billing.ConsumeProduct(product.purchaseToken, onSuccessCallback: () => _shopSystem.ConsumeProduct(product.productID), onErrorCallback: (string error) =>
                    {
                        if (error.Contains("User canceled"))
                            Debug.Log("User canceled purchase");
                        else
                            Debug.LogError(error);
                    }
                    );
                }
            });
        }
        else
            Debug.LogError("Billing isnot initialized");
    }
}
