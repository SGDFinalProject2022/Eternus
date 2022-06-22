using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeOverDistance : MonoBehaviour
{
    [SerializeField] AudioManager audio;
    [SerializeField] Transform player;
    [SerializeField] float soundDistance = 30;

    void Start()
    {
        StartCoroutine("StartLoop");
    }

    IEnumerator StartLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(.25f);
            float distance = Vector3.Distance(transform.position, player.position);            
            if (distance <= soundDistance)
            {
                float percent = (1f - (distance / soundDistance));
                audio.ChangeVolume("Steps", percent);
            }
            else
            {
                audio.ChangeVolume("Steps", 0f);
            }
        }
    }

    //GIRL HELP
}
