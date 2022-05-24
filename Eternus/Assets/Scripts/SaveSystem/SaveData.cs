using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SaveData 
{
    public string scene;
    public float x;
    public float y;
    public float z;

    public SaveData(PlayerData data)
    {
        scene = data.scene;
        x = data.position.x;
        y = data.position.y;
        z = data.position.z;
    }
}
