using UnityEngine;

namespace WoodBlock
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _container;

        public void Redraw(Sprite sprite)
        {
            _container.sprite = sprite;
        }
    }
}