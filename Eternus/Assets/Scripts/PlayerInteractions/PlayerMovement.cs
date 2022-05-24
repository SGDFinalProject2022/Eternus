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
    Vector3 velocity;
    bool isOnGround;

    [Header("Crouching")]
    [SerializeField] float crouchHeight = 1f; //base is 3.8
    [SerializeField] float crouchSpeed = 3f;
    bool isCrouching;
    float originalHeight;

    [Header("Sprint")]
    [SerializeField] float sprintSpeed = 12f;
    bool isSprinting;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    [Header("Stealth")]
    public bool isHidden = false;

    [Header("Headbob")]
    [SerializeField] HeadBobController headBobController;
    [SerializeField] float normalHeadBobAmplitude = 0.0005f;
    [SerializeField] float sprintHeadBobAmplitude = 0.001f;

    [Header("Audio")]
    AudioManager audioMan;
    [SerializeField] AudioClip[] footStepSFX; //make multiple arrays if there are more floor materials (probably for water)
    float footstepTimer = 0f;
    float baseStepSpeed = 0.6f;
    float crouchStepMultiplier = 1.5f;
    float sprintStepMultiplier = 0.6f;   
    //no clue how tf this works but i found a tutorial :D
    float getCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : isSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

    // Start is called before the first frame update
    void Start()
    {
        originalHeight = controller.height;
        audioMan = GetComponent<AudioManager>();
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
        if(Input.GetButtonDown("Crouch") && y < 0.5)
        {
            audioMan.Play("Crouch");
            audioMan.Play("Crouch Walk");
        }
        if(Input.GetButtonUp("Crouch") && y > 0.5f)
        {
            audioMan.Play("Stand");
            audioMan.Stop("Crouch Walk");
        }

        audioMan.ChangeVolume("Step", 0.25f);

        //movement + sprint + crouch speed
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float sprint = Input.GetAxis("Sprint");
        float finalSpeed = walkingSpeed;
        if (y > 0) //Crouching
        {
            finalSpeed = Mathf.Lerp(walkingSpeed, crouchSpeed, y);
            audioMan.ChangeVolume("Crouch Walk", Mathf.Lerp(0.0f, 0.1f, Mathf.Abs(x) + Mathf.Abs(z)));
            audioMan.ChangeVolume("Step", 0.1f);
        }
        //can only sprint forward
        if (!isCrouching && sprint > 0 && z > 0 && x == 0) //Sprinting
        {
            finalSpeed = Mathf.Lerp(walkingSpeed, sprintSpeed, sprint);
            headBobController.amplitude = Mathf.Lerp(normalHeadBobAmplitude, sprintHeadBobAmplitude, sprint);
            isSprinting = true;
            audioMan.ChangeVolume("Step", 0.5f);
        }
        else
        {
            //headbobs in any direction
            headBobController.amplitude = Mathf.Lerp(0, normalHeadBobAmplitude, (Mathf.Abs(x) + Mathf.Abs(z)));
            isSprinting = false;
        }
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(finalSpeed * Time.deltaTime * move);

        //gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //footstep SFX
        if(isOnGround && (x != 0 || z != 0))
        {
            footstepTimer -= Time.deltaTime;

            if(footstepTimer <= 0)
            {
                //we don't need this right now but we will later
                /*if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3))
                {

                }*/

                audioMan.ReplaceClip("Step", footStepSFX[Random.Range(0, footStepSFX.Length - 1)]);
                audioMan.Play("Step");

                footstepTimer = getCurrentOffset;
            }
        }
    }
}
