using TMPro;
using UnityEngine;

namespace WoodBlock
{
    public sealed class LifeDestroyedStars : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private int _destroyedCount = 0;

        public static LifeDestroyedStars Instance { get; private set; }

        private void OnEnable()
        {
            GridMap.Instance.OnDestroyCellInBlocks += OnDestroyCellInBlock;
            if (Instance == null)
                Instance = this;
            else
                Debug.Log("Обнаружен дубликат LifeTime", this);
        }

        private void OnDisable()
        {
            GridMap.Instance.OnDestroyCellInBlocks -= OnDestroyCellInBlock;
            if (Instance == this)
                Instance = null;
        }

        private void OnDestroyCellInBlock(int count)
        {
            _destroyedCount += count;
            _text.text = _destroyedCount.ToString();
        }
    }
}