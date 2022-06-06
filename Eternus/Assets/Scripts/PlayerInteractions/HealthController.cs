using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthController : MonoBehaviour
{
    //use vignette.intensity.value as the health lol
    [Header("PostProcessing")]
    [SerializeField] Volume playerVolume;
    Vignette vignette;
    DepthOfField dof;

    bool isDead = false;
    UI uI;

    // Start is called before the first frame update
    void Start()
    {
        uI = FindObjectOfType<UI>();

        //no clue what this does but it assigns things?
        playerVolume.profile.TryGet(out vignette);
        playerVolume.profile.TryGet(out dof);

        //vignette.intensity.value = 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        if (uI.isPaused || isDead) { return; }

        //testing
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            HurtPlayer();
        }*/
        if (vignette.intensity.value >= 0f) //health regen
        {
            vignette.intensity.value -= Time.deltaTime / 12;
        }

        //adjusting depth of field
        dof.focusDistance.value = Mathf.Lerp(6f, 0.1f, vignette.intensity.value * 1.5f);
    }
    
    public void HurtPlayer()
    {
        vignette.intensity.value += 0.33f;
        if (vignette.intensity.value >= 0.9f)
        {
            isDead = true;
            Debug.Log("Player has died");
        }
    }
    public void HurtPlayer(float damage)
    {
        vignette.intensity.value += damage;
        if (vignette.intensity.value >= 0.9f)
        {
            isDead = true;
            Debug.Log("Player has died");
        }
    }
}
