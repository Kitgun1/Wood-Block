using AYellowpaper.SerializedCollections;
using KimicuUtility;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BillingSettings",menuName = "Salat1k Extensions/Billings Settings", order =0)]
public class BillingsSettings : ScriptableObject
{
    [SerializeField] private SerializedDictionary<string, ProductSetting> _catalogProducts;

    public void GetAllCatalog(Action<SerializedDictionary<string, ProductSetting>> onSeccess,Action<string> onErrorCallback = null)
    {
        if (_catalogProducts.Keys.Count != 0)
            onSeccess?.Invoke(_catalogProducts);
        else
            onErrorCallback?.Invoke("Products catalog cant has zero keys");
    }
}
