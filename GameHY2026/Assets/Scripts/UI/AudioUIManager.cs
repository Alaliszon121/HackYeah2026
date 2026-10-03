using UnityEngine;
using UnityEngine.UI;

public class AudioUIManager : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // PlayerPrefs keys for saving/loading
    private const string PREF_MAIN = "MainVolume";
    private const string PREF_MUSIC = "MusicVolume";
    private const string PREF_SFX = "SFXVolume";

    private void Start()
    {
        // Load saved values, defaulting to 1f (max volume) if no previous save exists
        float savedMaster = PlayerPrefs.GetFloat(PREF_MAIN, 1f);
        float savedMusic = PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
        float savedSFX = PlayerPrefs.GetFloat(PREF_SFX, 1f);

        // Update the visual UI sliders on launch
        if (mainSlider != null) mainSlider.value = savedMaster;
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        // Apply the loaded values to the mixer to initialize the audio on launch
        SetMainVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        // ADD THESE LINES: Connect the sliders to their respective methods
        if (mainSlider != null) mainSlider.onValueChanged.AddListener(SetMainVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMainVolume(float sliderValue)
    {
        AudioManager.Instance.SetMainVolume(sliderValue);
        PlayerPrefs.SetFloat(PREF_MAIN, sliderValue);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float sliderValue)
    {
        AudioManager.Instance.SetMusicVolume(sliderValue);
        PlayerPrefs.SetFloat(PREF_MUSIC, sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float sliderValue)
    {
        AudioManager.Instance.SetSFXVolume(sliderValue);
        PlayerPrefs.SetFloat(PREF_SFX, sliderValue);
        PlayerPrefs.Save();
    }
}