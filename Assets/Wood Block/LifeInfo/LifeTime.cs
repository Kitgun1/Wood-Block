using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;

namespace WoodBlock
{
    public sealed class LifeTime : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] _labels;

        private float _seconds;
        public float Value => _seconds;

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
            _seconds += Time.deltaTime;
            for (int i = 0; i < _labels.Length; i++)
                _labels[i].text = GetText();
        }
        public void Clear() => _seconds = 0;

        public string GetText()
        {
            int seconds = (int)_seconds;
            int minutes = 0;
            int hours = 0;

            if (seconds > 60)
            {
                minutes = seconds / 60;
                seconds -= minutes * 60;
            }

            if (minutes > 60)
            {
                hours = minutes / 60;
                minutes -= hours * 60;
            }

            string text = string.Empty;

            if (hours != 0)
                text += $"{hours} ч. ";

            if (minutes != 0)
                text += $"{minutes} м. ";

            return text + $"{seconds} с.";
        }
    }
}