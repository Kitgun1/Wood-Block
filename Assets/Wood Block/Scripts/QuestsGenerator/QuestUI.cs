using Lean.Localization;
using System;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _timerText; // Для timed квестов

    private Quest _currentQuest;


    public void Setup(Quest quest)
    {
        _currentQuest = quest;

        // Подписываемся на события
        quest.OnProgressChanged += UpdateUI;
        quest.OnCompleted += OnQuestCompleted;

        if (quest.QuestType == QuestType.CollectTimed)
            _timerText.gameObject.SetActive(true);

        UpdateUI(quest);
    }

    private void Update()
    {
        if (_currentQuest != null && _currentQuest.IsActive && _currentQuest.QuestType == QuestType.CollectTimed)
        {
            UpdateTimerUI(_currentQuest);
        }
    }

    private void UpdateUI(Quest quest)
    {
        var serparatedString = quest.GetDescription();
        switch (serparatedString.Item1)
        {
            case QuestType.CollectBlocks:
                if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
                    _descriptionText.text = $"Собрать {quest.CurrentProgress}/{serparatedString.Item2} очков";
                else
                    _descriptionText.text = $"Collect {quest.CurrentProgress}/{serparatedString.Item2} points";
                break;
            case QuestType.CollectTimed:
                if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
                    _descriptionText.text =  $"Собрать {quest.CurrentProgress}/{serparatedString.Item2} очков за {serparatedString.Item3} сек";
                else
                    _descriptionText.text = $"Collect {quest.CurrentProgress}/{serparatedString.Item2} points for {serparatedString.Item3} sec";
                break;
            default:
                Debug.LogError("Unknown quest");
                break;
        }
    }

    private void UpdateTimerUI(Quest quest)
    {
        TimeSpan time = TimeSpan.FromSeconds(quest.TimeRemaining);
        _timerText.text = $"{time.Minutes:00}:{time.Seconds:00}";
    }

    private void OnQuestCompleted(Quest quest)
    {
        // Отписываемся от событий
        quest.OnProgressChanged -= UpdateUI;
        quest.OnCompleted -= OnQuestCompleted;
    }

    private void OnDestroy()
    {
        if (_currentQuest != null)
        {
            _currentQuest.OnProgressChanged -= UpdateUI;
            _currentQuest.OnCompleted -= OnQuestCompleted;
        }
    }
}
