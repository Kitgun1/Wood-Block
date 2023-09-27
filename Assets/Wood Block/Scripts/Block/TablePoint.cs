using UnityEngine;
using UnityEngine.Events;

namespace WoodBlock
{
    public class TablePoint : MonoBehaviour
    {
        private Block _block;

        public UnityEvent DroppedBlock;

        public bool IsAvailable { get; private set; } = true;
        public DictionaryVector2CellInBlock CellsInBlock => _block.CellsInBlock;

        private void OnDestroy() => DroppedBlock.RemoveAllListeners();

        public Block CreateBlock(Block prefab)
        {
            _block = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            _block.Dropped += BlockOnDropped;
            IsAvailable = false;
            return _block;
        }

        private void BlockOnDropped()
        {
            _block.Dropped -= BlockOnDropped;
            Destroy(_block.gameObject);
            _block = null;
            IsAvailable = true;
            DroppedBlock?.Invoke();
        }
    }
}