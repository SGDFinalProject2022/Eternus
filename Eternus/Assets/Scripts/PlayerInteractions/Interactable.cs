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

    [Header("Door Stuff")]
    public bool isUnlocked = true;
    [SerializeField] AudioClip[] jiggleSFX;

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
        AudioManager audioMan = GetComponent<AudioManager>();
        if(isUnlocked)
        {
            audioMan.PlayForceEntirely("Open"); //when we have an animation for the door, this will play
            FindObjectOfType<UI>().ShowObjective("Find a key: Complete");
            print(this.gameObject.name + " will open (probably using some door open animation)");
            gameObject.layer = 0;
            gameObject.SetActive(false); //remove when there is an animation!!
        }
        else
        {
            audioMan.PlayForceEntirely("Locked", jiggleSFX[Random.Range(0, jiggleSFX.Length - 1)]);
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
