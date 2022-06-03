using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoorAI : MonoBehaviour
{
    [SerializeField] DoorController doorController;

    //checks if peaches is in range to open the door. ONLY allows this for peaches; other enemies cannot open doors
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && other.gameObject.name == "Peaches")
        {           
            if(doorController.isOpen)
            {
                print("Peaches has walked through the door.");
            }
            else
            {
                print("Peaches has opened the open door.");
                doorController.AIDoor();
            }
        }
    }
    //closes the door after peaches leaves range.
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && other.gameObject.name == "Peaches")
        {
            print("Peaches has closed the door.");
            doorController.AIDoor();
        }
    }
}
