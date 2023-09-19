using UnityEngine.EventSystems;

namespace Wood_Block.Scripts.DragAndDrop
{
    public interface IDraggable : IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler { }
}