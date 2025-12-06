using Kimicu.YandexGames;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace WoodBlock
{
    [RequireComponent(typeof(Toggle))]
    [RequireComponent(typeof(Image))]
    public class ToggleSetting : MonoBehaviour
    {
        [SerializeField] private Sprite _isOffPicture;
        [SerializeField] private Sprite _isOnPicture;

        [SerializeField] private bool _isSaving = true;
        [SerializeField, ShowIf("_isSaving")] private string _key;

        private Image _image;
        private Shadow _shadow;

        protected Toggle Toggle;

        private void Awake()
        {
            Toggle ??= GetComponent<Toggle>();
            _image ??= GetComponent<Image>();
            _shadow ??= GetComponent<Shadow>();

            if (_isSaving)
            {
                Toggle.isOn = Cloud.GetValue(_key, true);
                UpdateVisual();
            }

            Toggle.onValueChanged.AddListener(ValueChanged);
        }

        private void OnValidate()
        {
            Toggle ??= GetComponent<Toggle>();
            _image ??= GetComponent<Image>();
            _shadow ??= GetComponent<Shadow>();
            UpdateVisual();
        }

        private void ValueChanged(bool value)
        {
            _image.sprite = value ? _isOnPicture : _isOffPicture;

            if (!_isSaving) return;
            Cloud.SetValue(_key, value);
        }

        [Button]
        private void UpdateVisual()
        {
            _image.sprite = Toggle.isOn ? _isOnPicture : _isOffPicture;
        }
    }
}