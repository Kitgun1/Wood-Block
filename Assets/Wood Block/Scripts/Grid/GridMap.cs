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

        private Cell[,] _cellMatrix;

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
                if (!cell.IsAvailable)
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
            ClearGrid();

            LevelMap selectedMap = _maps[Random.Range(0, _maps.Count)];

            int minX = selectedMap.GetPositions().Min(v => v.x);
            int maxX = selectedMap.GetPositions().Max(v => v.x);
            int minY = selectedMap.GetPositions().Min(v => v.y);
            int maxY = selectedMap.GetPositions().Max(v => v.y);

            Vector3 startPosition = new(MathF.Ceiling(maxX / 2), MathF.Ceiling(maxY / 2), 0);
            startPosition += new Vector3(0.5f, 0.5f, 0);

            _cellMatrix = new Cell[maxX - minX + 1, maxY - minY + 1];

            transform.position = new Vector3(0, 0);
            SpawnCells(selectedMap, startPosition, minX, minY);
            transform.position = new Vector3(-maxX, -maxY + _offsetY);
        }

        /* Перебрать весь массив:
        for (int y = _cellMatrix.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < _cellMatrix.GetLength(0); x++)
            {
                if (_cellMatrix[x, y] == null) continue;
            }
        }
        */

        private void ClearGrid()
        {
            foreach (Cell cell in _spawnedCells)
            {
                Destroy(cell.gameObject);
            }

            _spawnedCells.Clear();
        }

        private void SpawnCells(LevelMap selectedMap, Vector3 startPosition, int minX, int minY)
        {
            foreach (Vector2Int position in selectedMap.GetPositions())
            {
                Vector3 spawnPosition = (Vector3)(Vector2)position + startPosition;
                Cell spawned = Instantiate(_cellTemplate, spawnPosition, Quaternion.identity, transform);
                _spawnedCells.Add(spawned);
                _cellMatrix[position.x - minX, position.y - minY] = spawned;
            }
        }

        List<Cell> filledCells = new(128);
        List<Cell> yFilledCells = new(64);
        List<Cell> xFilledCells = new(64);
        private void UpdateGrid(List<Vector2Int> updatedCells)
        {
            filledCells.Clear();

            foreach (Vector2Int updatedCell in updatedCells)
            {
                bool yLineFilled = true;
                yFilledCells.Clear();

                bool xLineFilled = true;
                xFilledCells.Clear();

                for (int y = updatedCell.y; y < _cellMatrix.GetLength(1); y++)
                {
                    if (_cellMatrix[updatedCell.x, y] == null) 
                        break;

                    if (_cellMatrix[updatedCell.x, y].IsAvailable)
                    {
                        yLineFilled = false;
                        break;
                    }

                    yFilledCells.Add(_cellMatrix[updatedCell.x, y]);
                }

                for (int y = updatedCell.y; y >= 0; y--)
                {
                    if (_cellMatrix[updatedCell.x, y] == null) break;
                    if (_cellMatrix[updatedCell.x, y].IsAvailable)
                    {
                        yLineFilled = false;
                        break;
                    }

                    yFilledCells.Add(_cellMatrix[updatedCell.x, y]);
                }

                for (int x = updatedCell.x; x < _cellMatrix.GetLength(0); x++)
                {
                    if (_cellMatrix[x, updatedCell.y] == null) break;
                    if (_cellMatrix[x, updatedCell.y].IsAvailable)
                    {
                        xLineFilled = false;
                        break;
                    }

                    xFilledCells.Add(_cellMatrix[x, updatedCell.y]);
                }

                for (int x = updatedCell.x; x >= 0; x--)
                {
                    if (_cellMatrix[x, updatedCell.y] == null) break;
                    if (_cellMatrix[x, updatedCell.y].IsAvailable)
                    {
                        xLineFilled = false;
                        break;
                    }

                    xFilledCells.Add(_cellMatrix[x, updatedCell.y]);
                }

                if (yLineFilled) filledCells.AddRange(yFilledCells);
                if (xLineFilled) filledCells.AddRange(xFilledCells);
            }

            int starsDestroyedCount = 0;

            foreach (Cell cell in filledCells)
            {
                if (cell.TryRemoveBlock())
                    starsDestroyedCount++;
            }

            if (starsDestroyedCount > 0 && OnDestroyCellInBlocks != null)
                OnDestroyCellInBlocks.Invoke(starsDestroyedCount);
        }

        private bool CheckPastBlockInCells(DictionaryVector2CellInBlock cellsInBlock, CellInBlock origin)
        {
            Vector2 originPosition = cellsInBlock.FirstOrDefault(pair => pair.Value == origin).Key;

            for (int ySelect = _cellMatrix.GetLength(1) - 1; ySelect >= 0; ySelect--)
            {
                for (int xSelect = 0; xSelect < _cellMatrix.GetLength(0); xSelect++)
                {
                    if (_cellMatrix[xSelect, ySelect] == null) continue;
                    int availableBlock = 0;

                    foreach ((Vector2 pos, CellInBlock cell) in cellsInBlock)
                    {
                        Vector2 offsetOnOrigin = pos - originPosition;
                        int x = xSelect + (int)offsetOnOrigin.x;
                        int y = ySelect + (int)offsetOnOrigin.y;

                        if (x >= _cellMatrix.GetLength(0) || x < 0 ||
                            y >= _cellMatrix.GetLength(1) || y < 0) continue;
                        if (_cellMatrix[x, y] == null || !_cellMatrix[x, y].IsAvailable) continue;
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
                Vector2Int positionPointerInMatrix = _cellMatrix.GetCellPosition(PointerCell);

                foreach ((Vector2 pos, CellInBlock cell) in cellsInBlock)
                {
                    Vector2 offsetOnOrigin = pos - originPosition;
                    int x = positionPointerInMatrix.x + (int)offsetOnOrigin.x;
                    int y = positionPointerInMatrix.y + (int)offsetOnOrigin.y;

                    if (x >= _cellMatrix.GetLength(0) || x < 0 ||
                        y >= _cellMatrix.GetLength(1) || y < 0) return false;

                    if (_cellMatrix[x, y] != null && _cellMatrix[x, y].IsAvailable)
                    {
                        availableBlock.Add(new Vector2Int(x, y), cell);
                        continue;
                    }

                    return false;
                }

                foreach ((Vector2Int pos, CellInBlock cellInBlock) in availableBlock)
                {
                    _cellMatrix[pos.x, pos.y].SetBlock(cellInBlock);
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