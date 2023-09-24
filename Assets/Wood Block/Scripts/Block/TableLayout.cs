using System;
using System.Collections.Generic;
using System.Linq;
using KiUtility;
using NaughtyAttributes;
using UnityEngine;

namespace WoodBlock
{
    public class TableLayout : MonoBehaviour
    {
        [SerializeField] private float _zOffset;
        [SerializeField] private int _blocksInHotBar = 3;
        [SerializeField] private Bounds _bounds;
        [SerializeField] private Transform _point1;
        [SerializeField] private Transform _point2;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [ReadOnly, SerializeField] private List<TablePoint> _tablePoints = new();

        private TableBounds _tableBounds;

        public int AmountPoints
        {
            get => _blocksInHotBar;
            set
            {
                _blocksInHotBar = value;
                GenerateTablePoints();
            }
        }

        public IEnumerable<TablePoint> Points => _tablePoints;

        private void Update()
        {
            CalculateBounds();
        }

        private void OnValidate()
        {
            if (_blocksInHotBar != _tablePoints.Count)
            {
                GenerateTablePoints();
            }
        }

        [Button]
        private void CalculateBounds()
        {
            _tableBounds ??= new TableBounds(Camera.main);

            Bounds bounds = _tableBounds.CalculateCameraBoundsPixels(
                -_bounds.YMax,
                -_bounds.XMax,
                -_bounds.YMin,
                -_bounds.XMin);
            _point1.position = new Vector3(bounds.XMin, bounds.YMin);
            _point2.position = new Vector3(bounds.XMax, bounds.YMax);

            CalculateSprite();
        }

        [Button]
        private void GenerateTablePoints()
        {
            foreach (TablePoint tablePoint in _tablePoints.Where(tablePoint => tablePoint == null))
            {
                _tablePoints.Remove(tablePoint);
            }

            if (_tablePoints.Count > _blocksInHotBar)
            {
                for (int i = 0; i < _tablePoints.Count - _blocksInHotBar; i++)
                {
                    if (_tablePoints[i] != null) Destroy(_tablePoints[i].gameObject);
                }
            }
            else if (_tablePoints.Count < _blocksInHotBar)
            {
                for (int i = 0; i < _blocksInHotBar - _tablePoints.Count; i++)
                {
                    TablePoint point = new GameObject($"Table Point").AddComponent<TablePoint>();
                    point.transform.parent = transform;
                    _tablePoints.Add(point);
                }
            }

            UpdateContainerPoints();
        }

        [Button]
        private void UpdateContainerPoints()
        {
            Vector3 position1 = _point1.position;
            Vector3 position2 = _point2.position;
            float height = position1.y - position2.y;
            float width = position1.x - position2.x;

            float partWidth = width / _blocksInHotBar;

            for (int i = 0; i < _tablePoints.Count; i++)
            {
                _tablePoints[i].transform.position = new Vector3(_point1.position.x - (partWidth * i) - partWidth / 2,
                    _point1.position.y - (height / 2), 0);
            }
        }

        private void CalculateSprite()
        {
            Vector2 size = _point2.position - _point1.position;
            Vector3 position = _point1.position + (_point2.position - _point1.position) / 2;
            position.z = _zOffset;

            _spriteRenderer.size = new Vector2(size.x, size.y);

            _spriteRenderer.transform.position = position;
        }
    }
}