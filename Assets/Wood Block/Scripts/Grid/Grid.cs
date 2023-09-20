using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WoodBlock
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private bool _generateOnAwake;
        
        [SerializeField] private Cell _cellTemplate;
        [SerializeField] private List<LevelMap> _maps = new();

        private readonly List<Cell> _spawnedCells = new();

        private Cell[,] _cellMatrix;

        public static Cell Cell;

        private void Start()
        {
            if(_generateOnAwake) GenerateGrid();
        }

        [Button(nameof(GenerateGrid))]
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

            SpawnCells(selectedMap, startPosition, minX, minY);

            for (int y = _cellMatrix.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < _cellMatrix.GetLength(0); x++)
                {
                    if (_cellMatrix[x, y] == null) continue;
                }
            }
        }

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
                Vector3 spawnPosition = -(Vector3)(Vector2)position + startPosition;
                Cell spawned = Instantiate(_cellTemplate, spawnPosition, Quaternion.identity, transform);
                _spawnedCells.Add(spawned);
                _cellMatrix[position.x - minX, position.y - minY] = spawned;
            }
        }
    }
}