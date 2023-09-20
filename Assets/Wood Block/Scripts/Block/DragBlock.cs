using KiUtility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace WoodBlock
{
    public class DragBlock : MonoBehaviour, IBlock, IDraggable, IDroppable
    {
        [SerializeField] private Collider2D _collider;
        
        private Vector3 _position;
        private Vector3 _homePosition;
        private bool _inCell;

        public Transform Transform { get; private set; }

        private void Start()
        {
            Transform = transform;
            _homePosition = Transform.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_inCell) return;
            PlayerInput.PlayerActions.MousePosition.performed += MousePositionChanged;
            PlayerInput.PlayerActions.Touch.canceled += TouchCanceled;
            _collider.enabled = false;
        }

        public void Drop()
        {
            if (Grid.Cell == null || !Grid.Cell.TryInsert(this))
            {
                transform.position = _homePosition;
                return;
            }

            _inCell = true;
        }

        private void MousePositionChanged(InputAction.CallbackContext ctx)
        {
            transform.position = ctx.ReadValue<Vector2>().GetWorldSpace(0.5f);
        }

        private void TouchCanceled(InputAction.CallbackContext ctx)
        {
            PlayerInput.PlayerActions.MousePosition.performed -= MousePositionChanged;
            PlayerInput.PlayerActions.Touch.canceled -= TouchCanceled;
            _collider.enabled = true;
            Drop();
        }
    }
}