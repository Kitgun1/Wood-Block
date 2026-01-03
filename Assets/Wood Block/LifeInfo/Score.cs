using TMPro;
using UnityEngine;

namespace WoodBlock
{
    public sealed class Score : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] _labels;

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                UpdateUI();
            }
        }

        public static Score Instance { get; private set; }

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
            Value += count;
        }

        private void UpdateUI()
        {
            for (int i = 0; i < _labels.Length; i++)
                _labels[i].text = Value.ToString();
        }
    }
}