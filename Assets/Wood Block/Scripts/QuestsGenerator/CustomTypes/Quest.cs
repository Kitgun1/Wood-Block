using Lean.Localization;
using System;
using UnityEngine;

public class Quest
{
    private QuestType _type;
    private int _targetBlocks;
    private float _timeLimit;           // Для timed квестов (в секундах)
    private int _currentProgress;
    private int _level;                 // Уровень сложности квеста
    private bool _isCompleted;
    private bool _isActive;

    // Для timed квестов
    private float _timeRemaining;
    private bool _isTimeOut;

    // События
    public event Action<Quest> OnProgressChanged;
    public event Action<Quest> OnCompleted;
    public event Action<Quest> OnTimeOut;

    public bool IsTimeOut { get => _isTimeOut; }
    public int CurrentProgress { get => _currentProgress; }
    public float TimeRemaining { get => _timeRemaining; }
    public QuestType QuestType { get => _type; }
    public bool IsActive { get => _isActive; }
    public bool IsCompleted { get => _isCompleted; }

    public Quest(QuestType questType, int target, int questLevel, float time = 0)
    {
        _type = questType;
        _targetBlocks = target;
        _level = questLevel;
        _currentProgress = 0;
        _isCompleted = false;
        _isActive = false;

        if (_type == QuestType.CollectTimed)
        {
            _timeLimit = time;
            _timeRemaining = _timeLimit;
            _isTimeOut = false;
        }
    }

    // Добавить прогресс
    public void AddProgress(int amount)
    {
        if (!_isActive || _isCompleted || _isTimeOut) return;

        _currentProgress += amount;
        OnProgressChanged?.Invoke(this);

        if (_currentProgress >= _targetBlocks)
        {
            CompleteQuest();
        }
    }
    private void CompleteQuest()
    {
        _isCompleted = true;
        _isActive = false;
        OnCompleted?.Invoke(this);
    }

    // Обновление таймера (вызывать каждый кадр)
    public void UpdateTimer(float deltaTime)
    {
        if (!_isActive || _isCompleted || _type != QuestType.CollectTimed) return;

        _timeRemaining -= deltaTime;

        if (_timeRemaining <= 0)
        {
            _timeRemaining = 0;
            _isTimeOut = true;
            _isActive = false;
            OnTimeOut?.Invoke(this);
        }
    }

    // Активировать квест
    public void Activate()
    {
        _isActive = true;
        _currentProgress = 0;

        if (_type == QuestType.CollectTimed)
        {
            _timeRemaining = _timeLimit;
            _isTimeOut = false;
        }
    }

    // Получить описание квеста
    public (QuestType,int,float?) GetDescription()
    {
        return (_type, _targetBlocks, _timeLimit);
    }
}

