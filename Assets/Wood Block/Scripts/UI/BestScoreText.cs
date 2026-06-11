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
        _scoreText.text = LeanLocalization.GetTranslationText("BestScore", "Best Score: ");
        _scoreText.text += DataSaver.Load<int>(SaveKeys.BestScore).ToString();

    }
    
    private void OnEnable()
    {
        LeanLocalization.OnLocalizationChanged += Boot;
    }

    private void OnDisable()
    {
        LeanLocalization.OnLocalizationChanged -= Boot;
    }


}
