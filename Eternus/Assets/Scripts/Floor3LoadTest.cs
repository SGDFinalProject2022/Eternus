using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor3LoadTest : MonoBehaviour
{

    [SerializeField] GameObject mainArea;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //set main area false
            mainArea.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //set main area true
            mainArea.SetActive(true);
        }
    }
}
