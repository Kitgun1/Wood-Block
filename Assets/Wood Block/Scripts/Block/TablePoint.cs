using System;
using UnityEngine;

namespace WoodBlock
{
    public class TablePoint : MonoBehaviour
    {
        private Block _block;

        public event Action DroppedBlock;

        public bool IsAvailable { get; private set; } = true;

        public void CreateBlock(Block prefab)
        {
            _block = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            _block.Dropped += BlockOnDropped;
            IsAvailable = false;
        }

        private void BlockOnDropped()
        {
            _block = null;
            IsAvailable = true;
            DroppedBlock?.Invoke();
        }
    }
}