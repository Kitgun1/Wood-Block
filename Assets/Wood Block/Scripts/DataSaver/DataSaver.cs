using System;
using Newtonsoft.Json;
using Playgama;
using Playgama.Modules.Storage;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public static class DataSaver
{
    public static readonly Dictionary<SaveKeys, IDefualtSave> SavesKeys = new()
    {
        {SaveKeys.Products, new DefualtSave<List<string>>("items", new List<string>()) },
        {SaveKeys.SelectedBackgroundId, new DefualtSave<string>("selectedBGId", "base_bg") },
        {SaveKeys.SelectedSkinId, new DefualtSave<string>("selectedSkinId", "base_skin") },
        {SaveKeys.MusicVolume, new DefualtSave<float>("musicVolume", 1f) },
        {SaveKeys.SoundsVolume, new DefualtSave<float>("soundsVolume", 1f) },
        {SaveKeys.CurrentLevel, new DefualtSave<int>("levels", 1) },
        {SaveKeys.LevelQuests, new DefualtSave<List<QuestData>>("levelQuests", new List<QuestData>()) },
        {SaveKeys.BestScore, new DefualtSave<int>("bestScore", 0) }
    };

    private static bool _isInitialized;

    public static async UniTask Initialize()
    {
        foreach(var value in SavesKeys)
        {
            if (!PlayerPrefs.HasKey(value.Value.Key))
            {
                string json = JsonConvert.SerializeObject(value.Value.DefualtSavesValue);
                PlayerPrefs.SetString(value.Value.Key, json);
                PlayerPrefs.Save();
            }
        }

        _isInitialized = true;
    }
    public static void Save<T>(SaveKeys key, T data)
    {
        if (_isInitialized)
        {
            string json = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(SavesKeys[key].Key, json);
            PlayerPrefs.Save();
            
          
                try 
                {
#if UNITY_WEBGL
                    Bridge.storage?.Set(SavesKeys[key].Key, json);
#else
                    // Playgama Storage is skipped locally.
#endif
                } 
                catch(Exception e)
                {
                    Debug.LogWarning("Playgama storage set error: " + e.Message);
                }
            
        }
        else
            Debug.LogError("DataSaver isnt initilized!");
    }

    public static T Load<T>(SaveKeys key)
    {
        if (_isInitialized)
        {
            string json = PlayerPrefs.GetString(SavesKeys[key].Key);

            if (string.IsNullOrEmpty(json))
                return default(T);

            return JsonConvert.DeserializeObject<T>(json);
        }
        else
        {
            Debug.LogError("DataSaver isnt initilized!");
            return default(T);
        }
    }

    public static bool HasSaves(SaveKeys key)
    {
        if (_isInitialized)
            return PlayerPrefs.HasKey(SavesKeys[key].Key);
        else
        {
            Debug.LogError("DataSaver isnt initilized!");
            return false;
        }
    }

    public static void Clear()
    {
        try
        {

            foreach (var pair in SavesKeys)
            {
                Bridge.storage.Set(pair.Value.Key, "", storageType: StorageType.PlatformInternal);
                PlayerPrefs.DeleteKey(pair.Value.Key);
            }
            Debug.Log("[DataSaver] Все сохранения были очищены через PlaygamaBridge!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DataSaver] Не удалось очистить сохранения: {ex.Message}");
        }
    }
    
    
#if UNITY_EDITOR
     [MenuItem("Tools/Clear All Saves")]
    public static void ClearAllSaves()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Clear Saves", "Пожалуйста, запустите игру (Play Mode) перед очисткой сохранений, чтобы PlaygamaBridge SDK был активен.", "OK");
            return;
        }
        Clear();
        EditorUtility.DisplayDialog("Clear Saves", "Все сохранения были успешно сброшены через PlaygamaBridge!", "OK");
    }
#endif
}

public interface IDefualtSave
{
    public string Key { get; }
    public object DefualtSavesValue { get; }
}
public class DefualtSave<T>:IDefualtSave
{
    public string Key { get; private set; }
    private T DefualtValue { get; set; }

    public object DefualtSavesValue => DefualtValue;

    public DefualtSave(string key, T value)
    {
        Key = key;
        DefualtValue = value;
    }
}