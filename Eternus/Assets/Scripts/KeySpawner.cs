using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [SerializeField] GameObject key;
    [SerializeField] Transform spawnParent;
    List<Transform> spawns = new List<Transform>();
    [SerializeField] List<Material> menuTextures = new List<Material>();
    [SerializeField] GameObject menuPanel;

    void Start()
    {
        SetUpSpawns();
        Spawn();
    }

    void SetUpSpawns()
    {
        foreach(Transform child in spawnParent)
        {
            spawns.Add(child);
        }
    }

    void Spawn()
    {
        int randomLocation = Random.Range(0, spawns.Count);
        for(int i = 0; i < menuTextures.Count; i++)
        {
            if(i == randomLocation)
            {
                print("assigning texture " + menuTextures[i]);
                menuPanel.GetComponent<Renderer>().material = menuTextures[i];
            }
        }
        //Instantiate(key, spawns[randomLocation]);        
        key.transform.parent = spawns[randomLocation];
        key.transform.localPosition = Vector3.zero;
    }
}
