using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class CatalogProduct
{
    public string ID { get; private set; }
    public string Price { get; private set; }
    public string Name { get; private set; }
    public Sprite Image { get; private set; }

    public CatalogProduct(string id, string price, string name, Sprite image)
    {
        ID = id;
        Price = price;
        Name = name;
        Image = image;
    }
}

[Serializable]
public class ProductSetting
{
    [field:SerializeField] public string RuTitle { get; set; }
    [field: SerializeField] public string EnTitle { get; set; }
    [field: SerializeField] public Sprite Image { get; set; }
}
