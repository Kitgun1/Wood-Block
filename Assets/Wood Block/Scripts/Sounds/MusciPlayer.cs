using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class MusciPlayer : MonoBehaviour
{
    private AudioSource _audioSource;
    private static MusciPlayer _instance = null;

    public void Initialize()
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

        _audioSource.Play();
    }
    public void SetVolume(float volume)
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = volume;
    }
}
