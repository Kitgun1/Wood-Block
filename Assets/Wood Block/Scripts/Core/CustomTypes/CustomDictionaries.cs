using System;
using KimicuUtility;
using UnityEditor;
using UnityEngine;

namespace WoodBlock
{
    [Serializable]
    public class DictionaryVector2CellInBlock : SerializableDictionary<Vector2, CellInBlock>
    {
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(DictionaryVector2CellInBlock))]
    public class DictionaryVector2CellInBlockDrawer : DictionaryDrawer<Vector2, CellInBlock>
    {
    }

#endif

    [Serializable]
    public class DictionaryWindows : SerializableDictionary<WindowType, CanvasGroup>
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DictionaryWindows))]
    public class DictionaryWindowsDrawer : DictionaryDrawer<WindowType, CanvasGroup>
    {
    }
#endif
}