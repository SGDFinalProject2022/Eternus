using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    [SerializeField] List<GameObject> lights = new List<GameObject>();

    void Awake()
    {
        foreach (GameObject light in lights)
        {
            light.GetComponent<LightFade>().enabled = false;
            light.GetComponent<Light>().enabled = false;
        }
    }

    public void ActivateLights()
    {
        foreach(GameObject light in lights)
        {
            light.GetComponent<LightFade>().enabled = true;
            light.GetComponent<Light>().enabled = true;
        }
    }
}
