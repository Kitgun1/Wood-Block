using Agava.YandexGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Billing = Kimicu.YandexGames.Billing;

[RequireComponent(typeof(GUIDrawer))]
public class ShopSystem : MonoBehaviour
{

    private List<ShopItem> _skins = new();
    private string _currentSelectedSkin = "";

    public Action<List<ShopItem>> OnShopUpdated;
    public Action<List<ShopItem>> OnInitialized;

    private void Start()
    {
        Initialize();
    }

    public void BuyItem(string itemID)
    {
        if (CheckIsItemBought(itemID))
            SelectSkin(itemID);
        else
            Billing.PurchaseProduct(itemID, ConsumePayment, Debug.LogError);
    }
    public ShopItem GetShopItemByID(string ItemID) => _skins.Find(x => x.CatalogProduct.id == ItemID);

    private void SelectSkin(string ItemID)
    {
        _currentSelectedSkin = ItemID;
        ShopSaver.Save(_currentSelectedSkin);
    }
    private void ConsumePayment(PurchaseProductResponse response)
    {
        var itemID = _skins.FindIndex(x => x.CatalogProduct.id == response.purchaseData.productID);
        Billing.ConsumeProduct(response.purchaseData.purchaseToken, () => _skins[itemID].IsBought = true);

        ShopSaver.Save(_skins);

        OnShopUpdated?.Invoke(_skins);
    }
    private bool CheckIsItemBought(string id) => _skins.First(x => x.CatalogProduct.id == id).IsBought;
    private void Initialize()
    {
        if (Billing.Initialized)
        {
            if (ShopSaver.HasSkinsSaves())
                _skins = ShopSaver.LoadData().Item1;
            else
            {
                //фильтрация продуктов, чтобы в гоп не попадало ничего кроме скинов
                var justSkins = Billing.CatalogProducts.Where(x => x.id.Contains("skin")).ToList();

                foreach (var item in justSkins)
                    _skins.Add(new ShopItem(false, item));
            }

            if (ShopSaver.HasSelectedSkinsSaves())
                _currentSelectedSkin = ShopSaver.LoadData().Item2;


            OnInitialized?.Invoke(_skins);
        }
        else
            Debug.LogError("Billing not initialized");
    }
}

public class ShopItem
{
    private bool _isBought;
    private CatalogProduct _catalogProduct;

    public bool IsBought { get => _isBought; set => _isBought = value; }
    public CatalogProduct CatalogProduct { get => _catalogProduct; }

    public ShopItem(bool isBought, CatalogProduct catalogProduct)
    {
        _isBought = isBought;
        _catalogProduct = catalogProduct;
    }

}
