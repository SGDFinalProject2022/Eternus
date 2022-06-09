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
    public UnityEvent onUnlock;
    public int ID;
    public string interactText;
    public bool isUnlocked = true;
    public bool canBeUnlocked = true;
    [SerializeField] bool onlyOpenOnce = false;
    [SerializeField] AudioClip[] jiggleSFX;
    [SerializeField] AudioClip[] squeakSFX;
    [SerializeField] Animator doorAnimator;
    [SerializeField] bool final = false;
    public bool isOpen = false;
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
                FindObjectOfType<UI>().ShowObjective("Unlocked"); 
                uI.HideItem();
                wasPreviouslyLocked = false;
                onUnlock.Invoke();
            }

            if (!isOpen)
            {
                if (final)
                {
                    GlobalData.instance.LoadScene("final_cutscene");
                }
                else
                {
                    Animate(true);
                    audioMan.Play("Open");
                    audioMan.PlayOneShot("Squeak", squeakSFX[Random.Range(0, jiggleSFX.Length - 1)]);
                    if (onlyOpenOnce) { gameObject.layer = 0; }
                    isOpen = true;
                }                
            }
            else
            {
                Animate(false);
                audioMan.PlayOneShot("Squeak", squeakSFX[Random.Range(0, jiggleSFX.Length - 1)]);
                isOpen = false;
            }
        }
        else //LOCKED
        {
            audioMan.PlayForceEntirely("Locked", jiggleSFX[Random.Range(0, jiggleSFX.Length - 1)]);
            if (canBeUnlocked) 
            { interactText = "Locked. Find a key"; uI.HideItem(); }            
        }
    }


    //function added to allow peaches to open/close doors - Ven
    public void AIDoor()
    {
        AudioManager audioMan = GetComponent<AudioManager>();
        if (!isOpen)
        {
            Animate(true);
            audioMan.Play("Open");
            audioMan.PlayOneShot("Squeak", squeakSFX[Random.Range(0, jiggleSFX.Length - 1)]);
            if (onlyOpenOnce) { gameObject.layer = 0; }
            isOpen = true;
        }
        else
        {
            Animate(false);
            audioMan.PlayOneShot("Squeak", squeakSFX[Random.Range(0, jiggleSFX.Length - 1)]);
            isOpen = false;
        }
    }


    public void UnlockDoor()
    {
        isUnlocked = true;
        interactText = "Interact";
    }

    public void LockDoor()
    {
        isUnlocked = false;
        if (canBeUnlocked) { interactText = "Locked. Find a key"; }
        else { interactText = "Locked"; }
        isOpen = false;
        Animate(false);
    }

    public void TriggerCutscene()
    {
        CutsceneTrigger.instance.Play();
    }

    void Animate(bool open)
    {
        if(doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", open);
        }
    }
}
