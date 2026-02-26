using Kimicu.YandexGames;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusciPlayer : MonoBehaviour
{
    private AudioSource _audioSource;
    private static MusciPlayer _instance = null;

    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = DataSaver.Load<float>(SaveKeys.MusicVolume);
        WebApplication.InAdvertChangeState += Play;
    }
    private void Play(bool value)
    {
        if(value == false)
        _audioSource.Play();
    }
    public void SetVolume(float volume) 
    {
        if(_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = volume; 
    }
}
