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
    public static void Save(List<ShopItem> items)
    {
        Cloud.SetValue("skins", items, onErrorCallback: Debug.LogError);
        Cloud.SaveInCloud();
    }
    public static void Save(string selectedSkinId)
    {
        Cloud.SetValue("selectedSkinId", selectedSkinId, onErrorCallback: Debug.LogError);
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

        return (list, selectedItem);

    }
    public static string LoadSelectedSkinData()
    {
        string selectedItem = "";

        if (Cloud.HasKey("selectedSkinId"))
            selectedItem = Cloud.GetValue<string>("selectedSkinId");

        return selectedItem;
    }

    public static bool HasSkinsSaves() => Cloud.HasKey("skins");
    public static bool HasSelectedSkinsSaves() => Cloud.HasKey("selectedSkinId");
}
