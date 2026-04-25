using Kimicu.YandexGames;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestGenerator _questGenerator;
    [SerializeField] private int _currentLevel = 1;

    [Header("Префабы UI")]
    [SerializeField] private GameObject _questPrefab;
    [SerializeField] private TMP_Text _levelText;
    [Header("Container")]
    [SerializeField] private Transform _questContainer;

    [Header("Настройки авто-обновления")]
    [SerializeField] private bool _autoGenerateOnStart = true;
    [SerializeField] private bool _clearContainerOnStart = true;


    private List<Quest> _activeQuests = new List<Quest>();
    private List<QuestUI> _activeQuestUIs = new List<QuestUI>();

    private bool _isAlreayComplete = false;

    public event Action<Quest> OnQuestCompleted;
    public event Action<Quest> OnQuestAdded;
    public UnityEvent OnAllQuestCompleted;
    public UnityEvent OnQuestFailed;

    private void Start()
    {
        // Очищаем контейнер при старте
        if (_clearContainerOnStart)
        {
            ClearQuestContainer();
        }

        // Автоматически генерируем квесты при старте
        if (_autoGenerateOnStart)
        {
            GenerateInitialQuests();
        }

        _levelText.text = _currentLevel.ToString();
    }
    private void Update()
    {
        // Обновляем таймеры активных квестов
        foreach (var quest in _activeQuests)
        {
            quest.UpdateTimer(Time.deltaTime);

            if (quest.IsTimeOut)
            {
                HandleQuestTimeOut(quest);
            }
        }

        if (_isAlreayComplete == false && _activeQuests.All(x => x.IsCompleted))
        {
            OnAllQuestCompleted?.Invoke();
            _isAlreayComplete = true;
        }

    }
    private void ClearQuestContainer()
    {

        foreach (Transform child in _questContainer)
        {
            Destroy(child.gameObject);
        }
        _activeQuestUIs.Clear();
    }
    private void GenerateInitialQuests()
    {
        if (DataSaver.HasSaves(SaveKeys.LevelQuests))
        {
            var questList = DataSaver.Load<List<QuestData>>(SaveKeys.LevelQuests);
            if (questList.Count >= _currentLevel)
            {
                var questData = questList[_currentLevel - 1];
                AddQuest(new Quest(questData.QuestType,questData.TargetBlock,_currentLevel,questData.TimeLimit));
            }
            else
            {
                AddNewQuest();
                var newQuestData = new QuestData(_activeQuests[0].TargetBlock, _activeQuests[0].QuestType, _activeQuests[0].TimeLimit);
                questList.Add(newQuestData);
                DataSaver.Save(SaveKeys.LevelQuests, questList);
            }
        }
        else
        {
            AddNewQuest();
            var questData = new QuestData(_activeQuests[0].TargetBlock, _activeQuests[0].QuestType, _activeQuests[0].TimeLimit);
            var newList = new List<QuestData>() { questData };
            DataSaver.Save(SaveKeys.LevelQuests,newList);
        }
    }

    private QuestUI CreateQuestUI(Quest quest)
    {
        if (_questPrefab == null || _questContainer == null) return null;

        GameObject questObject;

        questObject = Instantiate(_questPrefab, _questContainer);

        questObject.name = $"Quest_{_activeQuests.Count + 1}_{quest.QuestType}";

        // Получаем компонент QuestUI
        QuestUI questUI = questObject.GetComponent<QuestUI>();
        if (questUI == null)
        {
            Debug.LogError("QuestUI компонент не найден на префабе!");
            Destroy(questObject);
            return null;
        }

        // Настраиваем UI
        questUI.Setup(quest);

        return questUI;
    }

    public void SetLevelNumber(int level) => _currentLevel = level;

    // Добавить новый квест
    public void AddNewQuest()
    {

        Quest newQuest = _questGenerator.GenerateQuest(_currentLevel);
        newQuest.Activate();

        QuestUI questUI = CreateQuestUI(newQuest);
        if (questUI != null)
        {
            _activeQuestUIs.Add(questUI);
        }

        _activeQuests.Add(newQuest);
        OnQuestAdded?.Invoke(newQuest);

        newQuest.OnCompleted += HandleQuestComplete;
        newQuest.OnTimeOut += HandleQuestTimeOut;
    }

    // Добавить конкретный квест
    public void AddQuest(Quest quest)
    {

        quest.Activate();
        _activeQuests.Add(quest);
        OnQuestAdded?.Invoke(quest);

        QuestUI questUI = CreateQuestUI(quest);
        if (questUI != null)
        {
            _activeQuestUIs.Add(questUI);
        }

        quest.OnCompleted += HandleQuestComplete;
        quest.OnTimeOut += HandleQuestTimeOut;
    }

    public void ReportBlocksCollected(int count)
    {
        foreach (var quest in _activeQuests)
        {
            quest.AddProgress(count);
        }
    }

    // Обработчик завершения квеста
    private void HandleQuestComplete(Quest quest)
    {
        quest.OnCompleted -= HandleQuestComplete;
        quest.OnTimeOut -= HandleQuestTimeOut;

        if (_activeQuests.Contains(quest))
        {
            _activeQuests.Remove(quest);
            OnQuestCompleted?.Invoke(quest);
        }
    }

    // Обработчик тайм-аута
    private void HandleQuestTimeOut(Quest quest)
    {
        quest.OnCompleted -= HandleQuestComplete;
        quest.OnTimeOut -= HandleQuestTimeOut;

        OnQuestFailed?.Invoke();
    }

    // Получить активные квесты
    public List<Quest> GetActiveQuests()
    {
        return new List<Quest>(_activeQuests);
    }
}