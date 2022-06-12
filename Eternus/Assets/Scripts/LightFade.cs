using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFade : MonoBehaviour
{
    Light light;
    float lightMax;

    void Start()
    {
        light = GetComponent<Light>();
        StartCoroutine("Fade");
        lightMax = light.intensity;
    }

    IEnumerator Fade()
    {
        bool fadeOut = true;
        while (true)
        {
            while (fadeOut)
            {
                light.intensity -= .1f;
                yield return new WaitForSeconds(.1f);
                if (light.intensity <= 0)
                {
                    fadeOut = false;
                }
            }

            while(!fadeOut)
            {
                light.intensity += .1f;
                yield return new WaitForSeconds(.1f);
                if (light.intensity >= lightMax)
                {
                    fadeOut = true;
                }
            }
        }
       
    }
}
