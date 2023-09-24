using System;
using KiUtility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WoodBlock
{
    [RequireComponent(typeof(BlockGenerator))]
    public class Block : MonoBehaviour, IDroppable
    {
        private DictionaryVector2CellInBlock _blocks;
        private Vector3 _position;
        private Vector3 _offsetPosition;
        private CellInBlock _selectedCell;
        private bool _inCell;
        private Vector3 _homePosition;

        public event Action Dropped;
        
        private void Start()
        {
            _blocks = GetComponent<BlockGenerator>().GetCells();
            _homePosition = transform.position;

            foreach ((Vector2 _, CellInBlock block) in _blocks)
            {
                block.OnClick += OnClick;
            }
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
            PlayerInput.PlayerActions.MousePosition.performed += MousePositionChanged;
            PlayerInput.PlayerActions.Touch.canceled += TouchCanceled;
        }

        private void MousePositionChanged(InputAction.CallbackContext ctx)
        {
            transform.position = ctx.ReadValue<Vector2>().GetWorldSpace(0.5f) - _offsetPosition;
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
                transform.position = _homePosition;
                foreach (var pair in _blocks)
                {
                    pair.Value.EnableCollider();
                }
            }
        }
    }
}