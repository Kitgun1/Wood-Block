using Kimicu.YandexGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BillingsManager : MonoBehaviour
{
    [SerializeField] private UnityEvent<string> _buyActions;

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
                    Billing.ConsumeProduct(product.purchaseToken,onSuccessCallback:() => _buyActions?.Invoke(product.productID), onErrorCallback: Debug.LogError);
                }
            });
        }
        else
            Debug.LogError("Billing isnot initialized");
    }
}
