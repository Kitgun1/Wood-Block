using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestScoreText : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    public void Boot()
    {
        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
            _scoreText.text = $"Лучший счёт: {DataSaver.Load<int>(SaveKeys.BestScore)}";
        else
            _scoreText.text = $"Best score: {DataSaver.Load<int>(SaveKeys.BestScore)}";
    }

}
