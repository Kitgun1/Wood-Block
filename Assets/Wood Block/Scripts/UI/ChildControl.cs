using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace WoodBlock
{
    [RequireComponent(typeof(RectTransform))]
    public class ChildControl : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private RectTransform _layoutRect;
        private float _padding = 0;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (TryGetComponent(out LayoutGroup layoutGroup))
            {
                _layoutRect = layoutGroup.GetComponent<RectTransform>();
                _padding = layoutGroup.padding.horizontal;
            }
        }

        [Button]
        private void Update()
        {
            var childs = transform.GetComponentsInChildren<RectTransform>().Where(c => c.parent == transform);
            foreach (RectTransform child in childs)
            {
                child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.rect.width - _padding);
            }

            if (_layoutRect == null) return;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutRect);
        }
    }
}