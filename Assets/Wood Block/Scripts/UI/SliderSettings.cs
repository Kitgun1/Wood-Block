using Kimicu.YandexGames;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSettings : MonoBehaviour
{
    [SerializeField,Range(0,1)] private float _startValue;

    [SerializeField] private bool _isSaving = true;
    [SerializeField, ShowIf("_isSaving")] private string _key;

    public Action<float> OnValueChanged;

    private Slider _slider;

    private void Awake()
    {
        _slider ??= GetComponent<Slider>();
        _slider.onValueChanged.AddListener(ChangeValue);

        if (_isSaving)
            _slider.value = Cloud.GetValue<float>(_key);
    }
    private void OnValidate()
    {
        _slider ??= GetComponent<Slider>();
        _slider.value = _startValue;
    }


    private void ChangeValue(float value)
    {
        OnValueChanged?.Invoke(value);
        Cloud.SetValue(_key, value);
        Cloud.SaveInCloud();
    }
}
