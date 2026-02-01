using Kimicu.YandexGames;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopSaver
{
    public static void Save(List<ShopItem> items,string selectedSkinId)
    {
        Cloud.SetValue("skins", items, onErrorCallback: Debug.LogError);
        Cloud.SetValue("selectedSkinId",selectedSkinId,onErrorCallback:Debug.LogError);
        Cloud.SaveInCloud();
    }
    public static (List<ShopItem>, string) LoadData()
    {
        var list = new List<ShopItem>();
        string selectedItem = "";

        if (Cloud.HasKey("skins"))
            list = Cloud.GetValue<List<ShopItem>>("skins");
        else
            list = null;

        if (Cloud.HasKey("selectedSkinId"))
            selectedItem = Cloud.GetValue<string>("selectedSkinId");
        else
            selectedItem = "";

        return (list, selectedItem);

    }

    public static (bool skinSaves, bool selectedItemSaves) HasSaves()
    {
        return (Cloud.HasKey("skins"), Cloud.HasKey("selectedSkinId"));
    }
}
