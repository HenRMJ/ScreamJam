using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private enum Resolution
    {
        x640y480 = 0,
        x800y600 = 1,
        x1366y768 = 2,
        x1600y900 = 3,
        x1920y1080 = 4,
        x1920y1200 = 5,
        x2560y1440 = 6,
        x2560y1600 = 7,
        x3840y2160 = 8
    }

    private enum Framerate
    {
        noLimit = 0,
        limit30 = 30,
        limit60 = 60,
        limit120 = 120,
        limit240 = 240
    }

    [SerializeField] private Slider masterVolumeSlider, sfxVolumeSlider, musicVolumeSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown, frameRateDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private Resolution currentResolution;
    private Framerate currentFramerate;
    private Dictionary<string, Slider> volumeSliders = new Dictionary<string, Slider>();

    private Vector2Int[] resolutionValue =  {
        new Vector2Int(640, 480),
        new Vector2Int(800, 600),
        new Vector2Int(1366, 768),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(1920, 1200),
        new Vector2Int(2560, 1440),
        new Vector2Int(2560, 1600),
        new Vector2Int(3840, 2160)
    };

    private void Awake()
    {
        volumeSliders.Add("MasterVolume", masterVolumeSlider);
        volumeSliders.Add("SFXVolume", sfxVolumeSlider);
        volumeSliders.Add("MusicVolume", musicVolumeSlider);
    }

    private void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 75f);
        float SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 75f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 75f);
        int fullScreen = PlayerPrefs.GetInt("FullScreen", 1);

        currentResolution = (Resolution)PlayerPrefs.GetInt("Resolution", 4);
        ConvertIntToFramerate(PlayerPrefs.GetInt("Framerate", 2));

        AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume);
        AkSoundEngine.SetRTPCValue("SFXVolume", SFXVolume);
        AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume);

        SetFrameRate();
        SetScreenResolution();
        SetFullScreen(fullScreen);

        masterVolumeSlider.value = masterVolume;
        sfxVolumeSlider.value = SFXVolume;
        musicVolumeSlider.value = musicVolume;
        resolutionDropdown.value = (int)currentResolution;
        frameRateDropdown.value = ConvertFramerateToInt(currentFramerate);
        fullscreenToggle.isOn = fullScreen == 1;
    }

    public void SetVolume(string volumeToSet)
    {
        PlayerPrefs.SetFloat(volumeToSet, volumeSliders[volumeToSet].value);
        AkSoundEngine.SetRTPCValue(volumeToSet, PlayerPrefs.GetFloat(volumeToSet, 75f));
    }

    public void SetFullScreen(Toggle bFullScreen)
    {
        PlayerPrefs.SetInt("FullScreen", bFullScreen.isOn ? 1 : 0);
        Screen.fullScreen = PlayerPrefs.GetInt("FullScreen") == 1;
    }

    private void SetFullScreen(int fullScreenInt)
    {
        PlayerPrefs.SetInt("FullScreen", fullScreenInt);
        Screen.fullScreen = PlayerPrefs.GetInt("FullScreen") == 1;
    }

    private void SetFrameRate()
    {
        Application.targetFrameRate = (int)currentFramerate;
    }

    private void SetScreenResolution()
    {
        Screen.SetResolution(resolutionValue[(int)currentResolution].x, resolutionValue[(int)currentResolution].y, PlayerPrefs.GetInt("FullScreen") == 1);
    }

    public void UpdateResolution(TMP_Dropdown dropdown)
    {
        currentResolution = (Resolution)dropdown.value;
        PlayerPrefs.SetInt("Resolution", (int)currentResolution);
        SetScreenResolution();
    }

    public void UpdateFramerate(TMP_Dropdown dropdown)
    {
        PlayerPrefs.SetInt("Framerate", dropdown.value);

        ConvertIntToFramerate(PlayerPrefs.GetInt("Framerate"));

        SetFrameRate();
    }

    private void ConvertIntToFramerate(int framerateInt)
    {
        switch (framerateInt)
        {
            case 0:
                currentFramerate = Framerate.noLimit;
                break;
            case 1:
                currentFramerate = Framerate.limit30;
                break;
            case 2:
                currentFramerate = Framerate.limit60;
                break;
            case 3:
                currentFramerate = Framerate.limit120;
                break;
            case 4:
                currentFramerate = Framerate.limit240;
                break;
        }
    }

    private int ConvertFramerateToInt(Framerate framerate)
    {
        int intToReturn = 0;

        switch (framerate)
        {
            case Framerate.noLimit:
                intToReturn = 0;
                break;
            case Framerate.limit30:
                intToReturn = 1;
                break;
            case Framerate.limit60:
                intToReturn = 2;
                break;
            case Framerate.limit120:
                intToReturn = 3;
                break;
            case Framerate.limit240:
                intToReturn = 4;
                break;
        }

        return intToReturn;
    }
}
