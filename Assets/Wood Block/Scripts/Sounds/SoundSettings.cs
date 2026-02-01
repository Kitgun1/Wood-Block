using Kimicu.YandexGames;
using Unity.VectorGraphics;
using UnityEngine;
using WoodBlock;

[RequireComponent(typeof(AudioSource))]
public class SoundSettings : MonoBehaviour
{
    [SerializeField] private SliderSettings _slider;

    [Space]
    [Header("Pitch Settings")]
    [SerializeField, Range(0.1f, 1.9f)] private float _minPitch = 0.7f;
    [SerializeField, Range(0.2f, 2)] private float _maxPitch = 1.1f;
    private AudioSource _audioSource;


    private void Start() => Initialize();
    private void OnValidate()
    {
        _audioSource ??= GetComponent<AudioSource>();

        if (_minPitch >= _maxPitch)
            _minPitch = _maxPitch - 0.1f;
    }
    private void OnEnable() { if (_slider is not null) _slider.OnValueChanged += SetVolume; }
    private void OnDisable() { if (_slider is not null) _slider.OnValueChanged -= SetVolume; }


    public void Play()
    {
        float pitch = Random.Range(0.9f,1.1f);
        _audioSource.pitch = pitch;
        if(_audioSource.isActiveAndEnabled)
        _audioSource.Play();
    }
    public void SetVolume(float volume)
    {
        _audioSource.volume = volume;
        _slider.SetValue(volume);
        Cloud.SetValue("soundsVolume", volume);
        Cloud.SaveInCloud();
    }
    private void Initialize()
    {
        if (Cloud.HasKey("soundsVolume"))
            SetVolume(Cloud.GetValue<float>("soundsVolume"));
        else
            SetVolume(1f);
    }
}
