using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Attached to interactable objects
/// </summary>
public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public int ID;
    public string interactText;
    public bool isUnlocked = true;

    // Start is called before the first frame update
    void Start()
    {
        ID = Random.Range(0, 9999);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DebugPrint(string message)
    {
        Debug.Log(message);
    }

    public void CheckHasKey()
    {
        if(isUnlocked)
        {
            FindObjectOfType<UI>().ShowObjective("Find a key: Complete");
            print(this.gameObject.name + " will open (probably using some door open animation)");
            gameObject.layer = 0;
            gameObject.SetActive(false); //remove when there is an animation
        }
        else
        {
            FindObjectOfType<UI>().ShowObjective("Objective: Find a key");
            interactText = "locked. find a key";
        }
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
        interactText = "open";
    }
}
