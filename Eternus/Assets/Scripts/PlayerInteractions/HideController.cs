using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideController : MonoBehaviour
{
    [SerializeField] Transform hiddenLocation;
    [SerializeField] Transform exitLocation;
    [SerializeField] GameObject enterTrigger;
    [SerializeField] GameObject exitTrigger;
    [SerializeField] GameObject playerController;
    AudioManager audioMan;

    // Start is called before the first frame update
    void Start()
    {
        //playerController = GameObject.FindGameObjectWithTag("Player");
        audioMan = GetComponent<AudioManager>();
        enterTrigger.SetActive(true);
        exitTrigger.SetActive(false);
    }

    public void Hide()
    {
        playerController.transform.parent = hiddenLocation;
        playerController.GetComponent<PlayerMovement>().isHiding = true;
        playerController.GetComponent<CharacterController>().enabled = false;
        playerController.transform.localPosition = Vector3.zero;
        playerController.transform.localRotation = Quaternion.identity;
        audioMan.Play("Hide");
        audioMan.Play("Hidden");
        enterTrigger.SetActive(false);
        exitTrigger.SetActive(true);
    }

    public void Exit()
    {
        playerController.transform.parent = exitLocation;
        playerController.transform.localPosition = Vector3.zero;
        playerController.transform.localRotation = Quaternion.identity;
        playerController.transform.parent = null;
        playerController.GetComponent<CharacterController>().enabled = true;
        playerController.GetComponent<PlayerMovement>().isHiding = false;
        audioMan.Stop("Hidden");
        audioMan.Play("Exit");
        enterTrigger.SetActive(true);
        exitTrigger.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
