using Agava.YandexGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Billing = Kimicu.YandexGames.Billing;

[RequireComponent(typeof(GUIDrawer))]
public class ShopSystem : MonoBehaviour
{

    private List<ShopItem> _items = new();
    private string _currentSelectedSkin = "";
    private string _currentSelectedBackground = "";

    public Action<List<ShopItem>> OnShopUpdated;
    public Action<List<ShopItem>> OnInitialized;

    public static Action OnBackgroundSkinChanged;

    private void Start() => Initialize();

    public void BuyItem(string itemID)
    {
        if (CheckIsItemBought(itemID))
            SelectSkin(itemID);
        else
            Billing.PurchaseProduct(itemID, ConsumePayment, Debug.LogError);
    }
    public ShopItem GetShopItemByID(string ItemID) => _items.Find(x => x.CatalogProduct.id == ItemID);

    private void SelectSkin(string ItemID)
    {
        if (GetShopItemByID(ItemID).ProductType == ProductType.Skin)
        {
            _currentSelectedSkin = ItemID;
            DataSaver.Save(SaveKeys.SelectedSkinId, _currentSelectedSkin);
            OnShopUpdated?.Invoke(_items);
        }
        else
        {
            _currentSelectedBackground = ItemID;
            DataSaver.Save(SaveKeys.SelectedBackgroundId, _currentSelectedBackground);
            OnBackgroundSkinChanged?.Invoke();
            OnShopUpdated?.Invoke(_items);
        }
    }
    private void ConsumePayment(PurchaseProductResponse response)
    {
        var itemID = _items.FindIndex(x => x.CatalogProduct.id == response.purchaseData.productID);
        Billing.ConsumeProduct(response.purchaseData.purchaseToken, () => _items[itemID].IsBought = true);

        DataSaver.Save(SaveKeys.Products,_items);
            
        OnShopUpdated?.Invoke(_items);
    }
    private bool CheckIsItemBought(string id) => _items.First(x => x.CatalogProduct.id == id).IsBought;
    private void Initialize()
    {
        if (Billing.Initialized)
        {
            if (DataSaver.HasSaves(SaveKeys.Products))
                _items = DataSaver.Load<List<ShopItem>>(SaveKeys.Products);
            else
                SortProductsCatalog();


            if (DataSaver.HasSaves(SaveKeys.SelectedSkinId))
                _currentSelectedSkin = DataSaver.Load<string>(SaveKeys.SelectedSkinId);

            OnInitialized?.Invoke(_items);
        }
        else
            Debug.LogError("Billing not initialized");
    }
    private void SortProductsCatalog()
    {
        var justSkins = Billing.CatalogProducts.Where(x => x.id.Contains("skin")).ToList();
        var justBackgrounds = Billing.CatalogProducts.Where(x => x.id.Contains("bg")).ToList();

        foreach (var item in justSkins)
            _items.Add(new ShopItem(false, item,ProductType.Skin));
        foreach (var item in justBackgrounds)
            _items.Add(new ShopItem(false, item,ProductType.Background));
    }
}

public class ShopItem
{
    private bool _isBought;
    private CatalogProduct _catalogProduct;
    private ProductType _productType;

    public bool IsBought { get => _isBought; set => _isBought = value; }
    public CatalogProduct CatalogProduct { get => _catalogProduct; }
    public ProductType ProductType { get => _productType; }
 

    public ShopItem(bool isBought, CatalogProduct catalogProduct,ProductType productType)
    {
        _isBought = isBought;
        _catalogProduct = catalogProduct;
        _productType = productType;
    }

}
