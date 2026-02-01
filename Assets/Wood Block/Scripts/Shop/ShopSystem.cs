using Agava.YandexGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Billing = Kimicu.YandexGames.Billing;

[RequireComponent(typeof(GUIDrawer))]
public class ShopSystem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private List<ShopItem> _skins = new();
    private string _currentSelectedSkin = "";

    public Action<List<ShopItem>> OnShopUpdated;
    public Action<List<ShopItem>> OnInitialized;

    private void Start()
    {
        Initialize();
    }

    public void BuyItem(string itemID, Sprite skinSprite)
    {
        if (CheckIsItemBought(itemID))
            SelectSkin(itemID, skinSprite);
        else
            Billing.PurchaseProduct(itemID, ConsumePayment, Debug.LogError);
    }
    public void SelectSkin(string ItemID, Sprite skinSprite)
    {
        if (_currentSelectedSkin != "")
        {
            _currentSelectedSkin = ItemID;
            _spriteRenderer.sprite = skinSprite;
            ShopSaver.Save(_skins, _currentSelectedSkin);
        }
    }

    private void ConsumePayment(PurchaseProductResponse response)
    {
        var itemID = _skins.FindIndex(x => x.CatalogProduct.id == response.purchaseData.productID);
        Billing.ConsumeProduct(response.purchaseData.purchaseToken, () => _skins[itemID].IsBought = true);

        ShopSaver.Save(_skins, _currentSelectedSkin);

        OnShopUpdated?.Invoke(_skins);
    }
    private bool CheckIsItemBought(string id) => _skins.First(x => x.CatalogProduct.id == id).IsBought;
    private void Initialize()
    {
        if (Billing.Initialized)
        {
            var result = ShopSaver.HasSaves();

            if (result.skinSaves)
                _skins = ShopSaver.LoadData().Item1;
            else
                foreach (var item in Billing.CatalogProducts)
                    _skins.Add(new ShopItem(false, item));

            if (result.selectedItemSaves)
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
