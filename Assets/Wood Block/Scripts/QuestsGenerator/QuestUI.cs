using System;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _progressText;
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
        _descriptionText.text = quest.GetDescription();
        _progressText.text = quest.CurrentProgress.ToString();
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
