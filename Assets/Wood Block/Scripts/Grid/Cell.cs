using UnityEngine;
using UnityEngine.EventSystems;

namespace WoodBlock
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SpriteRenderer _container;

        private CellInBlock _block;

        public bool IsAvailable => _block == null;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Grid.Instance.PointerCell = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Grid.Instance.PointerCell == this) Grid.Instance.PointerCell = null;
        }

        public void SetBlock(CellInBlock block)
        {
            _block = block;
            _block.Transform.position = transform.position + Vector3.back * 0.5f;
        }
    }
}