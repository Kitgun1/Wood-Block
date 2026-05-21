using Kimicu.YandexGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyText : MonoBehaviour
{
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Image _currencyImage;

    private void Start()
    {
        var product = Billing.CatalogProducts.FirstOrDefault(x => x.id == "mapBlowUp");

        _priceText.text = product.price;
        _currencyImage.sprite = YandexCurrencyService.Currencies[0].sprite;
    }

}
