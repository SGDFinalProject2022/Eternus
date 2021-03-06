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
    [HideInInspector] public float speed;
    Vector3 lastPos;
    bool isOnGround;
    bool isJumping;
    public bool isDead = false;
    float finalSpeed;

    [Header("Crouching")]
    [SerializeField] float crouchHeight = 1f; //base is 3.8
    [SerializeField] float crouchSpeed = 3f;
    [HideInInspector] public bool isCrouching;
    bool isUnderSomething;
    float originalHeight;
    bool crouchSticky; //toggles on when window unfocuses

    [Header("Sprint")]
    [SerializeField] float sprintSpeed = 12f;
    [HideInInspector] public bool isSprinting;

    [Header("Water")]
    [SerializeField] float waterSpeed = 3f;
    [HideInInspector] public bool isInWater;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    [Header("Stealth")]
    public bool isHidden = false;
    public bool isHiding = false;

    [Header("Headbob")]
    [SerializeField] HeadBobController headBobController;
    [SerializeField] float normalHeadBobAmplitude = 0.0005f;
    [SerializeField] float sprintHeadBobAmplitude = 0.001f;
    [SerializeField] float normalHeadBobFrequency = 10f;
    [SerializeField] float sprintHeadBobFrequency = 15f;

    [Header("Audio")]
    AudioManager audioMan;
    [SerializeField] AudioClip[] footStepSFX; //make multiple arrays if there are more floor materials (probably for water)
    [SerializeField] AudioClip[] waterStepSFX;

    //Footsteps
    float footstepTimer = 0f;
    float baseStepSpeed = 0.6f;
    float crouchStepMultiplier = 1.5f;
    float sprintStepMultiplier = 0.6f;
    float waterStepMultiplier = 2f;
    //no clue how tf this works but i found a tutorial :D
    float getCurrentOffset => 
        isInWater ? baseStepSpeed * waterStepMultiplier 
        : isCrouching ? baseStepSpeed * crouchStepMultiplier 
        : isSprinting ? baseStepSpeed * sprintStepMultiplier 
        : baseStepSpeed;

    [SerializeField] UI uiController;

    //multiply everything that changes volume based on this value
    [HideInInspector] public float footstepBaseVolume = 0.25f;

    

    // Start is called before the first frame update
    void Start()
    {
        originalHeight = controller.height;
        audioMan = GetComponent<AudioManager>();
        //uiController = FindObjectOfType<UI>();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && isCrouching) { crouchSticky = true; }
    }

    // Update is called once per frame
    void Update()
    {      
        //forces everthing to a halt while the player is hiding or paused
        if (isHiding || uiController.isPaused || isDead)
        {
            headBobController.amplitude = 0f;
            headBobController.frequency = 0f;
            audioMan.Stop("Crouch Walk");
            return; 
        }

        //checks if it's on the ground or on water
        isOnGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3))
        {
            isInWater = hit.collider.CompareTag("Footsteps/Water");
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Crouch");
        float z = Input.GetAxis("Vertical");

        if (crouchSticky)
        {
            if (Input.GetAxis("Crouch") > 0) { crouchSticky = false; return; }
            y = 1f;
        }

        //preventing player from getting stuck when under objects while crouching
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit headHit, originalHeight / 1.5f)
            && isCrouching)
        {
            y = 1f;
            isUnderSomething = true;
            audioMan.Stop("Crouch Walk");
        }
        else { isUnderSomething = false; }

        JumpLandHandler();
        CrouchHandler(y);
        MovementHandler(x, y, z);
        GravityHandler();
        FootstepSoundHandler(x, z);

        speed = Vector3.Distance(lastPos, transform.position) / Time.deltaTime;
        lastPos = transform.position;
    }

    /// <summary>
    /// Handles Jumping and Landing
    /// </summary>
    void JumpLandHandler()
    {
        //landing
        if (isOnGround && velocity.y < 0)
        {
            velocity.y = -2f;
            if (isJumping)
            {
                PlayFootstep();
                isJumping = false;
            }
        }

        //jumping
        if (Input.GetButtonDown("Jump") && isOnGround && !isSprinting && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            if (!isInWater) { audioMan.Play("Jump"); }
            else { audioMan.PlayOneShot("Jump", waterStepSFX[Random.Range(0, waterStepSFX.Length - 1)]); }
            isJumping = true;
        }
    }

    /// <summary>
    /// Changes PlayerController's height, Sets isCrouching, Plays crouching SFX
    /// </summary>
    /// <param name="y"></param>
    void CrouchHandler(float y)
    {        
        //preventing crouch jumping
        if (!isOnGround && !isCrouching)
        {
            controller.height = originalHeight;
            groundCheck.localPosition = new Vector3(0, -1.8f, 0);
            
            if(!isInWater)
            {
                return;
            }
            
        }
        controller.height = Mathf.Lerp(originalHeight, crouchHeight, y);
        groundCheck.localPosition = new Vector3(0, Mathf.Lerp(-1.8f, -crouchHeight / 2, y), 0);

        if(isInWater)
        {
            audioMan.Stop("Crouch Walk");
            return;
        }

        //sets is crouching and moves groundCheck accordingly
        if (y > 0)
        {
            isCrouching = true;
            groundCheck.localPosition += new Vector3(groundCheck.localPosition.x, groundCheck.localPosition.y + (crouchHeight / 2), groundCheck.localPosition.z);
        }
        if (y == 0)
        {
            isCrouching = false;
        }

        //sfx
        if (Input.GetButtonDown("Crouch") && y < 0.5)
        {
            audioMan.Play("Crouch");
            audioMan.Play("Crouch Walk");
        }
        if (Input.GetButtonUp("Crouch") && y > 0.5f && !isUnderSomething)
        {
            audioMan.Play("Stand");
            audioMan.Stop("Crouch Walk");
        }
    }

    /// <summary>
    /// Movement + Sprint + Crouch Speed, Changes footsteps volume + Headbob
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    void MovementHandler(float x, float y, float z)
    {
        //setting footstep volume to default
        audioMan.ChangeVolume("Step", footstepBaseVolume);
        float sprint = Input.GetAxis("Sprint");
        finalSpeed = walkingSpeed;
        if (isCrouching && !isInWater)
        {
            finalSpeed = Mathf.Lerp(walkingSpeed, crouchSpeed, y);
            audioMan.ChangeVolume("Crouch Walk", Mathf.Lerp(0.0f, footstepBaseVolume * 0.4f, Mathf.Abs(x) + Mathf.Abs(z)));
            audioMan.ChangeVolume("Step", footstepBaseVolume * 0.4f);
        }
        //can only sprint forward
        if (!isCrouching && sprint > 0 && z > 0 && x == 0 && !isInWater) //Sprinting
        {
            finalSpeed = Mathf.Lerp(walkingSpeed, sprintSpeed, sprint);
            headBobController.amplitude = Mathf.Lerp(normalHeadBobAmplitude, sprintHeadBobAmplitude, sprint);
            headBobController.frequency = sprintHeadBobFrequency;
            isSprinting = true;
            audioMan.ChangeVolume("Step", footstepBaseVolume * 2f);
        }
        else
        {
            //headbobs in any direction
            headBobController.amplitude = Mathf.Lerp(0, normalHeadBobAmplitude, (Mathf.Abs(x) + Mathf.Abs(z)));
            headBobController.frequency = normalHeadBobFrequency;
            isSprinting = false;
        }
        if(isInWater) //separate things when the player is in water
        {
            isCrouching = false;
            headBobController.frequency = 5f;
            if(sprint > 0)
            {
                finalSpeed = Mathf.Lerp(waterSpeed, waterSpeed * 1.5f, sprint);
                isSprinting = true;
            }
            else
            {
                finalSpeed = waterSpeed;
                isSprinting = false;
            }            
        }
        Vector3 move = transform.right * x + transform.forward * z;     
        if(move.magnitude >= 1f) { move = move.normalized; }       
        controller.Move(finalSpeed * Time.deltaTime * move);
    }

    void GravityHandler()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Calls the PlayFootstep function if on the ground and footstepTimer is 0
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    void FootstepSoundHandler(float x, float z)
    {
        //if the player is on the ground, pressing a movement key
        if (isOnGround && (x != 0 || z != 0))
        {
            //helps when player is running into a wall
            footstepTimer -= Time.deltaTime * (speed / finalSpeed);
            if (footstepTimer <= 0)
            {
                PlayFootstep();

                footstepTimer = getCurrentOffset;
            }
        }
    }
    /// <summary>
    /// Plays a footstep sound based on the tag of the ground
    /// </summary>
    void PlayFootstep()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3))
        {
            switch (hit.collider.tag)
            {
                case "Footsteps/Pavement":
                    audioMan.PlayOneShot("Step", footStepSFX[Random.Range(0, footStepSFX.Length - 1)]);
                    break;
                case "Footsteps/Water":
                    audioMan.PlayOneShot("Step", waterStepSFX[Random.Range(0, waterStepSFX.Length - 1)]);
                    break;
                case "Footsteps/Custom":
                    CustomFootsteps customFootsteps;
                    if(hit.collider.gameObject.GetComponent<CustomFootsteps>() == null)
                    {
                        Debug.LogWarning(hit.collider.gameObject.name + " does not have CustomFootsteps attached!");
                        audioMan.PlayOneShot("Step", footStepSFX[Random.Range(0, footStepSFX.Length - 1)]);
                        break;
                    }
                    customFootsteps = hit.collider.gameObject.GetComponent<CustomFootsteps>();                  
                    audioMan.PlayOneShot("Step", customFootsteps.footsteps[Random.Range(0, customFootsteps.footsteps.Length - 1)]);
                    break;
                default:
                    audioMan.PlayOneShot("Step", footStepSFX[Random.Range(0, footStepSFX.Length - 1)]);
                    break;
            }

        }
    }
}
