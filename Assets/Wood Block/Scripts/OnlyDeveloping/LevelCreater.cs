#if UNITY_EDITOR
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using WoodBlock;
using Scene = UnityEngine.SceneManagement.Scene;

public class LevelCreater : MonoBehaviour
{
    [SerializeField] private int _maxLevelCount;

    [Header("Levels")]
    [SerializeField] private string _baseLevelPath;
    [SerializeField] private string _savePath;

    [Header("UI")]
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _root;
    [field: SerializeField] private List<Button> _buttons;

    [Button]
    public void GenerateLevel()
    {
        Scene baseScene = EditorSceneManager.OpenScene(_baseLevelPath, OpenSceneMode.Single);

        for (int levelNum = 1; levelNum <= _maxLevelCount; levelNum++)
        {
            CreateLevelScene(levelNum);
            if (_buttons != null || _buttons.Count != 0)
            {
                _buttons[levelNum - 1].onClick.RemoveAllListeners();
                _buttons[levelNum - 1].onClick.AddListener(() => { SceneLoader.LoadScene($"Level{levelNum + 2}"); });
            }
        }
        AssetDatabase.Refresh();
    }

    [Button]
    public void GenerateUI()
    {
        for (int i = 0; i < _maxLevelCount; i++)
        {
            var uiObject = Instantiate(_prefab, _root);
            uiObject.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
        }
    }

    private void CreateLevelScene(int levelNumber)
    {
        string scenePath = $"{_savePath}/Level{levelNumber}.unity";

        QuestManager manager = FindFirstObjectByType<QuestManager>();
        QuestGenerator questGenerator = FindFirstObjectByType<QuestGenerator>();

        if (manager != null)
        {
            manager.SetLevelNumber(levelNumber);
            CalculateDifficulte(levelNumber, manager, questGenerator);

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
        else
        {
            Debug.LogWarning($"LevelGenerator not found in base scene for level {levelNumber}");
        }

        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), scenePath);
    }

    private void CalculateDifficulte(int level, QuestManager manager, QuestGenerator generator)
    {
        if (level >= _maxLevelCount / 2)
            manager.SetMaxQuestCount(2);
        else
            manager.SetMaxQuestCount(1);

        var chance = Mathf.Clamp((float)level / _maxLevelCount * 100, 0, 80);

        generator.TimedQuestChanceSet(Convert.ToInt32(chance));

    }
}
#endif
