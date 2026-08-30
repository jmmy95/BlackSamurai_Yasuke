using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;
    public GameObject optionsPanel;

    public void SetMusicVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        // Connect your SFX AudioMixer here later.
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void Back()
    {
        optionsPanel.SetActive(false);
    }
}