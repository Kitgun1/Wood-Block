using System.Collections.Generic;
using UnityEngine;

namespace WoodBlock
{
    [CreateAssetMenu(fileName = "Map", menuName = "WoodenBlock", order = 0)]
    public class LevelMap : ScriptableObject
    {
        [SerializeField] private List<Vector2Int> _cells = new();

        public void AddCells(IEnumerable<Vector2Int> cells)
        {
            _cells.Clear();
            _cells.AddRange(cells);
        }

        public IEnumerable<Vector2Int> GetPositions() => _cells;
    }
}