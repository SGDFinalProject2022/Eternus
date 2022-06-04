using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [SerializeField] GameObject key;
    [SerializeField] Transform spawnParent;
    List<Transform> spawns = new List<Transform>();

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
        //Instantiate(key, spawns[randomLocation]);        
        key.transform.parent = spawns[randomLocation];
        key.transform.localPosition = Vector3.zero;
    }
}
