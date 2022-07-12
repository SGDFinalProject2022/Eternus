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
    Coroutine loadCor = null;

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

    public void LoadScene(string sceneName)
    {
        if(loadCor == null)
        {
            loadCor = StartCoroutine(SceneChange(sceneName));
        }
    }

    IEnumerator SceneChange(string sceneName)
    {
        print("Called");
        anim.SetTrigger("FadeOut");
        Time.timeScale = 1f;
        yield return new WaitForSeconds(2f);
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("FadeIn");
        loadCor = null;
    }
}
