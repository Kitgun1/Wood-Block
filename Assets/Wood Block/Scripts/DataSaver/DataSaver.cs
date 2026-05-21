using Kimicu.YandexGames;
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
        {SaveKeys.CurrentLevel, "levels" },
        {SaveKeys.LevelQuests, "levelQuests" },
        {SaveKeys.BestScore, "bestScore" }
    };

    public static bool HasSaves(SaveKeys type) => Cloud.HasKey(SavesKeys[type]);
    public static void Save<T>(SaveKeys type, T value)
    {
        //string key = SavesKeys[type];

        //if (Cloud.HasKey(key))
        //{
        //    try
        //    {
        //        T existingValue = Cloud.GetValue<T>(key);

        //        if (AreValuesEqual(existingValue, value))
        //        {
        //            Debug.Log($"DataSaver: Значение для ключа '{key}' не изменилось. Сохранение пропущено.");
        //            return;
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.LogWarning($"DataSaver: Ошибка при проверке данных для ключа '{key}': {e.Message}");
        //    }
        //}
        Cloud.SetValue(SavesKeys[type], value, true, onErrorCallback: Debug.LogError);
    }
    public static T Load<T>(SaveKeys type)
    {
        if (HasSaves(type))
            return Cloud.GetValue<T>(SavesKeys[type]);
        else
            return default(T);
    }


    private static bool AreValuesEqual<T>(T value1, T value2)
    {
        if (value1 == null && value2 == null) return true;
        if (value1 == null || value2 == null) return false;

        if (value1 is string str1 && value2 is string str2)
            return str1 == str2;

        if (value1 is float f1 && value2 is float f2)
            return Mathf.Approximately(f1, f2);

        if (value1 is double d1 && value2 is double d2)
            return System.Math.Abs(d1 - d2) < 0.0001;

        if (value1 is bool b1 && value2 is bool b2)
            return b1 == b2;

        if (value1 is System.Collections.IList list1 && value2 is System.Collections.IList list2)
            return AreListsEqual(list1, list2);

        return value1.Equals(value2);
    }

    private static bool AreListsEqual(System.Collections.IList list1, System.Collections.IList list2)
    {
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (!AreValuesEqual(list1[i], list2[i]))
                return false;
        }

        return true;
    }
}
