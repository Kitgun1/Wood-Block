using System;
using System.Collections.Generic;
using System.Linq;
using KimicuUtility;
using NaughtyAttributes;
using UnityEngine;

namespace WoodBlock
{
    public class ShopController : MonoBehaviour
    {
        #region Flags

        private bool _availableCurrencyPage = false;
        private bool _availableSkinsPage = false;
        private bool _availableDiscountsPage = false;
        private bool _availableBonusesPage = false;

        #endregion

        [SerializeField] private Transform _parentProducts;
        [SerializeField] private DictionaryShopWindow _buttons;

        [SerializeField] private ShopPageType _homePage;
        [SerializeField, EnumFlags] private ShopPageType _availablePages;

        #region Products

        [SerializeField, EnableIf(nameof(_availableBonusesPage))]
        private List<ShopCategory> _bonusesProducts = new();

        [SerializeField, EnableIf(nameof(_availableCurrencyPage))]
        private List<ShopCategory> _currencyProducts = new();

        [SerializeField, EnableIf(nameof(_availableSkinsPage))]
        private List<ShopCategory> _skinsProducts = new();

        [SerializeField, EnableIf(nameof(_availableDiscountsPage))]
        private List<ShopCategory> _discountProducts = new();

        #endregion

        private readonly List<ShopCategory> _spawnedCategories = new();

        private void OnValidate()
        {
            _availableCurrencyPage = (_availablePages & ShopPageType.CurrencyPage) != 0;
            _availableSkinsPage = (_availablePages & ShopPageType.SkinsPage) != 0;
            _availableDiscountsPage = (_availablePages & ShopPageType.DiscountsPage) != 0;
            _availableBonusesPage = (_availablePages & ShopPageType.BonusesPage) != 0;

            _buttons.First(pair => pair.Key == ShopPageType.CurrencyPage).Value.Active(_availableCurrencyPage);
            _buttons.First(pair => pair.Key == ShopPageType.SkinsPage).Value.Active(_availableSkinsPage);
            _buttons.First(pair => pair.Key == ShopPageType.DiscountsPage).Value.Active(_availableDiscountsPage);
            _buttons.First(pair => pair.Key == ShopPageType.BonusesPage).Value.Active(_availableBonusesPage);
        }

        private void OnEnable()
        {
            DrawPage(ShopPageType.BonusesPage);
        }

        private void OnDisable()
        {
        }

        public void DrawPage(ShopPageType pagesType)
        {
            ClearPage();

            switch (pagesType)
            {
                case ShopPageType.CurrencyPage:
                    foreach (ShopCategory currencyProduct in _currencyProducts)
                    {
                        var category = Instantiate(currencyProduct);
                    }
                    break;
                case ShopPageType.SkinsPage:
                    break;
                case ShopPageType.DiscountsPage:
                    break;
                case ShopPageType.BonusesPage:
                    break;
                case ShopPageType.None:
                default:
                    break;
            }
        }

        private void ClearPage()
        {
            foreach (ShopCategory category in _spawnedCategories)
            {
                Destroy(category.gameObject);
            }

            _spawnedCategories.Clear();
        }
    }
}