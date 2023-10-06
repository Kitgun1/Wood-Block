using System;
using KimicuUtility;
using UnityEditor;
using UnityEngine;

namespace WoodBlock
{
    [Serializable] public class DictionaryVector2CellInBlock : SerializableDictionary<Vector2, CellInBlock> { }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DictionaryVector2CellInBlock))] 
    public class DictionaryVector2CellInBlockDrawer : DictionaryDrawer<Vector2, CellInBlock> { }
#endif

    [Serializable] public class DictionaryWindows : SerializableDictionary<WindowType, CanvasGroup> { }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DictionaryWindows))] 
    public class DictionaryWindowsDrawer : DictionaryDrawer<WindowType, CanvasGroup> { }
#endif

    [Serializable] public class DictionaryShopWindow : SerializableDictionary<ShopPageType, CanvasGroup> { }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DictionaryShopWindow))] 
    public class DictionaryShopWindowDrawer : DictionaryDrawer<ShopPageType, CanvasGroup> { }
#endif

    [Serializable] public class DictionaryQuantitativeProductReward : SerializableDictionary<QuantitativeShopProduct, int> { }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DictionaryQuantitativeProductReward))] 
    public class DictionaryQuantitativeProductRewardDrawer : DictionaryDrawer<QuantitativeShopProduct, int> { }
#endif

    [Serializable] public class DictionaryDisposableProductReward : SerializableDictionary<DisposableShopProduct, int> { }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DictionaryDisposableProductReward))] 
    public class DictionaryDisposableProductRewardDrawer : DictionaryDrawer<DisposableShopProduct, int> { }
#endif
}