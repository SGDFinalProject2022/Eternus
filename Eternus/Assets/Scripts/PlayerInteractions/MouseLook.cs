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
    Interactable interactable;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        uiController = GetComponent<UI>();
    }

    // Update is called once per frame
    void Update()
    {
        //mouse movement
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        //interactions
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 4, interactableLayerMask))
        {
            //Debug.Log(hit.collider.name);

            //makes the crosshair visible
            crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0.5f);
            if (hit.collider.GetComponent<Interactable>()) //just in case
            {
                //making sure it only calls the selected interactable's event
                if(interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID)
                {
                    interactable = hit.collider.GetComponent<Interactable>();
                }
                uiController.interactText.text = interactable.interactText;
                if(Input.GetButtonDown("Interact"))
                {
                    interactable.onInteract.Invoke();
                }
            }
        }
        else
        {
            //makes the crosshair invisible
            crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0f);
            uiController.interactText.text = "";
        }
    }
}
