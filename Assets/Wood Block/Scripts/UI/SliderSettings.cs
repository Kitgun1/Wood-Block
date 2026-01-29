using Kimicu.YandexGames;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSettings : MonoBehaviour
{
    [SerializeField] private bool _isSaving = true;
    [SerializeField, ShowIf("_isSaving")] private string _key;

    public Action<float> OnValueChanged;

    private Slider _slider;

    private void Awake()
    {
        _slider.onValueChanged?.AddListener(ChangeValue);

        if (_isSaving && Cloud.HasKey(_key))
            _slider.value = Cloud.GetValue<float>(_key);
        else
            _slider.value = 1;
    }
    private void OnValidate() => _slider ??= GetComponent<Slider>();

    private void ChangeValue(float value)
    {
        OnValueChanged?.Invoke(value);
        Cloud.SetValue(_key, value);
        Cloud.SaveInCloud();
    }
}
