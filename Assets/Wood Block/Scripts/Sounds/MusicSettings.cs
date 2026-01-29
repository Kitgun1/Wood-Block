using Kimicu.YandexGames;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicSettings : MonoBehaviour
{
    [SerializeField] private SliderSettings _slider;

    private AudioSource _musicSource;
    private static MusicSettings _instance = null;

    private void Start()
    {
        if (_instance == null)
            _instance = this; 
        else if (_instance == this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        Initialize();
        PlayMusic();
    }
    private void OnValidate() => _musicSource ??= GetComponent<AudioSource>();
    private void OnEnable() { if(_slider is not null)_slider.OnValueChanged += SetVolume; }
    private void OnDisable() { if (_slider is not null) _slider.OnValueChanged -= SetVolume; }

    public void SetVolume(float volume)
    {
        _musicSource.volume = volume;
        Cloud.SetValue("musicVolume", volume);
        Cloud.SaveInCloud();
    }
    private void Initialize()
    {

        if (Cloud.HasKey("musicVolume"))
            SetVolume(Cloud.GetValue<float>("musicVolume"));
        else
        {
            SetVolume(1);
            Cloud.SetValue("musicVolume", 1);
            Cloud.SaveInCloud();
        }
    }
    private void PlayMusic() => _musicSource.Play();
}
