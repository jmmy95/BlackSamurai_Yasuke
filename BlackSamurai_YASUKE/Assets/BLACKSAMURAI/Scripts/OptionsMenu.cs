using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

    public GameObject optionsPanel;
    public GameObject mainMenuPanel;

    private void Start()
    {
        LoadSettings();
    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= 0)
            volume = 0.0001f;

        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= 0)
            volume = 0.0001f;

        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;

        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        fullscreenToggle.isOn = fullscreen;

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetFullscreen(fullscreen);
    }
}