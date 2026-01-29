using Kimicu.YandexGames;
using UnityEngine;

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
    private void OnEnable() => _slider.OnValueChanged += SetVolume;
    private void OnDisable() => _slider.OnValueChanged -= SetVolume;

    public void Play()
    {
        float pitch = Random.Range(0.9f,1.1f);
        _audioSource.pitch = pitch;
        _audioSource.Play();
    }
    public void SetVolume(float volume)
    {
        _audioSource.volume = volume;
        Cloud.SetValue("soundsVolume", volume);
        Cloud.SaveInCloud();
    }
    private void Initialize()
    {
        if (Cloud.HasKey("soundsVolume"))
            SetVolume(Cloud.GetValue<float>("soundsVolume"));
    }
}
