using KimicuUtility;
using Lean.Localization;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Purchase : MonoBehaviour
{
    [SerializeField] private ShopProduct _product;

    private string _id;
    private ShopSystem _shopSystem;

    private void Start() => UpdatePurchase();

    public void UpdatePurchase()
    {
        if (gameObject.activeInHierarchy)
        {
            var item = _shopSystem.GetShopItemByID(_id);

            _product.TitleText.text = item.CatalogProduct.Name;
            _product.PriceText.text = item.CatalogProduct.Price;

            if (item.IsBought)
            {
                if (DataSaver.Load<string>(SaveKeys.SelectedSkinId) == _id || DataSaver.Load<string>(SaveKeys.SelectedBackgroundId) == _id)
                {
                    if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
                        _product.ButtonText.text = "Выбран";
                    else
                        _product.ButtonText.text = "Selected";
                }
                else
                {
                    if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
                        _product.ButtonText.text = "Выбрать";
                    else
                        _product.ButtonText.text = "Select";
                }
            }
            else
            {
                switch (item.WaysToUnlockSkin)
                {
                    case WaysToUnlockSkin.BuyForLevels:
                        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
                            _product.ButtonText.text = $"За левел {item.LevelToGetSkin}";
                        else
                            _product.ButtonText.text = $"For level {item.LevelToGetSkin}";
                        break;
                    case WaysToUnlockSkin.BuyForCurrencie:
                        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
                            _product.ButtonText.text = "Купить";
                        else
                            _product.ButtonText.text = "Buy";
                        break;
                    case WaysToUnlockSkin.BuyForAdd:
                        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
                            _product.ButtonText.text = "Получить за рекламу";
                        else
                            _product.ButtonText.text = "Get For Add";
                        break;
                }
            }

            // No need currency sprite for default items or buy for add/levels
            _product.CurrencySprite.enabled = false;

            if (item.WaysToUnlockSkin == WaysToUnlockSkin.BuyForCurrencie && 
            _shopSystem.IsDefaultItem(item.CatalogProduct.ID) != true)
            {
                _product.PriceText.text = item.CatalogProduct.Price;
                _product.CurrencySprite.color = Color.white;
            }
            else
            {  
                _product.PriceText.text = "";
            }

            _product.Image.sprite = item.CatalogProduct.Image;
        }
    }
    public void Initialize(string id, ShopSystem system)
    {
        _id = id;
        _shopSystem = system;

        var item = _shopSystem.GetShopItemByID(_id);
        _product.Button.AddListener(() => _shopSystem.BuyItem(item.CatalogProduct.ID));
    }

}

[Serializable]
public struct ShopProduct
{
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Button _buttons;
    [SerializeField] private Image _image;
    [SerializeField] private Image _currencySprite;

    public TMP_Text PriceText { get => _priceText; }
    public TMP_Text ButtonText { get => _buttonText; }
    public TMP_Text TitleText { get => _titleText; }
    public Image Image { get => _image; }
    public Image CurrencySprite { get => _currencySprite; }
    public Button Button { get => _buttons; }
}
