using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Main player movement
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Character Controller")]
    [SerializeField] CharacterController controller;

    [Header("Player Parameters")]
    [SerializeField] float walkingSpeed = 8f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1f;
    [Header("Crouching")]
    [SerializeField] float crouchHeight = 1f; //base is 3.8
    [SerializeField] float crouchSpeed = 3f;
    [Header("Sprint")]
    [SerializeField] float sprintSpeed = 12f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    Vector3 velocity;
    bool isOnGround;
    bool isCrouching;
    float originalHeight;

    // Start is called before the first frame update
    void Start()
    {
        originalHeight = controller.height;
    }

    // Update is called once per frame
    void Update()
    {
        //checks if it's on the ground
        isOnGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isOnGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }       

        //jump
        if(Input.GetButtonDown("Jump") && isOnGround && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);           
        }

        //crouch
        float y = Input.GetAxis("Crouch");
        controller.height = Mathf.Lerp(originalHeight, crouchHeight, y);
        groundCheck.localPosition = new Vector3(0, Mathf.Lerp(-1.8f, -crouchHeight/2, y), 0);
        if (y > 0)
        {
            isCrouching = true;
            groundCheck.localPosition += new Vector3(groundCheck.localPosition.x, groundCheck.localPosition.y + (crouchHeight / 2), groundCheck.localPosition.z);
        }
        if (y == 0)
        {
            isCrouching = false;
        }

        //movement + sprint + crouch
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float sprint = Input.GetAxis("Sprint");
        float finalSpeed = walkingSpeed;
        if (y > 0)
        {
            finalSpeed = Mathf.Lerp(walkingSpeed, crouchSpeed, y);
        }
        //can only sprint forward
        if (!isCrouching && sprint > 0 && z > 0 && x == 0)
        {
            finalSpeed = Mathf.Lerp(walkingSpeed, sprintSpeed, sprint);
        }
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * finalSpeed * Time.deltaTime);

        //gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
