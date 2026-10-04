using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    
    public static AudioManager Instance;

    [Header("Audio Sources & Mixers")]
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource ambientAudioSource;
    [SerializeField] private AudioSource sfxAudioSource; 
    
    [Header("Common Sound Effects")]
    [SerializeField] private AudioResource buttonClickSound; // Assign your RandomButtonClickSFXGenerator here
    [SerializeField] private AudioResource itemDragSound;
    [SerializeField] private AudioResource itemDropSound;

    private List<AudioSource> sfxPool = new List<AudioSource>();

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
        
        if (sfxAudioSource != null) {
            sfxPool.Add(sfxAudioSource);
        }
    }

    // Dedicated methods for centralized sounds
    public void PlayButtonClick() {
        PlaySFX(buttonClickSound);
    }

    public void PlayItemDrag() {
        PlaySFX(itemDragSound);
    }

    public void PlayItemDrop() {
        PlaySFX(itemDropSound);
    }

    // Core playback logic
    public void PlaySFX(AudioResource audioResource) {
        if (audioResource != null) {
            AudioSource availableSource = GetAvailableSFXSource();
            availableSource.resource = audioResource;
            availableSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip) {
        if (clip != null) {
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    private AudioSource GetAvailableSFXSource() {
        foreach (AudioSource source in sfxPool) {
            if (!source.isPlaying) {
                return source;
            }
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        if (sfxAudioSource != null) {
            newSource.outputAudioMixerGroup = sfxAudioSource.outputAudioMixerGroup;
            newSource.playOnAwake = false;
        }
        
        sfxPool.Add(newSource);
        return newSource;
    }

    // Volume Controls
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