using TMPro;
using UnityEngine;

namespace WoodBlock
{
    public sealed class LifeTime : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private float _value;
        public float Value => _value;

        public static LifeTime Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Debug.Log("Обнаружен дубликат LifeTime", this);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Update()
        {
            _value += Time.deltaTime;
            _text.text = _value.ToString("0");
        }
    }
}