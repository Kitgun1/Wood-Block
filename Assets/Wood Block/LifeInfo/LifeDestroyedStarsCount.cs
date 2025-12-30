using TMPro;
using UnityEngine;

namespace WoodBlock
{
    public sealed class LifeDestroyedStars : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] _labels;
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
            for (int i = 0; i < _labels.Length; i++)
            {
                _labels[i].text = _destroyedCount.ToString();
            }
        }
    }
}