using UnityEngine;
using UnityEngine.EventSystems;

namespace WoodBlock
{
    public class Cell : MonoBehaviour, ICell, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SpriteRenderer _container;

        private IBlock _block;

        public bool TryInsert(IBlock block)
        {
            if (_block != null) return false;

            _block = block;
            _block.Transform.position = transform.position + Vector3.back * 0.1f;
            return true;
        }

        public void Remove()
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Grid.Cell = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Grid.Cell == this) Grid.Cell = null;
        }
    }
}