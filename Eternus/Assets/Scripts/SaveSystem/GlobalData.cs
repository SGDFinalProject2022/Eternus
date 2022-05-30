using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalData : MonoBehaviour
{
    public static GlobalData instance;

    [HideInInspector] public bool loadSaveData;
    [HideInInspector] public PlayerData player;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }        
        
    }

    //Saves player data passed in through player GO
    public void SaveData(PlayerData data)
    {
        data.scene = SceneManager.GetActiveScene().name;
        data.position = data.gameObject.transform.position;
        SaveLoad.Save(data);
    }
    
    //Loads scene saved in player data
    public void LoadData()
    {
        loadSaveData = true;
        SaveData data = SaveLoad.Load();
        SceneManager.LoadScene(data.scene);    
    } 

    public void SaveSettings(float bgm, float sfx, bool isFull)
    {
        SettingsData data = new SettingsData(bgm, sfx, isFull);
        SaveLoad.SaveSettings(data);
    }
}
