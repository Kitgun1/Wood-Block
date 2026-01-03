using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WoodBlock
{
    public class GridMap : MonoBehaviour
    {
        [SerializeField] private bool _generateOnAwake;
        [SerializeField] private float _offsetY;

        [SerializeField] private Cell _cellTemplate;
        [SerializeField] private List<LevelMap> _maps = new();

        private readonly List<Cell> _spawnedCells = new();

        private Vector2Int _size;
        private Cell[,] _grid;


        [ReadOnly] public Cell PointerCell;

        public static GridMap Instance { get; private set; }

        /// <summary>
        /// Вызывается при уничтожении определённого количества звёзд(блоков хз)
        /// Но не при загрузке/выгрузке карты или возвращения в истории
        /// </summary>
        public event Action<int> OnDestroyCellInBlocks;

        public int CalculateBlocksCount()
        {
            int count = 0;
            for (int i = 0; i < _spawnedCells.Count; i++)
            {
                var cell = _spawnedCells[i];
                if (!cell.IsEmpty)
                    count++;
            }
            return count;
        }

        private void Awake()
        {
            Instance = this;
            if (_generateOnAwake) GenerateGrid();
        }

        [Button]
        private void GenerateGrid()
        {
            DisposeGrid();

            LevelMap selectedMap = _maps[Random.Range(0, _maps.Count)];

            int minX = selectedMap.GetPositions().Min(v => v.x);
            int maxX = selectedMap.GetPositions().Max(v => v.x);
            int minY = selectedMap.GetPositions().Min(v => v.y);
            int maxY = selectedMap.GetPositions().Max(v => v.y);

            Vector3 startPosition = new(MathF.Ceiling(maxX / 2), MathF.Ceiling(maxY / 2), 0);
            startPosition += new Vector3(0.5f, 0.5f, 0);

            _size = new(maxX - minX + 1, maxY - minY + 1);
            _grid = new Cell[_size.x, _size.y];

            transform.position = new Vector3(0, 0);
            SpawnCells(selectedMap, startPosition, minX, minY);
            transform.position = new Vector3(-maxX, -maxY + _offsetY);
        }

        private void DisposeGrid()
        {
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    Destroy(_grid[x, y].gameObject);
                }
            }
            _spawnedCells.Clear();
            _grid = null;
        }

        private void SpawnCells(LevelMap selectedMap, Vector3 startPosition, int minX, int minY)
        {
            foreach (Vector2Int position in selectedMap.GetPositions())
            {
                Vector3 spawnPosition = (Vector3)(Vector2)position + startPosition;
                Cell spawned = Instantiate(_cellTemplate, spawnPosition, Quaternion.identity, transform);
                _spawnedCells.Add(spawned);
                _grid[position.x - minX, position.y - minY] = spawned;
            }
        }

        private void UpdateGrid(List<Vector2Int> updatedCells)
        {
            for (int i = 0; i < updatedCells.Count; i++)
            {
                var updatedCell = updatedCells[i];

                if (ChechFillY(updatedCell, out int maxY, out int minY))
                {
                    int x = updatedCell.x;
                    for (int y = minY; y <= maxY; y++)
                    {
                        _grid[x, y].RemoveBlock();
                    }
                }

                if (ChechFillX(updatedCell, out int maxX, out int minX))
                {
                    int y = updatedCell.y;
                    for (int x = minX; x <= maxX; x++)
                    {
                        _grid[x, y].RemoveBlock();
                    }
                }
            }
        }

        private bool ChechFillY(Vector2Int point, out int maxY, out int minY)
        {
            int x = point.x;
            maxY = point.y;
            minY = point.y;

            for (int y = maxY; y < _size.y; y++)
            {
                var cell = _grid[x, y];
                if (Cell.NotExist(cell))
                    break;

                if (cell.IsEmpty)
                    return false;

                maxY = y;
            }

            for (int y = minY; 0 <= y; y--)
            {
                var cell = _grid[x, y];
                if (Cell.NotExist(cell))
                    break;

                if (cell.IsEmpty)
                    return false;

                minY = y;
            }

            return true;
        }

        private bool ChechFillX(Vector2Int point, out int maxX, out int minX)
        {
            int y = point.y;
            maxX = point.x;
            minX = point.x;

            for (int x = maxX; x < _size.x; x++)
            {
                var cell = _grid[x, y];
                if (Cell.NotExist(cell))
                    break;

                if (cell.IsEmpty)
                    return false;

                maxX = x;
            }

            for (int x = minX; 0 <= x; x--)
            {
                var cell = _grid[x, y];
                if (Cell.NotExist(cell))
                    break;

                if (cell.IsEmpty)
                    return false;

                minX = x;
            }

            return true;
        }

        private bool CheckPastBlockInCells(DictionaryVector2CellInBlock cellsInBlock, CellInBlock origin)
        {
            Vector2 originPosition = cellsInBlock.FirstOrDefault(pair => pair.Value == origin).Key;

            for (int ySelect = _grid.GetLength(1) - 1; ySelect >= 0; ySelect--)
            {
                for (int xSelect = 0; xSelect < _grid.GetLength(0); xSelect++)
                {
                    if (_grid[xSelect, ySelect] == null) continue;
                    int availableBlock = 0;

                    foreach ((Vector2 pos, CellInBlock cell) in cellsInBlock)
                    {
                        Vector2 offsetOnOrigin = pos - originPosition;
                        int x = xSelect + (int)offsetOnOrigin.x;
                        int y = ySelect + (int)offsetOnOrigin.y;

                        if (x >= _grid.GetLength(0) || x < 0 ||
                            y >= _grid.GetLength(1) || y < 0) continue;
                        if (_grid[x, y] == null || !_grid[x, y].IsEmpty) continue;
                        availableBlock++;
                    }

                    if (availableBlock >= cellsInBlock.Count) return true;
                }
            }

            return false;
        }

        public bool TrySetBlockInCells(DictionaryVector2CellInBlock cellsInBlock, CellInBlock origin)
        {
            Dictionary<Vector2Int, CellInBlock> availableBlock = new();
            List<Vector2Int> updatedCells = new();

            Vector2 originPosition = cellsInBlock.FirstOrDefault(pair => pair.Value == origin).Key;
            if (PointerCell != null)
            {
                Vector2Int positionPointerInMatrix = _grid.GetCellPosition(PointerCell);

                foreach ((Vector2 pos, CellInBlock cell) in cellsInBlock)
                {
                    Vector2 offsetOnOrigin = pos - originPosition;
                    int x = positionPointerInMatrix.x + (int)offsetOnOrigin.x;
                    int y = positionPointerInMatrix.y + (int)offsetOnOrigin.y;

                    if (x >= _grid.GetLength(0) || x < 0 ||
                        y >= _grid.GetLength(1) || y < 0) return false;

                    if (_grid[x, y] != null && _grid[x, y].IsEmpty)
                    {
                        availableBlock.Add(new Vector2Int(x, y), cell);
                        continue;
                    }

                    return false;
                }

                foreach ((Vector2Int pos, CellInBlock cellInBlock) in availableBlock)
                {
                    _grid[pos.x, pos.y].SetBlock(cellInBlock);
                    updatedCells.Add(pos);
                }

                UpdateGrid(updatedCells);

                return true;
            }

            return false;
        }

        public bool TryLoss(Dictionary<DictionaryVector2CellInBlock, CellInBlock> remainingBlocks)
        {
            bool loss = true;
            foreach (var pair in remainingBlocks)
            {
                if (CheckPastBlockInCells(pair.Key, pair.Value))
                {
                    loss = false;
                    break;
                }
            }

            return loss;
        }
    }
}