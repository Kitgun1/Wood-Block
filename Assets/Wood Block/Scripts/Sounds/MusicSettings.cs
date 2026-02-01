using Kimicu.YandexGames;
using UnityEngine;
public class MusicSettings : MonoBehaviour
{
    [SerializeField] private SliderSettings _slider;
    private MusciPlayer _musicPlayer;

    private void Start() => Initialize();
    private void OnEnable() { if(_slider is not null)_slider.OnValueChanged += SetVolume; }
    private void OnDisable() { if (_slider is not null) _slider.OnValueChanged -= SetVolume; }

    public void SetVolume(float volume)
    {
        _musicPlayer.SetVolume(volume);
        _slider.SetValue(volume);
        Cloud.SetValue("musicVolume", volume);
        Cloud.SaveInCloud();
    }
    private void Initialize()
    {
        _musicPlayer = FindFirstObjectByType<MusciPlayer>();

        if (Cloud.HasKey("musicVolume"))
            SetVolume(Cloud.GetValue<float>("musicVolume"));
        else
        {
            SetVolume(1);
            Cloud.SetValue("musicVolume", 1);
            Cloud.SaveInCloud();
        }
    }
}
