using Kimicu.YandexGames;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver
{
    public static readonly Dictionary<SaveKeys, string> SavesKeys = new() 
    {
        {SaveKeys.Products,"items" },
        {SaveKeys.SelectedBackgroundId,"selectedBGId" },
        {SaveKeys.SelectedSkinId,"selectedSkinId" },
        {SaveKeys.MusicVolume,"musicVolume" },
        {SaveKeys.SoundsVolume,"soundsVolume" },
        {SaveKeys.CurrentLevel, "levels" }
    };

    public static bool HasSaves(SaveKeys type) => Cloud.HasKey(SavesKeys[type]);
    public static void Save<T>(SaveKeys type, T value)
    {
        if (Cloud.HasKey(SavesKeys[type]))
        {
                Cloud.SetValue(SavesKeys[type], value,true, onErrorCallback: Debug.LogError);
        }
        else
            Cloud.SetValue(SavesKeys[type], value,true, onErrorCallback: Debug.LogError);
    }
    public static T Load<T>(SaveKeys type) 
    {
        if (HasSaves(type))
            return Cloud.GetValue<T>(SavesKeys[type]);
        else
            return default(T);
    }
}
