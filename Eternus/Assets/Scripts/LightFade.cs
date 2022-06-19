using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFade : MonoBehaviour
{
    [SerializeField] float speed = 100;
    Light _light;
    float lightMax;

    void Start()
    {

        _light = GetComponent<Light>();
        StartCoroutine("Fade");
        lightMax = _light.intensity;
        _light.intensity = 0;
    }

    IEnumerator Fade()
    {
        bool fadeOut = false;
        while (true)
        {
            while (fadeOut)
            {
                _light.intensity -= (lightMax/100);
                yield return new WaitForSeconds(1/speed);
                if (_light.intensity <= 0)
                {
                    fadeOut = false;
                }
            }

            while(!fadeOut)
            {
                _light.intensity += (lightMax/100);
                yield return new WaitForSeconds(1/speed);
                if (_light.intensity >= lightMax)
                {
                    fadeOut = true;
                }
            }
        }
       
    }
}
