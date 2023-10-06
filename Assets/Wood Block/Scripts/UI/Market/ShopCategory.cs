using System;
using System.Collections.Generic;
using System.Linq;
using Agava.YandexGames;
using KimicuUtility;
using NaughtyAttributes;
using UnityEngine;
using Billing = KiYandexSDK.Billing;

namespace WoodBlock
{
    public class ShopCategory : MonoBehaviour
    {
        [SerializeField] private CategoryProductType _categoryProductType;

        [SerializeField, EnableIf(nameof(_categoryProductType), CategoryProductType.Quantitative)]
        private DictionaryQuantitativeProductReward _quantitativeProduct = new();

        [SerializeField, EnableIf(nameof(_categoryProductType), CategoryProductType.Disposable)]
        private DictionaryDisposableProductReward _disposableProduct = new();

        private void OnEnable()
        {
            switch (_categoryProductType)
            {
                case CategoryProductType.Quantitative:
                    foreach ((QuantitativeShopProduct product, int reward) in _quantitativeProduct)
                    {
                        CatalogProduct selected = Billing.CatalogProduct.First(p => p.id == product.ProductId);
                        product.SetPriceJan(int.Parse(selected.price));
                        StartCoroutine(selected.imageURI.GetPicture(
                            texture2D => product.SetPicture(
                                Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                                    Vector2.one / 2))));
                        product.SetAmountReward(reward);
                        product.OnClickBuyJan.AddListener(QuantitativeProductPurchaseSuccessCallback);
                    }

                    break;
                case CategoryProductType.Disposable:
                    foreach ((DisposableShopProduct product, int price) in _disposableProduct)
                    {
                        CatalogProduct selected = Billing.CatalogProduct.First(p => p.id == product.ProductId);
                        product.SetPriceJan(int.Parse(selected.price));
                        StartCoroutine(selected.imageURI.GetPicture(
                            texture2D => product.SetPicture(
                                Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                                    Vector2.one / 2))));
                        product.OnClickBuyJan.AddListener(QuantitativeProductPurchaseSuccessCallback);
                    }

                    break;
                case CategoryProductType.None:
                default:
                    break;
            }
        }

        private void QuantitativeProductPurchaseSuccessCallback(string productId, int amount)
        {
            Billing.PurchaseProduct(productId, null, () => { }, onErrorConsume: Debug.LogWarning);
        }

        [Button, EnableIf(nameof(_categoryProductType), CategoryProductType.Quantitative)]
        private void FillQuantitativeDictionary()
        {
            foreach (QuantitativeShopProduct child in gameObject.GetComponentsInChildren<QuantitativeShopProduct>())
            {
                _quantitativeProduct.TryAdd(child, 0);
            }
        }

        [Button, EnableIf(nameof(_categoryProductType), CategoryProductType.Disposable)]
        private void FillDisposableDictionary()
        {
            foreach (DisposableShopProduct child in gameObject.GetComponentsInChildren<DisposableShopProduct>())
            {
                Debug.Log((QuantitativeShopProduct)child);
                if ((QuantitativeShopProduct)child != null) continue;
                _disposableProduct.TryAdd(child, 0);
            }
        }
    }
}