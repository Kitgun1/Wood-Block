using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WoodBlock
{
    public class QuantitativeShopProduct : DisposableShopProduct
    {
        [SerializeField] protected TMP_Text TMPAmount;

        [ReadOnly, SerializeField] private List<RectTransform> _layouts = new();

        private int _amountReward;

        protected int AmountReward
        {
            get => _amountReward;
            set
            {
                _amountReward = value;
                TMPAmount.text = value.ToString();
                UpdateAllLayouts();
            }
        }

        public DisposableShopProduct SetAmountReward(int value)
        {
            AmountReward = value;
            return this;
        }

        private void UpdateAllLayouts()
        {
            foreach (RectTransform layout in _layouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }
        }

        [Button]
        private void GetLayouts()
        {
            var layouts = transform.GetComponentsInChildren<LayoutGroup>();
            _layouts = layouts.Select(layout => layout.GetComponent<RectTransform>()).ToList();
        }
    }
}