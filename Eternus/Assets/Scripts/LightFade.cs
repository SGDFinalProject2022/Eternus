using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFade : MonoBehaviour
{
    [SerializeField] float speed = 100;
    Light light;
    float lightMax;

    void Start()
    {

        light = GetComponent<Light>();
        StartCoroutine("Fade");
        lightMax = light.intensity;
        light.intensity = 0;
    }

    IEnumerator Fade()
    {
        bool fadeOut = false;
        while (true)
        {
            while (fadeOut)
            {
                light.intensity -= (lightMax/100);
                yield return new WaitForSeconds(1/speed);
                if (light.intensity <= 0)
                {
                    fadeOut = false;
                }
            }

            while(!fadeOut)
            {
                light.intensity += (lightMax/100);
                yield return new WaitForSeconds(1/speed);
                if (light.intensity >= lightMax)
                {
                    fadeOut = true;
                }
            }
        }
       
    }
}
