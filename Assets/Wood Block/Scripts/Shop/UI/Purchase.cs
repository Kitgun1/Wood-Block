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
                    _product.ButtonText.text = LeanLocalization.GetTranslationText("Selected", "Selected");
                }
                else
                {
                    _product.ButtonText.text = LeanLocalization.GetTranslationText("Select", "Select");
                }

 // No need currency sprite for default items or buy for add/levels
                _product.CurrencySprite.enabled = false;
                _product.PriceText.enabled = false;
                _product.PriceText.text = string.Empty;
            }
            else
            {
                    // No need currency sprite for default items or buy for add/levels
                    _product.CurrencySprite.enabled = false;

                switch (item.WaysToUnlockSkin)
                {
                    case WaysToUnlockSkin.BuyForLevels:
                    {
                        string localizedFormat = LeanLocalization.GetTranslationText("ForLevel", "For level {0}");
                        _product.ButtonText.text = string.Format(localizedFormat, item.LevelToGetSkin);
                        _product.PriceText.enabled = false;
                        _product.PriceText.text = string.Empty;
                        break;
                    }

                    case WaysToUnlockSkin.BuyForCurrencie:
                    {
                        _product.ButtonText.text = LeanLocalization.GetTranslationText("Buy", "Buy");

                        if (_shopSystem.IsDefaultItem(item.CatalogProduct.ID) != true)
                        {
                            _product.PriceText.enabled = true;
                            _product.PriceText.text = item.CatalogProduct.Price;
                            _product.CurrencySprite.color = Color.white;
                        }
                        else
                        {
                            _product.PriceText.enabled = false;
                            _product.PriceText.text = string.Empty;
                        }

                        break;
                    }
                    case WaysToUnlockSkin.BuyForAdd:
                    {
                        _product.PriceText.enabled = false;
                        _product.PriceText.text = string.Empty;
                        _product.ButtonText.text = LeanLocalization.GetTranslationText("GetForAd", "Get For Add");
                        break;
                    }
                }
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

    private void OnEnable()
    {
        LeanLocalization.OnLocalizationChanged += UpdatePurchase;
    }

    private void OnDisable()
    {
        LeanLocalization.OnLocalizationChanged -= UpdatePurchase;
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
