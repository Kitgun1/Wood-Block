using UnityEngine;

namespace WoodBlock
{
    public sealed class CancelLastMove : MonoBehaviour
    {
        public void Cancel()
        {
            var map = GridMap.Instance;
            int oldCount = map.CalculateBlocksCount();
            int newCount = map.CalculateBlocksCount();

            Debug.Log($"OldCount = {oldCount}");
            Debug.Log($"NewCount = {newCount}");
        }
    }
}