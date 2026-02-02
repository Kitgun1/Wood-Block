using Kimicu.YandexGames.Extension;
using KimicuUtility;
using NaughtyAttributes.Test;
using System;
using System.Collections;
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
        var item = _shopSystem.GetShopItemByID(_id);

        _product.PriceText.text = item.CatalogProduct.priceValue;
        _product.TitleText.text = item.CatalogProduct.title;

        if (item.IsBought)
            _product.ButtonText.text = "Select";
        else
            _product.ButtonText.text = "Buy";

        StartCoroutine(DownloadImage(item.CatalogProduct.imageURI, _product.Image));
    }
    public void Initialize(string id,ShopSystem system)
    {
        _id = id;
        _shopSystem = system;

        var item = _shopSystem.GetShopItemByID(_id);
        _product.Button.AddListener(() => _shopSystem.BuyItem(item.CatalogProduct.id));
        _product.CurrencySprite.sprite = YandexCurrencyService.Currencies[0].sprite;
    }
    private IEnumerator DownloadImage(string url, Image targetImage)
    {
        yield return PictureExtension.GetPicture(url, texture =>
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            targetImage.sprite = sprite;
        });
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
