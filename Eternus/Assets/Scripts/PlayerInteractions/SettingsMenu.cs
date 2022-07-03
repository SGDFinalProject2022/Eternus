using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
/// <summary>
/// Manages changing the settings, saving/loading settings
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] bool isMainMenu = false;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject brightnessPanel;
    [Header("Audio")]
    [SerializeField] AudioListener audioListener;
    AudioManager[] audioMen;
    [Header("Sliders")]
    [SerializeField] Slider overallVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider gammaSlider;
    [Header("Fullscreen")]
    [SerializeField] Text fullscreenText;
    [SerializeField] string checkmark; //cheating hehe
    [Header("Misc.")]   
    [SerializeField] MouseLook mouseLook;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Volume playerVolume;

    bool isFullscreen = false;
    LiftGammaGain liftGammaGain;


    // Start is called before the first frame update
    void Start()
    {
        _ = playerVolume.profile.TryGet(out liftGammaGain);
        LoadSettings();
        settingsPanel.SetActive(false);
        brightnessPanel.SetActive(false);
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
        if (!isMainMenu)
        {
            playerMovement.footstepBaseVolume =
            Mathf.Lerp(0, 0.25f, sfxVolumeSlider.value); //hhhhhhh
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        //GlobalData.instance.SaveSettings(musicVolumeSlider.value, sfxVolumeSlider.value, isFullscreen);
    }
    public void OnMusicVolumeChange()
    {
        audioMen = FindObjectsOfType<AudioManager>();
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
            mouseLook.mouseSensitivity = Mathf.Lerp(5f, 500f, sensitivitySlider.value);
        }
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
    }
    public void OnGammaChange()
    {
        if (liftGammaGain == null) return;
        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, gammaSlider.value));
        PlayerPrefs.SetFloat("Gamma", gammaSlider.value);
        //Debug.LogWarning(gammaSlider.value);
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
        Debug.Log("Loading Settings...");
        if (PlayerPrefs.HasKey("MainVolume") == false)
        { ResetSettings(); Debug.LogWarning("player doesn't have playerprefs, resetting..."); }
        else
        {
            overallVolumeSlider.value = PlayerPrefs.GetFloat("MainVolume");
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        

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
        //Fullscreen Toggle
        switch (PlayerPrefs.GetInt("Fullscreen"))
        {
            case 0:
                Screen.fullScreen = false;
                //Debug.Log("fullscreen off");
                if (fullscreenText != null) fullscreenText.text = "X";
                isFullscreen = false; break;
            case 1: 
                Screen.fullScreen = true;
                //Debug.Log("fullscreen on");
                if (fullscreenText != null) fullscreenText.text = checkmark;
                isFullscreen = true; break;
            default: Debug.LogError("PlayerPrefs ''Fullscreen'' is nonbinary"); break;
        }
        //do something similar with content toggle?

        //Brightness Slider
        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, PlayerPrefs.GetFloat("Gamma")));
        gammaSlider.value = PlayerPrefs.GetFloat("Gamma");
    }
    public void ResetSettings()
    {
        overallVolumeSlider.value = 1f;
        sfxVolumeSlider.value = 1f;
        musicVolumeSlider.value = 1f;
        sensitivitySlider.value = 0.75f;
        gammaSlider.value = 0f;
        PlayerPrefs.SetFloat("MainVolume", overallVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
        PlayerPrefs.SetFloat("Gamma", gammaSlider.value);
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
