using Kimicu.YandexGames;
using Kimicu.YandexGames.Extension;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using ReadOnly = NaughtyAttributes.ReadOnlyAttribute;

namespace WoodBlock
{
    public struct GridFill : IEquatable<GridFill>
    {
        public int cord;
        public int min;
        public int max;

        public readonly bool Equals(GridFill other)
        {
            return min == other.min && max == other.max && cord == other.cord;
        }
    }

    public sealed class GridHistory
    {
        public List<Vector2Int> created;
        public Vector2Int[] removed;
        public int points;
    }

    public class GridMap : MonoBehaviour
    {
        [SerializeField] private bool _generateOnAwake;
        [SerializeField] private float _offsetY;

        [SerializeField] private Cell _cellTemplate;
        [SerializeField] private CellInBlock _cellInBlockPrefab;
        [SerializeField] private List<LevelMap> _mapsForMobile = new();

        [SerializeField, Min(1)] private int _scoreMultipier = 10;
        [SerializeField] private QuestManager _questManager;

        public bool IsMultiplierEnabled { get; set; } = false;

        private readonly List<Cell> _spawnedCells = new();

        private Vector2Int _size;
        private Cell[,] _grid;
        private Stack<GridHistory> _history = new();

        [ReadOnly] public Cell PointerCell;

        public static GridMap Instance { get; private set; }

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
        public void GenerateGrid()
        {
            DisposeGrid();

            LevelMap selectedMap = _mapsForMobile[Random.Range(0, _mapsForMobile.Count)];

            int minX = selectedMap.GetPositions().Min(v => v.x);
            int maxX = selectedMap.GetPositions().Max(v => v.x);
            int minY = selectedMap.GetPositions().Min(v => v.y);
            int maxY = selectedMap.GetPositions().Max(v => v.y);

            Vector3 startPosition = new(MathF.Ceiling(maxX / 2), MathF.Ceiling(maxY / 2), 0);
            startPosition += new Vector3(0.5f, -0.5f, 0);

            _size = new(maxX - minX + 1, maxY - minY + 1);
            _grid = new Cell[_size.x, _size.y];

            transform.position = new Vector3(0, 0);
            SpawnCells(selectedMap, startPosition, minX, minY, maxY);
            transform.position = new Vector3(-maxX, -maxY + _offsetY);
        }

