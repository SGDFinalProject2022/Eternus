using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalData : MonoBehaviour
{
    public static GlobalData instance;

    [HideInInspector] public bool loadSaveData;
    [HideInInspector] public PlayerData player;
    [SerializeField] Animator anim;

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

    void Update()
    {
        //FOR TESTING PURPOSED ONLY. DELETE LATER
        if (Input.GetKeyDown("u"))
        {
            LoadScene("floor_3");
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

    public void LoadScene(string sceneName)
    {
        StartCoroutine(SceneChange(sceneName));
    }

    IEnumerator SceneChange(string sceneName)
    {
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("FadeIn");
    }
}
