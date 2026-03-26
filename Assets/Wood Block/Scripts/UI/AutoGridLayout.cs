using UnityEngine;
using UnityEngine.UI;

public class AutoGridLayout : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridGroup;
    [SerializeField] private float minElementWidth = 200f; // минимальная ширина элемента
    [SerializeField] private int mobileMaxColumns = 4;

    [Header("Grid Settings")]
    [SerializeField] private Vector2 spacing = new Vector2(10f, 10f);
    [SerializeField] private RectOffset padding;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (gridGroup == null)
            gridGroup = GetComponent<GridLayoutGroup>();

        // Инициализируем отступы, если они не заданы
        if (padding == null)
            padding = new RectOffset(10, 10, 10, 10);

        RecalculateLayout();
    }

    void Update()
    {
        // Проверяем изменение размера
        if (Screen.width != rectTransform.rect.width)
        {
            RecalculateLayout();
        }
    }

    void RecalculateLayout()
    {
        float availableWidth = rectTransform.rect.width;

        // Применяем настройки отступов
        gridGroup.spacing = spacing;
        if (padding != null)
        {
            gridGroup.padding.top = padding.top;
            gridGroup.padding.bottom = padding.bottom;
            gridGroup.padding.left = padding.left;
            gridGroup.padding.right = padding.right;
        }

        float spacingX = gridGroup.spacing.x;
        float paddingTotal = gridGroup.padding.left + gridGroup.padding.right;

        // Вычисляем доступное пространство для элементов
        float spaceForElements = availableWidth - paddingTotal;

        // Определяем, мобильное ли устройство
        bool isMobile = Screen.width <= 768;

        int maxColumns;

        if (isMobile)
        {
            // На мобильных фиксированное количество - 4
            maxColumns = Mathf.Min(mobileMaxColumns, 4);
        }
        else
        {
            // На ПК рассчитываем максимальное количество
            maxColumns = Mathf.FloorToInt((spaceForElements + spacingX) / (minElementWidth + spacingX));
            maxColumns = Mathf.Max(1, maxColumns); // минимум 1 колонка
        }

        // Применяем настройки
        gridGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridGroup.constraintCount = maxColumns;

        // Рассчитываем размер ячейки (всегда квадратная)
        float cellSize = (spaceForElements - (maxColumns - 1) * spacingX) / maxColumns;
        gridGroup.cellSize = new Vector2(cellSize, cellSize);
    }

    // Метод для принудительного пересчета
    public void ForceRecalculate()
    {
        RecalculateLayout();
    }

    // Метод для обновления минимальной ширины элемента
    public void SetMinElementWidth(float newWidth)
    {
        minElementWidth = newWidth;
        RecalculateLayout();
    }
}
