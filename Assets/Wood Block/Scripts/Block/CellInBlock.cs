using Kimicu.YandexGames;
using Kimicu.YandexGames.Extension;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WoodBlock
{
    [RequireComponent(typeof(Collider2D))]
    public class CellInBlock : MonoBehaviour, IDraggable
    {
        private Collider2D _collider;

        public Action<CellInBlock> OnClick;
        public Transform Transform { get; private set; }

        private void Awake()
        {
            Transform = transform;
            _collider = GetComponent<Collider2D>();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
            _collider.enabled = false;
        }

        public void EnableCollider()
        {
            _collider.enabled = true;
        }
    }
}