        private void DisposeGrid()
        {
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    if (_grid[x, y] != null)
                        Destroy(_grid[x, y].gameObject);
                }
            }
            _spawnedCells.Clear();
            _grid = null;
        }

        private void SpawnCells(LevelMap selectedMap, Vector3 startPosition, int minX, int minY, int maxY)
        {
            var newList = selectedMap.GetPositions().Where(item => item.y <= maxY).ToList();
            foreach (Vector2Int position in newList)
            {
                Vector3 spawnPosition = (Vector3)(Vector2)position + startPosition;
                Cell spawned = Instantiate(_cellTemplate, spawnPosition, Quaternion.identity, transform);

                _spawnedCells.Add(spawned);
                _grid[position.x - minX, position.y - minY] = spawned;
            }
        }

        public void DestroyAllBlocks()
        {
            foreach (var cell in _grid)
                cell?.TryRemoveBlock();
        }


        private readonly HashSet<Vector2Int> s_removed = new(32);
        private readonly HashSet<GridFill> s_vertiacalFills = new(32);
        private readonly HashSet<GridFill> s_horizontalFills = new(32);
        private void CheckFills(List<Vector2Int> created)
        {
            s_removed.Clear();
            s_vertiacalFills.Clear();
            s_horizontalFills.Clear();
            int fillsCount = 0;

            for (int i = 0; i < created.Count; i++)
            {
                var updatedCell = created[i];

                if (ChechFillVertical(updatedCell, out var vertiacalFill))
                {
                    if (!s_vertiacalFills.Contains(vertiacalFill))
                    {
                        s_vertiacalFills.Add(vertiacalFill);
                        fillsCount++;
                    }
                }

                if (ChechFillHorizontal(updatedCell, out var horizontalFill))
                {
                    if (!s_horizontalFills.Contains(horizontalFill))
                    {
                        s_horizontalFills.Add(horizontalFill);
                        fillsCount++;
                    }
                }
            }

            foreach (GridFill fill in s_vertiacalFills)
            {
                int x = fill.cord;
                int minY = fill.min;
                int maxY = fill.max;

                for (int y = minY; y <= maxY; y++)
                {
                    if (s_removed.Contains(new(x, y)))
                        continue;

                    var cell = _grid[x, y];
                    if (cell.TryRemoveBlock())
                        s_removed.Add(new(x, y));
                }
            }

            foreach (GridFill fill in s_horizontalFills)
            {
                int y = fill.cord;
                int minX = fill.min;
                int maxX = fill.max;

                for (int x = minX; x <= maxX; x++)
                {
                    if (s_removed.Contains(new(x, y)))
                        continue;

                    var cell = _grid[x, y];
                    if (cell.TryRemoveBlock())
                        s_removed.Add(new(x, y));
                }
            }

            int score = s_removed.Count * fillsCount;
            _history.Push(new() { created = created, removed = s_removed.ToArray(), points = score });


            if (IsMultiplierEnabled)
            {
                Score.Instance.Value += score * _scoreMultipier;

                if (_questManager != null)
                {
                    var quests = _questManager.GetActiveQuests();
                    foreach (var quest in quests)
                    quest.AddProgress(score * _scoreMultipier);
                }
                else
                {
                    if (DataSaver.Load<int>(SaveKeys.BestScore) < Score.Instance.Value)
                        DataSaver.Save(SaveKeys.BestScore, Score.Instance.Value);
                }
            }
            else
            {
                Score.Instance.Value += score;

                if (_questManager != null)
                {
                    var quests = _questManager.GetActiveQuests();
                    foreach (var quest in quests)
                    quest.AddProgress(score);
                }
                else
                {
                    if (DataSaver.Load<int>(SaveKeys.BestScore) < Score.Instance.Value)
                        DataSaver.Save(SaveKeys.BestScore, Score.Instance.Value);
                }
            }

        }

        public bool CanUndo()
        {
            return _history.Count > 0;
        }

        /// <summary>
        /// Отменяет последний ход и возвращает разницу между количеством блоков
        /// с текущего до прошлого (возвращаемого) хода
        /// </summary>
        /// <returns></returns>
        public void Undo()
        {
            var step = _history.Pop();

            var removedList = step.removed;
            for (int i = 0; i < removedList.Length; i++)
            {
                var removed = removedList[i];

                var block = Instantiate(_cellInBlockPrefab);
                _grid[removed.x, removed.y].SetBlock(block, false);
            }
            Score.Instance.Value -= step.points;

            var createdList = step.created;
            for (int i = 0; i < createdList.Count; i++)
            {
                var created = createdList[i];
                _grid[created.x, created.y].RemoveBlock(false);
            }
        }

        private bool ChechFillVertical(Vector2Int point, out GridFill result)
        {
            result = default;

            int x = point.x;
            int maxY = point.y;
            int minY = point.y;

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

            result.cord = point.x;
            result.min = minY;
            result.max = maxY;

            return true;
        }

        private bool ChechFillHorizontal(Vector2Int point, out GridFill result)
        {
            result = default;

            int y = point.y;
            int maxX = point.x;
            int minX = point.x;

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

            result.cord = point.y;
            result.min = minX;
            result.max = maxX;

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

        private static readonly List<Vector2Int> createdEmpty = new();
        public void UseBomb(Cell position)
        {
            Vector2Int intPos = _grid.GetCellPosition(position);
            int x = intPos.x;
            int y = intPos.y;

            var removed = new NativeList<Vector2Int>(Allocator.Temp);

            TryRemoveBlockAt(x - 1, y);
            TryRemoveBlockAt(x - 1, y + 1);
            TryRemoveBlockAt(x - 1, y - 1);

            TryRemoveBlockAt(x, y);
            TryRemoveBlockAt(x, y + 1);
            TryRemoveBlockAt(x, y - 1);

            TryRemoveBlockAt(x + 1, y);
            TryRemoveBlockAt(x + 1, y + 1);
            TryRemoveBlockAt(x + 1, y - 1);

            _history.Push(new() { created = createdEmpty, removed = removed.ToArray(), points = removed.Length });
            Score.Instance.Value += removed.Length;

            bool TryRemoveBlockAt(int x, int y)
            {
                try
                {
                    if (_grid[x, y].TryRemoveBlock())
                    {
                        removed.Add(new(x, y));
                        return true;
                    }
                }
                catch { }

                return false;

            }
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
                    SetSortingLayer(cellInBlock);
                    SetSpriteSize(cellInBlock.GetComponent<SpriteRenderer>(), 102, 104);
                }

                CheckFills(updatedCells);

                return true;
            }

            return false;
        }
        private void SetSpriteSize(SpriteRenderer spriteRenderer, int targetWidthPixels, int targetHeightPixels)
        {
            if (spriteRenderer.sprite == null) return;

            float originalPixelWidth = spriteRenderer.sprite.rect.width;
            float originalPixelHeight = spriteRenderer.sprite.rect.height;

            float ppu = spriteRenderer.sprite.pixelsPerUnit;

            float originalWorldWidth = originalPixelWidth / ppu;
            float originalWorldHeight = originalPixelHeight / ppu;

            float targetWorldWidth = targetWidthPixels / ppu;
            float targetWorldHeight = targetHeightPixels / ppu;

            float scaleX = targetWorldWidth / originalWorldWidth;
            float scaleY = targetWorldHeight / originalWorldHeight;

            spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
        private void SetSortingLayer(CellInBlock cell)
        {
            cell.GetComponent<SpriteRenderer>().sortingOrder = 1; ;
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