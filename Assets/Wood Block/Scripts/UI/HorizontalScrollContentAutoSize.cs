using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalScrollContentAutoSize : MonoBehaviour
{

    private HorizontalLayoutGroup _root;
    private RectTransform _contentRectTransform;
    private RectTransform _itemRectTransform;

    private int _priviousChildCount = 0;


    private void Start()
    {
        _root = GetComponent<HorizontalLayoutGroup>();
        _contentRectTransform = GetComponent<RectTransform>();

        _priviousChildCount = _contentRectTransform.childCount;

        GetItemTransform();
    }

    private void FixedUpdate() { if (_priviousChildCount != _contentRectTransform.childCount) CalculateSize(); }

    private void CalculateSize()
    {
        _priviousChildCount = _contentRectTransform.childCount;

        if (_itemRectTransform is null)
            GetItemTransform();

        if (_root.transform.childCount != 0)
        {
            int totalChildren = _root.transform.childCount;
            float padding = _root.padding.right + _root.padding.left;

            float totalWidth = (_itemRectTransform.rect.width + padding) * totalChildren;

            _contentRectTransform.sizeDelta = new Vector2(totalWidth, _contentRectTransform.rect.height);
        }
    }

    private void GetItemTransform()
    {
        if (_root.transform.childCount != 0)
            if (_itemRectTransform is null)
                _itemRectTransform = _root.transform.GetChild(0).GetComponent<RectTransform>();
    }
}
