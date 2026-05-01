using Newtonsoft.Json;
using Playgama;
using Playgama.Modules.Storage;
using System;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.InputSystem;

public static class DataSaver
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

    public static bool HasSaves(SaveKeys type)
    {
        bool isSeccess = false;
        Bridge.storage.Get(SavesKeys[type], (seccess, data) => { isSeccess = seccess; });
        return isSeccess;
    }

    public static void Save<T>(SaveKeys key, T data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        Debug.Log($"[Save] Сохраняю {key}: {jsonData}");

        Bridge.storage.Set(SavesKeys[key], jsonData, storageType: StorageType.PlatformInternal);
    }

    public static T Load<T>(SaveKeys key, T defaultValue = default(T))
    {
        bool isSuccess = false;
        string jsonData = "";
        Bridge.storage.Get(SavesKeys[key], (success, data) => { isSuccess = success;jsonData = data; }, StorageType.PlatformInternal);
        if (isSuccess)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.Log($"[Load] Данных по ключу {key} нет. Использую значение по умолчанию.");
                return defaultValue;
            }

            T loadedData = JsonConvert.DeserializeObject<T>(jsonData);
            Debug.Log($"[Load] Загружено {key}: {jsonData}");
            return loadedData;
        }
        else
        {
            Debug.Log("[LOAD] Не удалось загрузить сохранение");
            return defaultValue;
        }
    }
}
