using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField, Range(0, 1)] private float _scrollOffset = 1f;
    [SerializeField] private bool _isVertical = true;

    public void SetValue()
    {
        if (_scrollRect is null)
            Debug.LogError("ScrollRect is null");
        if (_isVertical)
            _scrollRect.verticalNormalizedPosition = _scrollOffset;
        else
            _scrollRect.horizontalNormalizedPosition = _scrollOffset;
    }
}
