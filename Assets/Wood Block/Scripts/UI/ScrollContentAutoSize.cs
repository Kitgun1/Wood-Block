using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ScrollContentAutoSize : MonoBehaviour
{
    private GridLayoutGroup _gridLayout;
    private RectTransform _rectTransform;
    private RectTransform _scrollRectTransform;

    private float _lastAvailableWidth;
    private int _lastChildCount;
    private int _lastConstraintCount;

    private void Awake()
    {
        _gridLayout = GetComponent<GridLayoutGroup>();
        _rectTransform = GetComponent<RectTransform>();

        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
            _scrollRectTransform = scrollRect.GetComponent<RectTransform>();
    }

    private void Start()
    {
        CalculateHeight();
    }

    private void Update()
    {
        if (HasChanges())
            CalculateHeight();
    }

    private bool HasChanges()
    {
        if (_scrollRectTransform == null) return false;

        float currentWidth = GetAvailableWidth();
        int currentChildCount = _gridLayout.transform.childCount;
        int currentConstraintCount = GetCurrentColumnCount();

        bool hasChanged = Mathf.Abs(_lastAvailableWidth - currentWidth) > 1f ||
                         _lastChildCount != currentChildCount ||
                         _lastConstraintCount != currentConstraintCount;

        if (hasChanged)
        {
            _lastAvailableWidth = currentWidth;
            _lastChildCount = currentChildCount;
            _lastConstraintCount = currentConstraintCount;
        }

        return hasChanged;
    }

    private void CalculateHeight()
    {
        if (_gridLayout == null || _rectTransform == null) return;

        int totalChildren = _gridLayout.transform.childCount;
        if (totalChildren == 0)
        {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, 0);
            return;
        }

        // Получаем актуальное количество колонок из настроек GridLayoutGroup
        int columns = GetCurrentColumnCount();

        // Рассчитываем количество рядов
        int rows = Mathf.CeilToInt((float)totalChildren / columns);

        // Рассчитываем высоту контента
        float contentHeight = rows * (_gridLayout.cellSize.y + _gridLayout.spacing.y);
        contentHeight -= _gridLayout.spacing.y; // Убираем лишний отступ после последнего ряда
        contentHeight += _gridLayout.padding.top + _gridLayout.padding.bottom;

        // Применяем новую высоту
        Vector2 sizeDelta = _rectTransform.sizeDelta;
        sizeDelta.y = Mathf.Max(0, contentHeight);
        _rectTransform.sizeDelta = sizeDelta;
    }

    private int GetCurrentColumnCount()
    {
        // Определяем режим Constraint
        switch (_gridLayout.constraint)
        {
            case GridLayoutGroup.Constraint.FixedColumnCount:
                return _gridLayout.constraintCount;

            case GridLayoutGroup.Constraint.FixedRowCount:
                // Если фиксированное количество рядов, рассчитываем колонки
                int totalChildren = _gridLayout.transform.childCount;
                int rows = _gridLayout.constraintCount;
                return Mathf.CeilToInt((float)totalChildren / rows);

            case GridLayoutGroup.Constraint.Flexible:
            default:
                // Для Flexible режима рассчитываем колонки на основе доступной ширины
                return CalculateColumnsFromWidth();
        }
    }

    private int CalculateColumnsFromWidth()
    {
        float availableWidth = GetAvailableWidth();

        if (availableWidth <= 0) return 1;

        float availableForCells = availableWidth - _gridLayout.padding.left - _gridLayout.padding.right;
        float cellWidthWithSpacing = _gridLayout.cellSize.x + _gridLayout.spacing.x;

        if (cellWidthWithSpacing <= 0) return 1;

        int possibleColumns = Mathf.FloorToInt(availableForCells / cellWidthWithSpacing);

        return Mathf.Max(1, possibleColumns);
    }

    private float GetAvailableWidth()
    {
        if (_scrollRectTransform == null)
            return Screen.width;

        return _scrollRectTransform.rect.width;
    }

    // Публичный метод для принудительного перерасчета
    public void ForceCalculateHeight()
    {
        CalculateHeight();
    }

    // Обработка изменения размера экрана
    private void OnRectTransformDimensionsChange()
    {
        if (enabled && gameObject.activeInHierarchy)
            CalculateHeight();
    }
}