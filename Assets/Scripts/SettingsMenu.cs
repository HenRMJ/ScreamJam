using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider, sfxVolumeSlider, musicVolumeSlider;

    private void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 75f);
        float SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 75f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 75f);

        AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume);
        AkSoundEngine.SetRTPCValue("SFXVolume", SFXVolume);
        AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume);

        masterVolumeSlider.value = masterVolume;
        sfxVolumeSlider.value = SFXVolume;
        musicVolumeSlider.value = musicVolume;
    }

    public void SetVolume(string whatVolume)
    {
        Slider slider = null;

        switch (whatVolume)
        {
            case "MasterVolume":
                slider = masterVolumeSlider;
                break;
            case "SFXVolume":
                slider = sfxVolumeSlider;
                break;
            case "MusicVolume":
                slider = musicVolumeSlider;
                break;
        }

        PlayerPrefs.SetFloat(whatVolume, slider.value);
        AkSoundEngine.SetRTPCValue(whatVolume, PlayerPrefs.GetFloat(whatVolume, 75f));
    }

    public void SetFullScreen(bool bFullScreen)
    {
        PlayerPrefs.SetInt("FullScreen", bFullScreen ? 1 : 0);
        Screen.fullScreen = bFullScreen;
    }
}
