using Cysharp.Threading.Tasks;
using Kimicu.YandexGames;
using Kimicu.YandexGames.Extension;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YandexCurrencyService
{
    public List<(string id, string code, Sprite sprite)> Currencies { get; private set; }

    public async void Initialize()
    {
        /* »нициализаци€ данных о yandex currency */
        Currencies = new List<(string id, string code, Sprite sprite)>();
        var currencyUrls = Billing.CatalogProducts
          .Select(p => (p.id, p.priceCurrencyCode, p.priceCurrencyPicture))
          .Distinct()
          .ToArray();

        var totalCurrencyCount = currencyUrls.Length;
        var loadedCurrencyCount = 0;

        foreach (var product in currencyUrls)
        {
            await PictureExtension.GetPicture(product.priceCurrencyPicture, texture =>
            {
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
                Currencies.Add((product.id, product.priceCurrencyCode, sprite));

                loadedCurrencyCount++;
                if (loadedCurrencyCount == totalCurrencyCount)
                {
                    Debug.Log("All currencies loaded!"); // ќжидаем этого момента и можем запускать игру
                }
            });
        }
    }
}
