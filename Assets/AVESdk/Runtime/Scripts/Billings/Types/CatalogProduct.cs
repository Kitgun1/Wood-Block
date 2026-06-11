using System;
using UnityEngine;
using Lean.Localization;

[Serializable]
public class CatalogProduct
{
    public string ID { get; private set; }
    public string Price { get; set; }
    public string RuTitle { get; private set; }
    public string EnTitle { get; private set; }
    public string TitleLocalizationKey { get; private set; }

    public string Name
    {
        get
        {
            string defaultName = (LeanLocalization.GetFirstCurrentLanguage() == "Russian") ? RuTitle : EnTitle;

            if (!string.IsNullOrEmpty(TitleLocalizationKey))
            {
                return LeanLocalization.GetTranslationText(TitleLocalizationKey, defaultName);
            }

            if (!string.IsNullOrEmpty(ID))
            {
                return LeanLocalization.GetTranslationText(ID, defaultName);
            }

            return defaultName;
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public Sprite Image { get; private set; }

    public CatalogProduct(string id, string price, string name, Sprite image, string titleLocalizationKey = null)
    {
        ID = id;
        Price = price;
        RuTitle = name;
        EnTitle = name;
        Image = image;
        TitleLocalizationKey = titleLocalizationKey;
    }

    [Newtonsoft.Json.JsonConstructor]
    public CatalogProduct(string id, string price, string ruTitle, string enTitle, Sprite image, string titleLocalizationKey = null)
    {
        ID = id;
        Price = price;
        RuTitle = ruTitle;
        EnTitle = enTitle;
        Image = image;
        TitleLocalizationKey = titleLocalizationKey;
    }
}

[Serializable]
public class ProductSetting
{
    [field: SerializeField] public string TitleLocalizationKey { get; set; }
    [field: SerializeField] public string RuTitle { get; set; }
    [field: SerializeField] public string EnTitle { get; set; }
    [field: SerializeField] public Sprite Image { get; set; }
}
