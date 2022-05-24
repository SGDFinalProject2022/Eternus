using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    [HideInInspector] public string scene;
    [HideInInspector] public Vector3 position;

    void Start()
    {
        GlobalData.instance.player = this;
        //Checks if player loaded a save file
        if (GlobalData.instance.loadSaveData)
        {
            SaveData data = SaveLoad.Load();
            transform.position = new Vector3(data.x, data.y, data.z);
        }
    }
}
