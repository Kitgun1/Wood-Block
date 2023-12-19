using System;
using System.Linq;
using DG.Tweening;
using KimicuUtility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WoodBlock
{
    [RequireComponent(typeof(BlockGenerator))]
    public class Block : MonoBehaviour, IDroppable
    {
        [SerializeField] private float _speedMove = 5;
        [SerializeField] private float _durationScale = 0.5f;

        private DictionaryVector2CellInBlock _blocks;
        private Vector3 _position;
        private Vector3 _offsetPosition;
        private CellInBlock _selectedCell;
        private bool _inCell;
        private Vector3 _homePosition;
        private float _lenghtX = 0.2f;
        private float _lenghtY = 0.2f;
        private Vector3 _targetPosition;

        private float _currentScale = 1;
        private const float ScaleX1 = 1.3f;
        private const float ScaleX2 = 1.1f;
        private const float ScaleX3 = 0.7f;
        private const float ScaleX4 = 0.5f;

        public DictionaryVector2CellInBlock CellsInBlock => _blocks;

        public event Action Dropped;

        private void Awake()
        {
            _homePosition = transform.position;
            _targetPosition = _homePosition;

            _blocks = GetComponent<BlockGenerator>().GetCells();
            _lenghtX = _blocks.Max(pair => pair.Key.x) - _blocks.Min(pair => pair.Key.x) + 1;
            _lenghtY = _blocks.Max(pair => pair.Key.y) - _blocks.Min(pair => pair.Key.y) + 1;
            float lenght = MathF.Max(_lenghtX, _lenghtY);

            _currentScale = lenght switch
            {
                1 => ScaleX1,
                2 => ScaleX2,
                3 => ScaleX3,
                4 => ScaleX4,
                _ => _currentScale
            };
            transform.DOScale(Vector3.one * _currentScale, 0);

            foreach ((Vector2 _, CellInBlock block) in _blocks)
            {
                block.OnClick += OnClick;
            }
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, _targetPosition);
            float step = _speedMove * 10 * Time.deltaTime * (distance / 2);

            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);
        }

        private void OnDestroy()
        {
            foreach ((Vector2 _, CellInBlock block) in _blocks)
            {
                block.OnClick -= OnClick;
            }
        }

        private void OnClick(CellInBlock cell)
        {
            if (_inCell) return;
            _selectedCell = cell;
            _offsetPosition = cell.transform.position - transform.position;
            transform.DOScale(Vector3.one, _durationScale / 2)
                .OnComplete(() => _offsetPosition = cell.transform.position - transform.position);
            PlayerInput.PlayerActions.MousePosition.performed += MousePositionChanged;
            PlayerInput.PlayerActions.Touch.canceled += TouchCanceled;
        }

        private void MousePositionChanged(InputAction.CallbackContext ctx)
        {
            _targetPosition = ctx.ReadValue<Vector2>().GetWorldSpace(0.5f) - _offsetPosition;
        }

        private void TouchCanceled(InputAction.CallbackContext ctx)
        {
            PlayerInput.PlayerActions.MousePosition.performed -= MousePositionChanged;
            PlayerInput.PlayerActions.Touch.canceled -= TouchCanceled;
            Drop();
        }

        public void Drop()
        {
            if (GridMap.Instance.TrySetBlockInCells(_blocks, _selectedCell))
            {
                _inCell = true;
                Dropped?.Invoke();
            }
            else
            {
                _targetPosition = _homePosition;

                foreach (var pair in _blocks)
                {
                    pair.Value.EnableCollider();
                }

                transform.DOScale(Vector3.one * _currentScale, _durationScale);
            }
        }
    }
}