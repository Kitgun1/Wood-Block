using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSettings : MonoBehaviour
{
    public Action<float> OnValueChanged;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged?.AddListener(ChangeValue);
    }

    public void SetValue(float value)
    {
        if (_slider == null)
            _slider = GetComponent<Slider>();
        _slider.value = value;
    }
    private void ChangeValue(float value)
    {
        OnValueChanged?.Invoke(value);
    }
}
