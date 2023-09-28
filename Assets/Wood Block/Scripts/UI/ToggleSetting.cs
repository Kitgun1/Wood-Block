using KiYandexSDK;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace WoodBlock
{
    [RequireComponent(typeof(Toggle))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Shadow))]
    public class ToggleSetting : MonoBehaviour
    {
        [SerializeField] private bool _isShadow = true;
        [SerializeField] private Color _isOffColor = Color.white;
        [SerializeField, ShowIf("_isShadow")] private Color _isOffColorShadow = Color.white;
        [SerializeField] private Color _isOnColor = Color.white;
        [SerializeField, ShowIf("_isShadow")] private Color _isOnColorShadow = Color.white;

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
                Toggle.isOn = (bool)YandexData.Load(_key, Toggle.isOn);
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
            _image.color = value ? _isOnColor : _isOffColor;
            if (_isShadow) _shadow.effectColor = Toggle.isOn ? _isOnColorShadow : _isOffColorShadow;

            if (!_isSaving) return;
            YandexData.Save(_key, value);
        }

        [Button]
        private void UpdateVisual()
        {
            _image.color = Toggle.isOn ? _isOnColor : _isOffColor;
            if (_isShadow) _shadow.effectColor = Toggle.isOn ? _isOnColorShadow : _isOffColorShadow;
        }
    }
}