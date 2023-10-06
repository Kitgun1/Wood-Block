using System;

namespace WoodBlock
{
    [Flags]
    public enum ShopPageType
    {
        None = 0,
        CurrencyPage = 1 << 0,
        SkinsPage = 1 << 1,
        DiscountsPage = 1 << 2,
        BonusesPage = 1 << 3
    }
}