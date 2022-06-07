using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject playPanel;
    [SerializeField] Button continueButton;

    bool isSettingsToggled = false;
    //bool isPlayToggled = false;
    //things for saving and loading player progress
    //things for saving and loading player settings

    //eventually, there might be a function to open a "Play" panel that can access continue/new game

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        if (SaveLoad.Load() != null)
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayToggle()
    { 

    }

    public void OnContinue()
    {
        SaveData data = SaveLoad.Load();
        GlobalData.instance.LoadScene(data.scene);
    }


    public void LoadLevel(string levelName)
    {
        GlobalData.instance.LoadScene(levelName);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
        print("Quit");
    }

    public void SettingsToggle()
    {
        if(isSettingsToggled)
        {
            settingsPanel.SetActive(false);
            isSettingsToggled = false;
        }
        else
        {
            settingsPanel.SetActive(true);
            isSettingsToggled = true;
        }
    }
}
