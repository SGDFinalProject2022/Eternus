using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Scaled down version of the footstep logic in PlayerMovement meant for Enemy AI
/// </summary>
public class AIFootsteps : MonoBehaviour
{
    [SerializeField] bool automaticFootsteps = true;
    AudioManager audioMan;
    [SerializeField] AudioClip[] footStepSFX;

    public float velocity;
    public bool isAggrod;

    float footstepTimer = 0f;
    [SerializeField] float baseStepSpeed = 0.6f;
    [SerializeField] float aggroStepMultiplier = 0.6f;

    float getCurrentOffset => isAggrod ? baseStepSpeed * aggroStepMultiplier : baseStepSpeed;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<AudioManager>()) 
        { 
            audioMan = GetComponent<AudioManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!automaticFootsteps) { return; }
        FootstepSoundHandler();
    }

    void FootstepSoundHandler()
    {
        if (velocity > 0)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0)
            {
                PlayFootstep();

                footstepTimer = getCurrentOffset;
            }
        }
    }

    public void PlayFootstep()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3))
        {
            switch (hit.collider.tag)
            {
                case "Footsteps/Custom":
                    CustomFootsteps customFootsteps;
                    if (hit.collider.gameObject.GetComponent<CustomFootsteps>() == null)
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
