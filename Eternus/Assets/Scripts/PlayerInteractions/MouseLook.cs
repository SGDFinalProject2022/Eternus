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
    [SerializeField] float mouseSensitivity = 500f;
    [SerializeField] Transform playerBody;
    [Header("Interactions")]
    [SerializeField] LayerMask interactableLayerMask;
    [SerializeField] Image crosshair;

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
        if (uiController.isPaused) { return; }
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

    private void FixedUpdate()
    {
        
    }

    void PlayerInteractions()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 4, interactableLayerMask))
        {
            //Debug.Log(hit.collider.name);

            //makes the crosshair visible
            crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0.5f);
            if (hit.collider.GetComponent<Interactable>()) //interactables
            {
                //making sure it only calls the selected interactable's event
                if (interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID)
                {
                    interactable = hit.collider.GetComponent<Interactable>();
                }
                uiController.interactText.text = interactable.interactText;
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
                uiController.interactText.text = doorController.interactText;
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
            uiController.interactText.text = "";
        }
    }
}
