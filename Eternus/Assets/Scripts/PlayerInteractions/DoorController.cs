using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Attached to doors, like interactables but has functions exclusive to doors
/// </summary>
public class DoorController : MonoBehaviour
{
    public UnityEvent onInteract;
    public int ID;
    public string interactText;
    public bool isUnlocked = true;
    public bool canBeUnlocked = true;
    [SerializeField] AudioClip[] jiggleSFX;
    [SerializeField] AudioClip[] squeakSFX;
    [SerializeField] Animator doorAnimator;
    bool isOpen = false;
    UI uI;

    // Start is called before the first frame update
    void Start()
    {
        ID = Random.Range(0, 9999);
        uI = FindObjectOfType<UI>();

        //cleaning up for doors that are locked at the start
        if (!isUnlocked) { wasPreviouslyLocked = true; }
    }

    public void DebugPrint(string message)
    {
        Debug.Log(message);
    }

    bool wasPreviouslyLocked;
    public void CheckHasKey()
    {
        AudioManager audioMan = GetComponent<AudioManager>();
        if (isUnlocked) //UNLOCKED
        {
            if (wasPreviouslyLocked) //if the door was unlocked by a key
            { 
                FindObjectOfType<UI>().ShowObjective("Unlocked door"); 
                uI.HideItem();
                wasPreviouslyLocked = false;
            }

            if (!isOpen)
            {
                doorAnimator.SetBool("isOpen", true);
                audioMan.Play("Open");
                audioMan.PlayOneShot("Squeak", squeakSFX[Random.Range(0, jiggleSFX.Length - 1)]);
                isOpen = true;
            }
            else
            {
                doorAnimator.SetBool("isOpen", false);
                audioMan.PlayOneShot("Squeak", squeakSFX[Random.Range(0, jiggleSFX.Length - 1)]);
                isOpen = false;
            }
            //gameObject.layer = 0;
        }
        else //LOCKED
        {
            audioMan.PlayForceEntirely("Locked", jiggleSFX[Random.Range(0, jiggleSFX.Length - 1)]);
            if (canBeUnlocked) 
            { interactText = "locked. find a key"; uI.HideItem(); }            
        }
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
        interactText = "open";
    }
}
