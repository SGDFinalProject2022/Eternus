using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Manages changing the settings, saving/loading settings
/// </summary>
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
    [SerializeField] Slider sensitivitySlider;
    [Header("Fullscreen")]
    [SerializeField] Text fullscreenText;
    [SerializeField] string checkmark; //cheating hehe
    [Header("Misc.")]   
    [SerializeField] MouseLook mouseLook;
    [SerializeField] PlayerMovement playerMovement;

    bool isFullscreen = false;


    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();
        /*AudioManager[] audioManagers = FindObjectsOfType<AudioManager>();
        audioMen = audioManagers;*/
    }

    public void OnOverallVolumeChange()
    {
        AudioListener.volume = overallVolumeSlider.value;
        PlayerPrefs.SetFloat("MainVolume", overallVolumeSlider.value);
        //GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }
    public void OnSFXVolumeChange()
    {
        audioMen = FindObjectsOfType<AudioManager>();
        foreach (AudioManager audioMan in audioMen)
        {
            audioMan.ChangeVolumeOfType(soundType.SFX, sfxVolumeSlider.value);
        }
        if(!isMainMenu)
        {
            playerMovement.footstepBaseVolume =
            Mathf.Lerp(0, 0.25f, sfxVolumeSlider.value); //hhhhhhh
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        //GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }
    public void OnMusicVolumeChange()
    {
        foreach (AudioManager audioMan in audioMen)
        {
            audioMan.ChangeVolumeOfType(soundType.music, musicVolumeSlider.value);
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        //GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }
    public void OnSensitivityChange()
    {
        if (mouseLook != null)
        {
            mouseLook.mouseSensitivity = Mathf.Lerp(50f, 500f, sensitivitySlider.value);
        }
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
    }

    /*public void LoadSettings()
    {
        SettingsData data = SaveLoad.LoadSettings();
        if(data != null)
        {
            isFullscreen = data.fullscreen;
            musicVolumeSlider.value = data.bgmVolume;
            sfxVolumeSlider.value = data.sfxVolume;
        }
    }*/

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MainVolume") == false)
        { /*SaveSettings();*/ Debug.Log("player doesn't have playerprefs"); }
        overallVolumeSlider.value = PlayerPrefs.GetFloat("MainVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");

        //Main Volume
        AudioListener.volume = PlayerPrefs.GetFloat("MainVolume");
        //SFX Volume
        audioMen = FindObjectsOfType<AudioManager>();
        foreach (AudioManager audioMan in audioMen)
        {
            audioMan.ChangeVolumeOfType(soundType.SFX, PlayerPrefs.GetFloat("SFXVolume"));
        }
        if (!isMainMenu)
        {
            playerMovement.footstepBaseVolume =
            Mathf.Lerp(0, 0.25f, PlayerPrefs.GetFloat("SFXVolume")); //hhhhhhh
        }
        //Music Volume
        foreach (AudioManager audioMan in audioMen)
        {
            audioMan.ChangeVolumeOfType(soundType.music, PlayerPrefs.GetFloat("MusicVolume"));
        }
        //Mouse Sensitivity
        if (mouseLook != null)
        {
            mouseLook.mouseSensitivity = Mathf.Lerp(50f, 500f, PlayerPrefs.GetFloat("MouseSensitivity"));
        }
    }
    void SaveSettings()
    {
        PlayerPrefs.SetFloat("MainVolume", overallVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
    }

    public void FullscreenToggle()
    {
        if(!isFullscreen)
        {
            Debug.Log("fullscreen on");
            fullscreenText.text = checkmark;
            Screen.fullScreen = true;
            isFullscreen = true;
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            Debug.Log("fullscreen off");
            fullscreenText.text = "X";
            Screen.fullScreen = false;
            isFullscreen = false;
            PlayerPrefs.SetInt("Fullscreen", 0);
        }        
        //GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }
    public void ContentToggle()
    {

    }
}
