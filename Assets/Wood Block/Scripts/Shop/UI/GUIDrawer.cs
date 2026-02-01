using Kimicu.YandexGames;
using Kimicu.YandexGames.Extension;
using KimicuUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GUIDrawer : MonoBehaviour
{
    [SerializeField] private ShopSystem _shopSystem;
    [Space]
    [Header("Links")]
    [SerializeField] private List<ShopProduct> _shopProducts;



    private void OnEnable()
    {
        _shopSystem.OnShopUpdated += UpdateGUI;
        _shopSystem.OnInitialized += InitilizeGUI;
    }
    private void OnDisable()
    {
        _shopSystem.OnShopUpdated -= UpdateGUI;
        _shopSystem.OnInitialized -= InitilizeGUI;
    }

    private void UpdateGUI(List<ShopItem> shopItems)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].IsBought)
                _shopProducts[i].ButtonText.text = "Select";
            else
                _shopProducts[i].ButtonText.text = "Buy";
        }
    }
    public void InitilizeGUI(List<ShopItem> shopItems)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            ShopItem currentItem = shopItems[i];
            ShopProduct currentProduct = _shopProducts[i];

            _shopProducts[i].PriceText.text = currentItem.CatalogProduct.priceValue;

            if (currentItem.IsBought)
                _shopProducts[i].ButtonText.text = "Select";
            else
                _shopProducts[i].ButtonText.text = "Buy";

            StartCoroutine(DownloadImage(currentItem.CatalogProduct.imageURI, _shopProducts[i].Image));
            currentProduct.Button.AddListener(() => _shopSystem.BuyItem(currentItem.CatalogProduct.id, currentProduct.Image.sprite));
        }
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
    [SerializeField] private Button _buttons;
    [SerializeField] private Image _image;

    public TMP_Text PriceText { get => _priceText; }
    public TMP_Text ButtonText { get => _buttonText; }
    public Image Image { get => _image; }
    public Button Button { get => _buttons; }
}
