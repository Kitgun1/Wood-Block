using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    [Header("Настройки генерации")]
    [SerializeField] private int _startBlocks = 50;        
    [SerializeField] private int _blocksIncrease = 25;     
    [SerializeField] private float _startTime = 60f;       
    [SerializeField] private float _timeDecrease = 5f;           

    [Header("Вероятности")]
    [Range(0, 100)]
    [SerializeField] private int _timedQuestChance = 40;   // Шанс timed квеста в %

    // Сгенерировать новый квест
    public Quest GenerateQuest(int level)
    {
        // Определяем тип квеста
        QuestType type = UnityEngine.Random.Range(0, 100) < _timedQuestChance
            ? QuestType.CollectTimed
            : QuestType.CollectBlocks;

        // Рассчитываем целевое количество блоков
        int targetBlocks = CalculateTargetBlocks(level);

        if (type == QuestType.CollectBlocks)
        {
            return new Quest(type, targetBlocks, level);
        }
        else
        {
            // Рассчитываем время
            float timeLimit = CalculateTimeLimit(level);
            return new Quest(type, targetBlocks, level, timeLimit);
        }
    }

    public int TimedQuestChanceSet(int value) => _timedQuestChance = value;

    // Рассчитать целевое количество блоков
    private int CalculateTargetBlocks(int level)
    {
        int baseBlocks = _startBlocks + (level - 1) * _blocksIncrease;
        int randomVariance = UnityEngine.Random.Range(-10, 11);

        return Mathf.Max(10, baseBlocks + randomVariance);
    }

    // Рассчитать лимит времени
    private float CalculateTimeLimit(int level)
    {
        return _startTime + (level - 1) * _timeDecrease;
    }

    public List<Quest> GenerateQuests(int count, int baseLevel)
    {
        List<Quest> quests = new List<Quest>();

        for (int i = 0; i < count; i++)
        {
            int levelOffset = UnityEngine.Random.Range(-1, 2);
            int questLevel = Mathf.Max(1, baseLevel + levelOffset);

            Quest quest = GenerateQuest(questLevel);
            quests.Add(quest);
        }

        return quests;
    }

}

