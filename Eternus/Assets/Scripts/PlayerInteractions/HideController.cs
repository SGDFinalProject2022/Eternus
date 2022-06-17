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
    /*void Update()
    {
        if (exitTrigger.activeSelf)
        { //exit trigger is only active if player is hiding inside
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                GameObject hitCollider = hitColliders[i].gameObject;
                if (hitCollider.CompareTag("Enemy"))
                {
                    audioMan.StopAllCoroutines();
                    audioMan.sounds[3].source.volume = 1f;
                    audioMan.PlayForceEntirely("Tension");
                    Debug.Log("Tension should be playing");
                }
                else
                {
                    audioMan.VolumeFadeOut("Tension", true);
                }
            }
        }
    }*/
}
