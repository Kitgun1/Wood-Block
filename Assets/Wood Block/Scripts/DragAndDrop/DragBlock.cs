using System;
using KiUtility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wood_Block.Scripts.DragAndDrop
{
    public class DragBlock : MonoBehaviour, IDraggable
    {
        private bool _pressed = false;
        private Vector3 _position;

        private void Update()
        {
            if (!_pressed) return;
            transform.position = _position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _position = eventData.position.GetWorldSpace();
        }
    }
}