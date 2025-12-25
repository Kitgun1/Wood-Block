using System;
using System.Collections.Generic;
using System.Linq;
using Agava.YandexGames;
using KimicuUtility;
using NaughtyAttributes;
using UnityEngine;

namespace WoodBlock
{
    public class ShopCategory : MonoBehaviour
    {
        [SerializeField] private CategoryProductType _categoryProductType;

        [SerializeField, EnableIf(nameof(_categoryProductType), CategoryProductType.Quantitative)]
        private DictionaryQuantitativeProductReward _quantitativeProduct = new();

        [SerializeField, EnableIf(nameof(_categoryProductType), CategoryProductType.Disposable)]
        private DictionaryDisposableProductReward _disposableProduct = new();

        private readonly Dictionary<string, Action<int>> _rewards = new();

        private void Awake()
        {
            _rewards.Add("clear_table_1", RewardClearTable);
            _rewards.Add("clear_table_3", RewardClearTable);
            _rewards.Add("clear_table_5", RewardClearTable);
            _rewards.Add("clear_table_9", RewardClearTable);

            _rewards.Add("return_move_1", RewardReturnMove);
            _rewards.Add("return_move_5", RewardReturnMove);
            _rewards.Add("return_move_10", RewardReturnMove);
            _rewards.Add("return_move_15", RewardReturnMove);

            _rewards.Add("coins_100", RewardCoins);
            _rewards.Add("coins_500", RewardCoins);
            _rewards.Add("coins_1000", RewardCoins);
            _rewards.Add("coins_2000", RewardCoins);
        }

        private void OnEnable()
        {
            switch (_categoryProductType)
            {
                case CategoryProductType.Quantitative:
                    foreach ((QuantitativeShopProduct product, int reward) in _quantitativeProduct)
                    {
                        CatalogProduct selected =
                            Kimicu.YandexGames.Billing.CatalogProducts?.FirstOrDefault(p => p.id == product.ProductId);
                        if (selected == null) continue;
                        print(System.Convert.ToInt32(selected.priceValue));
                        product.InitializeClick()
                            .SetPriceJan(System.Convert.ToInt32(selected.priceValue));
                        StartCoroutine(selected.imageURI.GetPicture(
                            texture2D => product.SetPicture(
                                Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                                    Vector2.one / 2))));

                        product.SetAmountReward(reward);
                        product.OnClickBuyJan.AddListener(QuantitativeProductPurchase);
                    }

                    break;
                case CategoryProductType.Disposable:
                    foreach ((DisposableShopProduct product, int reward) in _disposableProduct)
                    {
                        CatalogProduct selected =
                            Kimicu.YandexGames.Billing.CatalogProducts?.FirstOrDefault(p => p.id == product.ProductId);
                        if (selected == null) continue;

                        product.InitializeClick()
                            .SetPriceJan(int.Parse(selected.priceValue));

                        StartCoroutine(selected.imageURI.GetPicture(
                            texture2D => product.SetPicture(
                                Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                                    Vector2.one / 2))));

                        product.OnClickBuyJan.AddListener(QuantitativeProductPurchase);
                    }

                    break;
                case CategoryProductType.None:
                default:
                    break;
            }
        }

        private void QuantitativeProductPurchase(string productId, int amount)
        {
            print($"click: {productId}:{amount}");
            Kimicu.YandexGames.Billing.PurchaseProduct
            (
                productId, 
                onSuccessCallback: x =>
                {
                    _rewards[productId].Invoke(amount);
                    print($"{productId}:{amount}");

                    Billing.ConsumeProduct(x.purchaseData.purchaseToken, onErrorCallback: Debug.LogError);
                },
                onErrorCallback: Debug.LogWarning
            );
        }

        private static void RewardClearTable(int amount) => PlayerBag.ClearTableAmount += amount;
        private static void RewardReturnMove(int amount) => PlayerBag.CancelMoveAmount += amount;
        private static void RewardCoins(int amount) => PlayerBag.CoinAmount += amount;

        [Button]
        private void FillQuantitativeDictionary()
        {
            foreach (QuantitativeShopProduct child in gameObject.GetComponentsInChildren<QuantitativeShopProduct>())
            {
                _quantitativeProduct.TryAdd(child, 0);
            }
        }

        [Button]
        private void FillDisposableDictionary()
        {
            foreach (DisposableShopProduct child in gameObject.GetComponentsInChildren<DisposableShopProduct>())
            {
                if (child is QuantitativeShopProduct) continue;
                _disposableProduct.TryAdd(child, 0);
            }
        }
    }
}