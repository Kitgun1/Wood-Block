using System;
using System.Collections.Generic;
using KiUtility;
using UnityEngine;
using UnityEngine.EventSystems;
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
        
        public Vector3 HomePosition { get; private set; }

        private void Start()
        {
            _blocks = GetComponent<BlockGenerator>().GetCells();
            HomePosition = transform.position;

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
            }
            else
            {
                transform.position = HomePosition;
                foreach (var pair in _blocks)
                {
                    pair.Value.EnableCollider();
                }
            }
        }
    }
}