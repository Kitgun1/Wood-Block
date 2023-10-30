using System;
using System.Collections.Generic;
using KimicuUtility;
using NaughtyAttributes;
using RelativeLayout;
using UnityEngine;
using UnityEngine.UI;

namespace WoodBlock
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class FlexGrid : MonoBehaviour
    {
        [SerializeField] private DictionaryFloatInt _amountXElementByWidthScreen = new();
        [SerializeField] private RectTransform _rectParent;
        [SerializeField] private RectTransform _child;

        private int _amountXElement = 1;
        private GridLayoutGroup _grid;

        [Button]
        private void UpdateLayout()
        {
            CalculateAmountXElement();
            float x = CalculateWidth();
            float y = CalculateHeight(x);
            _grid.cellSize = new Vector2(x, y);
            _grid.constraintCount = _amountXElement;
        }

        private void Awake()
        {
            _grid ??= GetComponent<GridLayoutGroup>();
        }

        private void OnValidate()
        {
            _grid ??= GetComponent<GridLayoutGroup>();

            UpdateLayout();
        }

        private void Update()
        {
            UpdateLayout();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_grid.GetComponent<RectTransform>());
        }

        private void CalculateAmountXElement()
        {
            _amountXElement = 1;
            foreach (var pair in _amountXElementByWidthScreen)
            {
                if (_rectParent.rect.width > pair.Key)
                {
                    _amountXElement = pair.Value;
                    return;
                }
            }
        }

        private float CalculateWidth()
        {
            float width = _rectParent.rect.width - _grid.padding.horizontal;
            width -= (_amountXElement - 1) * _grid.spacing.x;
            return width / _amountXElement;
        }

        private float CalculateHeight(float gridCellSizeX)
        {
            float height = gridCellSizeX * (1 / 0.6875f);
            return height;
        }
    }
}