using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates the player camera according to mouse input
/// </summary>
public class MouseLook : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 500f;
    [SerializeField] Transform playerBody;
    [SerializeField] LayerMask interactableLayerMask;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2, interactableLayerMask))
        {
            Debug.Log(hit.collider.name);

        }
    }
}
