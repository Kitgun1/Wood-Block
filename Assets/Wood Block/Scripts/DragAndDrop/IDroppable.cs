using System;

namespace WoodBlock
{
    public interface IDroppable
    {
        public event Action Dropped;
        public void Drop();
    }
}