using Kimicu.YandexGames;
using Kimicu.YandexGames.Utils;
using Kimicu.YandexGames.Extension;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class YandexCurrencyService
{
    public static List<(string id, string code, int priceValue, Sprite sprite)> Currencies { get; private set; }

    public static IEnumerator BillingYandexCurrencySetup()
    {
        /* »нициализаци€ данных о yandex currency */
        Currencies = new List<(string id, string code, int priceValue, Sprite sprite)>();
        var currencyUrls = Billing.CatalogProducts
          .Select(p => (p.id, p.priceCurrencyCode, p.priceValue, p.priceCurrencyPicture))
          .Distinct()
          .ToArray();

        var totalCurrencyCount = currencyUrls.Length;
        var loadedCurrencyCount = 0;

        foreach (var product in currencyUrls)
        {
            yield return SvgLoader.GetSvgSprite(product.priceCurrencyPicture, Vector2Int.one * 512, sprite =>
            {
                Currencies.Add((product.id, product.priceCurrencyCode, int.Parse(product.priceValue), sprite));
                
                loadedCurrencyCount++;
                if (loadedCurrencyCount == totalCurrencyCount)
                {
                    Debug.Log("All currencies loaded!"); // ќжидаем этого момента и можем запускать игру
                }
            },Debug.Log);
        }
    }
}
