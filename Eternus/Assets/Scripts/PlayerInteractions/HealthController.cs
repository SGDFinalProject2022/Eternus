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
    [Header("Animator")]
    [SerializeField] Animator camHoldAnimator;
    [Header("Audio")]
    [SerializeField] AudioManager audioMan;

    bool isDead = false;
    UI uI;

    // Start is called before the first frame update
    void Start()
    {
        uI = FindObjectOfType<UI>();

        //no clue what this does but it assigns things?
        playerVolume.profile.TryGet(out vignette);
        playerVolume.profile.TryGet(out dof);

        camHoldAnimator.enabled = false;
        if (audioMan == null) { audioMan = GetComponent<AudioManager>(); }
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
        //Debug.Log("Health: " + (1f - vignette.intensity.value)); //helps to visualize
        audioMan.ChangeVolume("Heartbeat", vignette.intensity.value * 4);
        audioMan.ChangePitch("Heartbeat", 1f + (vignette.intensity.value / 2));
        //Debug.Log(1f + (vignette.intensity.value / 2));

        //adjusting depth of field
        dof.focusDistance.value = Mathf.Lerp(6f, 0.1f, vignette.intensity.value * 1.5f);
    }
    
    public void HurtPlayer()
    {
        audioMan.Play("Hurt");
        vignette.intensity.value += 0.33f;
        if (vignette.intensity.value >= 0.95f)
        {
            Debug.Log("Player has died");           
            FindObjectOfType<PlayerMovement>().isDead = true;
            camHoldAnimator.enabled = true;           
            camHoldAnimator.SetBool("isDead", true);
            isDead = true;
            GlobalData.instance.LoadScene("Death");
        }
    }
    public void HurtPlayer(float damage)
    {
        audioMan.Play("Hurt");
        vignette.intensity.value += damage;
        if (vignette.intensity.value >= 0.95f)
        {
            Debug.Log("Player has died");
            FindObjectOfType<PlayerMovement>().isDead = true;
            camHoldAnimator.enabled = true;
            camHoldAnimator.SetBool("isDead", true);
            isDead = true;
            GlobalData.instance.LoadScene("Death");
        }
    }
}
