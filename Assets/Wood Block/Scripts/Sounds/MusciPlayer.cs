using Kimicu.YandexGames;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusciPlayer : MonoBehaviour
{
    private AudioSource _audioSource;
    private static MusciPlayer _instance = null;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        if (Cloud.HasKey("musicVolume"))
            _audioSource.volume = Cloud.GetValue<float>("musicVolume");
    }

    public void SetVolume(float volume) => _audioSource.volume = volume;
}
