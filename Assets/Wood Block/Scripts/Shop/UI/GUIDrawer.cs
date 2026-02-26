using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUIDrawer : MonoBehaviour
{
    [SerializeField] private ProductType _typeOfProducts;
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
        var sortedList = list.Where(x => x.ProductType == _typeOfProducts).ToList();

        for (int i = 0; i < sortedList.Count; i++)
        {
            _products[i].GetComponent<Purchase>().UpdatePurchase();
        }
    }
    public void InitializePurchasesList(List<ShopItem> list)
    {
        var sortedList = list.Where(x => x.ProductType == _typeOfProducts).ToList();

        for (int i = 0; i < sortedList.Count; i++)
        {
            GameObject purchaseObj = Instantiate(_purchasePrefab, _rootSpawnPurchases);
            purchaseObj.GetComponent<Purchase>().Initialize(sortedList[i].CatalogProduct.id,_shopSystem);
            _products.Add(purchaseObj);
        }
    }
}
