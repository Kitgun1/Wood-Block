using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct SerializebleRangeInt
{
    [SerializeField]private int _start;
    [SerializeField]private int _end;

    public int Start
    {
        get => _start;
        set
        {
            if (value > 0)
                _start = value;
            else
                _start = 1;
        }
    }
    public int End
    {
        get => _end;
        set
        {
            if (value > _start)
                _end = value;
            else
                _end = _start + 1;
        }
    }
    public int GetRandomValue => Random.Range(Start,End);

    public SerializebleRangeInt(int start, int end)
    {
        _start = 1;
        _end = 2;
        Start = start;
        End = end;
    }

}
