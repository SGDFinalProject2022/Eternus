using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] bool isMainMenu = false;
    [Header("Audio")]
    [SerializeField] AudioListener audioListener;
    AudioManager[] audioMen;
    [Header("Sliders")]
    [SerializeField] Slider overallVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [Header("Fullscreen")]
    [SerializeField] Text fullscreenText;
    [SerializeField] string checkmark; //cheating hehe
    bool isFullscreen = false;

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();

        audioMen = FindObjectsOfType<AudioManager>();
    }

    public void OnOverallVolumeChange()
    {
        AudioListener.volume = overallVolumeSlider.value;
        GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }
    public void OnSFXVolumeChange()
    {        
        foreach (AudioManager audioMan in audioMen)
        {
            audioMan.ChangeVolumeOfType(soundType.SFX, sfxVolumeSlider.value);
        }
        if(!isMainMenu)
        {
            FindObjectOfType<PlayerMovement>().footstepBaseVolume =
            Mathf.Lerp(0, 0.25f, sfxVolumeSlider.value); //hhhhhhh
        }
        GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }
    public void OnMusicVolumeChange()
    {
        foreach (AudioManager audioMan in audioMen)
        {
            audioMan.ChangeVolumeOfType(soundType.music, musicVolumeSlider.value);
        }
        GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }

    public void LoadSettings()
    {
        SettingsData data = SaveLoad.LoadSettings();
        if(data != null)
        {
            isFullscreen = data.fullscreen;
            musicVolumeSlider.value = data.bgmVolume;
            sfxVolumeSlider.value = data.sfxVolume;
        }
    }
    public void FullscreenToggle()
    {
        if(!isFullscreen)
        {
            Debug.Log("fullscreen on");
            fullscreenText.text = checkmark;
            Screen.fullScreen = true;
            isFullscreen = true;
        }
        else
        {
            Debug.Log("fullscreen off");
            fullscreenText.text = "X";
            Screen.fullScreen = false;
            isFullscreen = false;
        }
        GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }
}
