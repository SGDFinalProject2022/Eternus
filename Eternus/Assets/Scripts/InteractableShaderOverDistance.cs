using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableShaderOverDistance : MonoBehaviour
{
    public GameObject interactableShader;
    [SerializeField] float distance;
    [SerializeField] Transform player;
    bool hasBeenAssigned;

    bool isActive;

    void Update()
    {
        UpdateInteractable();
        if(interactableShader != null)
        {
            UpdateActive();
        }
    }

    void UpdateInteractable()
    {
        if(interactableShader != null && !hasBeenAssigned)
        {
            hasBeenAssigned = true;
            isActive = false;
            interactableShader.SetActive(false);
        }
    }

    void UpdateActive()
    {
        if(!isActive)
        {
            if(Vector3.Distance(player.position, transform.position) <= distance)
            {
                isActive = true;
                interactableShader.SetActive(true);
            }
        }
        else
        {
            if (Vector3.Distance(player.position, transform.position) >= distance)
            {
                isActive = false;
                interactableShader.SetActive(false);
            }
        }
    }
}
