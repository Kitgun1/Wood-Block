using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestData
{
    public int TargetBlock { get; set; }
    public float TimeLimit { get; set; }
    public QuestType QuestType { get; set; }

    public QuestData(int targetBlock, QuestType type,float timeLimit)
    {
        TargetBlock = targetBlock;
        QuestType = type;
        TimeLimit = timeLimit;
    }
}
