using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Saved the game");
            GlobalData.instance.SaveData(other.GetComponent<PlayerData>());
            gameObject.SetActive(false);
        }
    }
}
