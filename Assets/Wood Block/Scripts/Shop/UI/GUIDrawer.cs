using Kimicu.YandexGames;
using System.Collections.Generic;
using UnityEngine;

public class GUIDrawer : MonoBehaviour
{
    [Space]
    [Header("Links")]
    [SerializeField] private ShopSystem _shopSystem;
    [SerializeField] private Transform _rootSpawnPurchases;
    [SerializeField] private GameObject _purchasePrefab;

    private List<GameObject> _products = new();



    private void OnEnable()
    {
        _shopSystem.OnShopUpdated += UpdatePurchasesList;
        _shopSystem.OnInitialized += InitializePurchasesList;
    }
    private void OnDisable()
    {
        _shopSystem.OnShopUpdated -= UpdatePurchasesList;
        _shopSystem.OnInitialized -= InitializePurchasesList;
    }

    public void UpdatePurchasesList(List<ShopItem> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            _products[i].GetComponent<Purchase>().UpdatePurchase();
        }
    }
    public void InitializePurchasesList(List<ShopItem> list)
    {
        // Spawn catalog
        for (int i = 0; i < list.Count; i++)
        {
            GameObject purchaseObj = Instantiate(_purchasePrefab, _rootSpawnPurchases);
            purchaseObj.GetComponent<Purchase>().Initialize(list[i].CatalogProduct.id,_shopSystem);
            _products.Add(purchaseObj);
        }
    }
}
