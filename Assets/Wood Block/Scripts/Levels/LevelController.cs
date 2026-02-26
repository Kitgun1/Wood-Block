using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [field:SerializeField]private List<Button> _buttons;
    [SerializeField] private bool _isButtonsController;

    private void Start()
    {
        if (_isButtonsController == true)
            UpdateLevelsButtons();
    }
    public void SaveCurrentLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        int levelId = Convert.ToInt32(scene.name.Split('_')[1]);

        if(DataSaver.Load<int>(SaveKeys.CurrentLevel) < levelId + 1)
            DataSaver.Save(SaveKeys.CurrentLevel, levelId + 1);
    }
    private void UpdateLevelsButtons()
    {
        int level = DataSaver.Load<int>(SaveKeys.CurrentLevel);
        for(int i = 0; i < level; i++)
            _buttons[i].interactable = true;
    }
}
