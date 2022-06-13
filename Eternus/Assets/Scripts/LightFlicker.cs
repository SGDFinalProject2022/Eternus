using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] Light[] lights;
    [SerializeField] AudioSource buzzSFX;
    [SerializeField] float maxDelay = 0.2f;
    bool isFlickering = false;
    float timeDelay;

    private void Awake()
    {
        if (lights == null) { lights = GetComponents<Light>(); }
        if (buzzSFX == null) { buzzSFX = gameObject.AddComponent<AudioSource>(); }
    }

    void Update()
    {
        if(!isFlickering)
        {
            StartCoroutine(Flicker());
        }
    }

    IEnumerator Flicker()
    {
        isFlickering = true;
        foreach (Light light in lights) { light.enabled = false; }     
        buzzSFX.volume = 0f;
        timeDelay = Random.Range(0.01f, maxDelay);
        yield return new WaitForSeconds(timeDelay);
        foreach (Light light in lights) { light.enabled = true; }
        buzzSFX.volume = 1f;
        timeDelay = Random.Range(0.01f, maxDelay);
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false;
    }
}
