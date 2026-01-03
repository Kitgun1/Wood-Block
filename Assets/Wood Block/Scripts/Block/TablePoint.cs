using System;
using UnityEngine;
using UnityEngine.Events;

namespace WoodBlock
{
    public class TablePoint : MonoBehaviour
    {
        private Block _block;
        public Block prefab;

        public UnityEvent DroppedBlock;
        public event Action<TablePoint> OnBlockDropped;

        public bool IsEmpty { get; private set; } = true;
        public DictionaryVector2CellInBlock CellsInBlock => _block.CellsInBlock;

        private void OnDestroy() => DroppedBlock.RemoveAllListeners();

        public Block CreateBlock(Block prefab)
        {
            _block = Instantiate(prefab, transform.position, Quaternion.identity);
            _block.transform.parent = transform;
            _block.Dropped += BlockOnDropped;
            IsEmpty = false;
            this.prefab = prefab;
            return _block;
        }

        private void BlockOnDropped()
        {
            OnBlockDropped?.Invoke(this);
            Clear();
            DroppedBlock?.Invoke();
        }

        public void Clear()
        {
            _block.Dropped -= BlockOnDropped;
            Destroy(_block.gameObject);

            _block = null;
            prefab = null;
            IsEmpty = true;
        }
    }
}