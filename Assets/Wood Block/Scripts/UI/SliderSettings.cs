using Kimicu.YandexGames;
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
        _slider.onValueChanged?.AddListener(ChangeValue);
    }
    private void OnValidate() => _slider ??= GetComponent<Slider>();

    public void SetValue(float value)
    {
        _slider.value = value;
    }
    private void ChangeValue(float value)
    {
        OnValueChanged?.Invoke(value);
    }
}
