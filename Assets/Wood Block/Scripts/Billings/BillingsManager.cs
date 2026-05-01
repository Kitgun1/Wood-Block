using Playgama;
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
        if (Bridge.payments.isSupported)
        {
            //Bridge.payments.GetPurchases(OnPurchasesGot);
            //{
            //    Debug.Log("Получили купленные товары, за которые мы не выдали награду:");
            //    //var products = response.purchasedProducts.ToArray();

            ////    foreach (var product in products)
            ////    {
            ////        //Billing.ConsumeProduct(product.purchaseToken, onSuccessCallback: () => _shopSystem.ConsumeProduct(product.productID), onErrorCallback: (string error) =>
            ////        //{
            ////        //    if (error.Contains("User canceled"))
            ////        //        Debug.Log("User canceled purchase");
            ////        //    else
            ////        //        Debug.LogError(error);
            ////        //}
            ////        //);
            ////    }
            ////});
        }
        else
            Debug.LogError("Billing isnot initialized");
    }

    private void OnPurchasesGot(bool success, List<Dictionary<string, string>> purchases)
    {
        if (success)
        {
            foreach (var purchase in purchases)
            {

            }
        }
    }
}
