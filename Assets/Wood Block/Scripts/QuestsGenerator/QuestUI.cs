using System;
using TMPro;
using UnityEngine;
using Lean.Localization;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _descriptionText;

    private Quest _currentQuest;

    public QuestType Type => _currentQuest.QuestType;

    public void Setup(Quest quest)
    {
        _currentQuest = quest;

        quest.OnProgressChanged += UpdateUI;
        quest.OnCompleted += OnQuestCompleted;

        UpdateUI(quest);
    }

    private void Update()
    {
        if (_currentQuest != null && _currentQuest.IsActive && _currentQuest.QuestType == QuestType.CollectTimed)
        {
            UpdateUI(_currentQuest);
        }
    }

    private void UpdateUI(Quest quest)
    {
        if (quest == null) return;

        var serparatedString = quest.GetDescription();
        bool isRussian = LeanLocalization.GetFirstCurrentLanguage() == "Russian";
        switch (serparatedString.Item1)
        {
            case QuestType.CollectBlocks:
                if (isRussian)
                    _descriptionText.text = $"Соберите {quest.CurrentProgress}/{serparatedString.Item2} очков";
                else
                    _descriptionText.text = $"Collect {quest.CurrentProgress}/{serparatedString.Item2} points";
                break;
            case QuestType.CollectTimed:
                TimeSpan time = TimeSpan.FromSeconds(quest.TimeRemaining);
                if (isRussian)
                    _descriptionText.text =  $"Соберите {quest.CurrentProgress}/{serparatedString.Item2} очков за {ValidateTime(serparatedString.Item3)}  {time.Minutes:00}:{time.Seconds:00}";
                else
                    _descriptionText.text = $"Collect {quest.CurrentProgress}/{serparatedString.Item2} points for {ValidateTime(serparatedString.Item3)} {time.Minutes:00}:{time.Seconds:00}";
                break;
            default:
                Debug.LogError("Unknown quest");
                break;
        }
    }

    private string ValidateTime(float? time)
    {
        if(time != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds((double)time);
            return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
        return "";
    }

    private void OnQuestCompleted(Quest quest)
    {
        quest.OnProgressChanged -= UpdateUI;
        quest.OnCompleted -= OnQuestCompleted;
    }

    private void OnEnable()
    {
        LeanLocalization.OnLocalizationChanged += OnLocalizationChanged;
    }

    private void OnDisable()
    {
        LeanLocalization.OnLocalizationChanged -= OnLocalizationChanged;
    }

    private void OnLocalizationChanged()
    {
        if (_currentQuest != null)
        {
            UpdateUI(_currentQuest);
        }
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
