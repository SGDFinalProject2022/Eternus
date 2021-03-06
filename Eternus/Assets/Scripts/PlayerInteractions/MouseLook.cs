using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rotates the player camera according to mouse input
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("Player Parameters")]
    [Range(50f, 500f)] public float mouseSensitivity = 500f;
    [SerializeField] Transform playerBody;
    [Header("Interactions")]
    [SerializeField] LayerMask interactableLayerMask;
    [SerializeField] Image crosshair;
    [SerializeField] Sprite defaultCrosshair;
    [SerializeField] Sprite interactCrosshair;
    [HideInInspector] public bool isInCutscene = false;

    UI uiController;

    float xRotation = 0f;
    float yRotation = 0f;
    Interactable interactable;
    DoorController doorController;
    PlayerMovement playerMovement;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        uiController = GetComponent<UI>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiController.isPaused || playerMovement.isDead || isInCutscene) { return; }
        //mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        yRotation += mouseX;

        if (!playerMovement.isHiding) //Normal
        {           
            xRotation = Mathf.Clamp(xRotation, -70, 70);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else //Hiding
        {
            xRotation = Mathf.Clamp(xRotation, -20, 30);
            yRotation = Mathf.Clamp(yRotation, -30, 30);
            playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //playerBody.Rotate(Vector3.up * yRotation);

        //interactions
        PlayerInteractions();
    }

    public void StartIntroCutscene()
    {
        Debug.Log("Intro cutscene starting");
        isInCutscene = true;
        playerMovement.isHiding = true;
        transform.parent.rotation =
            new Quaternion(transform.parent.rotation.x, -transform.parent.rotation.y,
            transform.parent.rotation.z, transform.parent.rotation.w);
        Animator camHoldAnimator = GetComponent<Animator>();
        camHoldAnimator.enabled = true;
        camHoldAnimator.SetBool("isIntro", true);
        crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0f);
    }
    public void StopIntroCutscene()
    {
        Animator camHoldAnimator = GetComponent<Animator>();
        camHoldAnimator.SetBool("isIntro", false);
        camHoldAnimator.enabled = false;
        transform.rotation =
            new Quaternion(transform.parent.rotation.x, -transform.parent.rotation.y,
            transform.parent.rotation.z, transform.parent.rotation.w);
        playerMovement.isHiding = false;
        isInCutscene = false;
    }

    public void StartCutscene(float time) => StartCoroutine(StartCutsceneCoroutine(time));
    IEnumerator StartCutsceneCoroutine(float time) //time in secs
    {
        Debug.Log("Pausing player for cutscene, length: " + time);
        isInCutscene = true;
        playerMovement.isHiding = true;
        crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0f);
        yield return new WaitForSeconds(time);
        Debug.Log("Cutscene ended");
        isInCutscene = false;
        playerMovement.isHiding = false;
    }

    void PlayerInteractions()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 4, interactableLayerMask))
        {
            //Debug.Log(hit.collider.name);

            //makes the crosshair visible
            crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0.5f);
            crosshair.sprite = interactCrosshair;
            crosshair.transform.localScale = new Vector3(7, 7, 7);

            if (hit.collider.GetComponent<Interactable>()) //interactables
            {
                //making sure it only calls the selected interactable's event
                if (interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID)
                {
                    interactable = hit.collider.GetComponent<Interactable>();
                }
                //uiController.interactText.text = interactable.interactText;
                if (Input.GetButtonDown("Interact"))
                {
                    interactable.onInteract.Invoke();
                }
            }

            if (hit.collider.GetComponent<DoorController>()) //doors
            {
                //making sure it only calls the selected interactable's event
                if (doorController == null || doorController.ID != hit.collider.GetComponent<DoorController>().ID)
                {
                    doorController = hit.collider.GetComponent<DoorController>();
                }
                //uiController.interactText.text = doorController.interactText;
                if (Input.GetButtonDown("Interact"))
                {
                    doorController.onInteract.Invoke();
                }
            }
        }
        else
        {
            //makes the crosshair invisible
            crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0.15f);
            crosshair.sprite = defaultCrosshair;
            crosshair.transform.localScale = new Vector3(1, 1, 1);
            //uiController.interactText.text = "";
        }
    }
}
