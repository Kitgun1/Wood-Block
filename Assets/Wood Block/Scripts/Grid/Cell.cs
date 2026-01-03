using DG.Tweening;
using KimicuUtility;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace WoodBlock
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private CellInBlock _defaultBlockTemplate;

        private CellInBlock _block;
        private KiCoroutine _routine = new();

        public event Action OnDestroyBlock;
        public bool IsEmpty => _block == null;

        public void OnPointerEnter(PointerEventData eventData)
        {
            GridMap.Instance.PointerCell = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (GridMap.Instance.PointerCell == this) GridMap.Instance.PointerCell = null;
        }

        public void SetBlock(CellInBlock block, bool animate = true)
        {
            if (_block != null)
                return;

            _block = block;
            _block.Transform.parent = transform;
            
            if (animate)
                _block.Transform.DOMove(transform.position + Vector3.back * 0.5f, 0.2f);
        }

        public void SetBlock(bool animate = true)
        {
            if (_block == null)
            {
                Vector3 spawnPos = transform.position + (Vector3.up + Vector3.right) / 2;
                _block = Instantiate(_defaultBlockTemplate, spawnPos, Quaternion.identity, transform);
            }

            if (animate)
                _block.Transform.DOMove(transform.position + Vector3.back * 0.5f, 0.2f);
        }

        public void RemoveBlock()
        {
            if (!TryRemoveBlock())
                Debug.LogError("Блок уже удалён.", this);
        }

        public bool TryRemoveBlock(bool animate = true)
        {
            if (_block == null) 
                return false;

            if (animate)
            {
                Rigidbody2D body = _block.gameObject.AddComponent<Rigidbody2D>();
                _block.GetComponent<Collider2D>().isTrigger = true;

                // Animation Destroy
                var spriteRenderer = _block.GetComponent<SpriteRenderer>();
                body.AddForce(Vector2.up * 2 + new Vector2(Random.Range(-1f, -0.5f), 0), ForceMode2D.Impulse);
                //body.DORotate(15, 0.5f);
                //spriteRenderer.DOColor(new Color(1, 1, 1, 0), 3f);

                Destroy(_block.Transform.gameObject, 3f);
            }
            else
            {
                Destroy(_block.gameObject);
            }

            _block = null;
            OnDestroyBlock?.Invoke();
            return true;
        }

        public static bool Exist(Cell cell)
        {
            return cell != null;
        }

        public static bool NotExist(Cell cell)
        {
            return cell == null;
        }
    }
}