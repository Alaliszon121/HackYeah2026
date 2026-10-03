using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    
    public static AudioManager Instance;

    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    
    private const string MAIN_VOL_PARAM = "MainVolume";
    private const string MUSIC_VOL_PARAM = "MusicVolume";
    private const string SFX_VOL_PARAM = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void PlayButtonSound() {
        sfxAudioSource.Play();
    }

    public void SetMainVolume(float sliderValue) {
        float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mainAudioMixer.SetFloat(MAIN_VOL_PARAM, dbValue);
    }
    
    public void SetMusicVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mainAudioMixer.SetFloat(MUSIC_VOL_PARAM, dbValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mainAudioMixer.SetFloat(SFX_VOL_PARAM, dbValue);
    }
}
