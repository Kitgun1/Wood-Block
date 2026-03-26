using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScrollContentAutoSize : MonoBehaviour
{

    private GridLayoutGroup _root;
    private RectTransform _rectTransform;
    private RectTransform _scrollRectTransform;

    private void Start()
    {
        _root = GetComponent<GridLayoutGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _scrollRectTransform = _root.GetComponent<RectTransform>();
    }

    private void FixedUpdate() => CalculateSize();

    private void CalculateSize()
    {
        int totalChildren = _root.transform.childCount;
        int rows = Mathf.CeilToInt((float)totalChildren / CalculateColumnsFromWidth());

        float height = (_root.cellSize.y + _root.spacing.y) * rows;
        
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, height);
    }

    private int CalculateColumnsFromWidth()
    {
        float availableWidth = _scrollRectTransform.rect.width - _root.padding.left - _root.padding.right;

        float cellWidth = _root.cellSize.x + _root.spacing.x;
        int possibleColumns = Mathf.FloorToInt(availableWidth / cellWidth);

        return Mathf.Max(1, possibleColumns);
    }
}
