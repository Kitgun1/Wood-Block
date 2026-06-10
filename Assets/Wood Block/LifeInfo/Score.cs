using Newtonsoft.Json.Linq;
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
            if (Instance == null)
                Instance = this;
            else
                Debug.Log("Обнаружен дубликат Score", this);
        }

        private void OnDisable()
        {

            if (Instance == this)
                Instance = null;
        }
        public void Clear() { _value = 0; UpdateUI(); }
        private void UpdateUI()
        {
            for (int i = 0; i < _labels.Length; i++)
                _labels[i].text = Value.ToString();
        }
    }
}