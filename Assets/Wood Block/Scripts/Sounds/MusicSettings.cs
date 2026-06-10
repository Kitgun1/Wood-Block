using UnityEngine;
public class MusicSettings : MonoBehaviour
{
    [SerializeField] private SliderSettings _slider;

    private void Start() => Initialize();
    private void OnEnable() { if (_slider is not null) _slider.OnValueChanged += SetVolume; }
    private void OnDisable() { if (_slider is not null) _slider.OnValueChanged -= SetVolume; }

    public void SetVolume(float volume)
    {
        if (MusicPlayer.Instance != null)
            MusicPlayer.Instance.SetVolume(volume);
        _slider.SetValue(volume);

        DataSaver.Save(SaveKeys.MusicVolume, volume);
    }
    private void Initialize()
    {
        SetVolume(DataSaver.Load<float>(SaveKeys.MusicVolume));
    }
}
