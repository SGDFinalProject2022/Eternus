using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState { Patrol, Yield, Chase, SoundAggro};
public class NewEnemyAI : MonoBehaviour
{
    [SerializeField] string enemyName;
    [Header("Patrol")] //Patrol variables
    [SerializeField] Transform nodeParent;
    [SerializeField] bool reversePath;
    [SerializeField] bool randomPath;
    [SerializeField] float normalSpeed = 3f;

    [Header("Aggro")]//Aggro variables
    [SerializeField] float deaggroTime = 8f;
    [SerializeField] float deaggroDistance = 10f;
    [SerializeField] float yieldTime = 2f;
    [SerializeField] float aggroSpeed = 10f;

    [Header("Combat")]
    [SerializeField] float attackDistance = 5f;
    [SerializeField] HealthController healthController;
    [SerializeField] Transform player;

    [SerializeField] GameObject walkRange;
    [SerializeField] GameObject sprintRange;
    [SerializeField] GameObject crouchRange;
    [SerializeField] Animator anim;
    [SerializeField] Animator hagHair;
    [SerializeField] ParticleSystem waterEffect;
    PlayerMovement playerMov;
    bool aggrod;
    bool soundAggrod;
    bool inSight;
    bool isSearching;

    AIState state = AIState.Patrol;

    bool inLineOfSight;
    bool losAggrod;

    Coroutine aggroCor;
    Coroutine deaggroCor;
    Coroutine attackCor;
    Coroutine losCor;
    Coroutine soundAggroCor;

    int currentNode;
    List<Transform> nodes = new List<Transform>();
    bool inReverse;
    NavMeshAgent ai;

    [SerializeField] AudioManager audioMan;
    [SerializeField] AIFootsteps aiFootsteps;

    Transform soundAggro;

    void Awake()
    {
        ai = GetComponent<NavMeshAgent>();
        playerMov = player.gameObject.GetComponent<PlayerMovement>();
        if (audioMan == null) { audioMan = GetComponent<AudioManager>(); }
        //aiFootsteps = GetComponent<AIFootsteps>();
        SetUpNodes();
        transform.position = nodes[0].position;
        ai.speed = normalSpeed;
    }
    void Update()
    {
        ChangeRangeCheck();
        if(state == AIState.Patrol)
        {
            MoveToNextNode();
        }
        else if (state == AIState.Chase)
        {
            var lookAtPos = player.position;
            lookAtPos.y = transform.position.y; //set y pos to the same as mine, so I don't look up/down
            transform.LookAt(lookAtPos);
            LineOfSight();
            AttackPlayer();
            if (playerMov.isHiding)
            {
                if (audioMan.CheckIsPlaying("Chase")) { FadeOutAudio("Chase", true); }
                //audioMan.isPlaying = false;
            }
        }

        if (audioMan != null && enemyName == "Hag")
        {
            audioMan.sounds[0].source.pitch = Mathf.Lerp(0, 1, ai.velocity.magnitude);
            audioMan.sounds[2].source.pitch = Mathf.Lerp(0, 1, ai.velocity.magnitude);
        }
        if (aiFootsteps != null)
        { aiFootsteps.velocity = ai.velocity.magnitude; aiFootsteps.isAggrod = aggrod; }

    }

    public void ForceDeaggro()
    {
        isSearching = false;
        AnimateSearch();
        ResetAnimate("Fast");
        ResetAnimate("Transition");
        Animate("Slow");
        StopAudio("Fast");
        if (audioMan.CheckIsPlaying("Chase")) { FadeOutAudio("Chase", true); }
        PlayAudio("Idle");
        losAggrod = false;
        soundAggro = null;
        aggrod = false;
        ai.speed = normalSpeed;
        state = AIState.Patrol;
        losCor = null;
    }


    //Animation
    void Animate(string animation)
    {
        if (anim != null)
        {
            anim.SetTrigger(animation);
        }
        if(hagHair != null)
        {
            hagHair.SetTrigger(animation);
        }
    }
    void ResetAnimate(string animation)
    {
        if (anim != null)
        {
            anim.ResetTrigger(animation);
        }
        if (hagHair != null)
        {
            hagHair.ResetTrigger(animation);
        }
    }
    void AnimateSearch()
    {
        if (anim != null)
        {
            anim.SetBool("isSearching", isSearching);
        }
        if(enemyName == "Hag")
        {
            anim.SetTrigger("Slow");
            hagHair.SetTrigger("Slow");
        }
    }

    //Audio
    void PlayAudio(string audioName)
    {
        if (audioMan != null)
        {
            audioMan.Play(audioName);
        }
    }
    void StopAudio(string audioName)
    {
        if (audioMan != null)
        {
            audioMan.Stop(audioName);
        }
    }
    void PlayAudioForceEntirely(string audioName)
    {
        if (audioMan != null)
        {
            audioMan.PlayForceEntirely(audioName);
        }
    }
    void FadeOutAudio(string audioName, bool stopSound)
    {
        if (audioMan != null)
        {
            audioMan.VolumeFadeOut(audioName, stopSound);
        }
    }

    //Patrol
    void SetUpNodes()
    {
        currentNode = 0;
        foreach (Transform child in nodeParent)
        {
            nodes.Add(child);
        }
    }
    void MoveToNextNode()
    {
        //Check for random path parameter
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 5f && randomPath)
        {
            currentNode = Random.Range(0, nodes.Count);
        }
        else if (Vector3.Distance(transform.position, nodes[currentNode].position) < 5f)
        {
            //Check for reverse path parameter
            if (reversePath)
            {
                if (inReverse)
                {
                    currentNode--;
                    if (currentNode < 0)
                    {
                        inReverse = false;
                        currentNode = 1;
                    }
                }
                else
                {
                    currentNode++;
                    if (currentNode >= nodes.Count)
                    {
                        inReverse = true;
                        currentNode = nodes.Count - 2;
                    }
                }
            }
            else
            {
                currentNode++;
                if (currentNode >= nodes.Count)
                {
                    currentNode = 0;
                }
            }
        }
        ai.destination = nodes[currentNode].position;
    }

    //Aggro
    public void SightAggro()
    {
        RaycastHit hit;

        if (Physics.Linecast(transform.position, player.position, out hit, 3) && enemyName != "Water Monster")
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                if(soundAggroCor != null)
                {
                    StopCoroutine(soundAggroCor);
                    soundAggroCor = null;
                }

                losAggrod = true;
                if (!aggrod && aggroCor == null)
                {
                    aggroCor = StartCoroutine("StartAggro");
                }
                inLineOfSight = true;
            }
            else
            {
                inLineOfSight = false;
            }
        }
        else if (Physics.Linecast(transform.position, player.position, out hit, 3) && playerMov.isInWater)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                if (soundAggroCor != null)
                {
                    StopCoroutine(soundAggroCor);
                    soundAggroCor = null;
                }

                losAggrod = true;
                if (!aggrod && aggroCor == null)
                {
                    aggroCor = StartCoroutine("StartAggro");
                }
                inLineOfSight = true;
            }
            else
            {
                inLineOfSight = false;
            }
        }
    }
    IEnumerator StartAggro()
    {
        ai.speed = 0; 
        if (enemyName != "Water Monster")
        {
            Animate("Transition");
        }
        else
        {
            waterEffect.Stop();
        }
        StopAudio("Idle");
        PlayAudio("Stop");
        PlayAudioForceEntirely("Chase");
        yield return new WaitForSeconds(yieldTime);
        if (enemyName != "Water Monster")
        {
            Animate("Fast");
        }
        else
        {
            waterEffect.Play();
        }
        PlayAudio("Fast");
        ai.speed = aggroSpeed;
        state = AIState.Chase;
        aggrod = true;
        aggroCor = null;
    }
    void LineOfSight()
    {
        if (losAggrod)
        {
            ai.destination = player.position;
            if (!inLineOfSight || (inLineOfSight && playerMov.isHiding) || (enemyName == "Water Monster" && !playerMov.isInWater))
            {
                if (losCor == null)
                {
                    losCor = StartCoroutine("LoseLineOfSight");
                }
            }
            else if (inLineOfSight || (enemyName == "Water Monster" && playerMov.isInWater))
            {
                if (losCor != null)
                {
                    StopCoroutine(losCor);
                    losCor = null;
                }
            }
        }
    }
    IEnumerator LoseLineOfSight()
    {
        soundAggrod = false;
        float seconds = 0;
        while ((!inLineOfSight || (inLineOfSight && playerMov.isHiding) || (enemyName == "Water Monster" && !playerMov.isInWater) && !soundAggrod))
        {
            if (inLineOfSight && playerMov.isHiding || (enemyName == "Water Monster" && !playerMov.isInWater))
            {
                isSearching = true;                
                AnimateSearch();
                
            }
            if (seconds >= deaggroTime)
            {
                break;
            }

            yield return new WaitForSeconds(1f);
            seconds++;
        }
        isSearching = false;
        AnimateSearch();

        if ((!inLineOfSight || (inLineOfSight && playerMov.isHiding)) || (enemyName == "Water Monster" && !playerMov.isInWater) && !soundAggrod)
        {
            ResetAnimate("Fast");
            ResetAnimate("Transition");
            Animate("Slow");
            StopAudio("Fast");
            if (audioMan.CheckIsPlaying("Chase")) { FadeOutAudio("Chase", true); }
            PlayAudio("Idle");
            losAggrod = false;
            soundAggro = null;
            aggrod = false;
            ai.speed = normalSpeed;
            state = AIState.Patrol;
        }
        losCor = null;
    }
    void ChangeRangeCheck()
    {
        if (playerMov.isSprinting)
        {
            sprintRange.SetActive(true);
            crouchRange.SetActive(false);
            walkRange.SetActive(false);
        }
        else if (playerMov.isCrouching)
        {
            sprintRange.SetActive(false);
            crouchRange.SetActive(true);
            walkRange.SetActive(false);
        }
        else
        {
            sprintRange.SetActive(false);
            crouchRange.SetActive(false);
            walkRange.SetActive(true);
        }
    }
    public void SoundAggro(Transform sound)
    {
        if (state == AIState.Patrol)
        {
            state = AIState.SoundAggro;
            soundAggro = sound;
            if (soundAggroCor == null)
            {
                soundAggroCor = StartCoroutine("ArriveAtSoundAggro");
            }
        }
    }
    IEnumerator ArriveAtSoundAggro()
    {
        ai.speed = 0f;
        ai.destination = soundAggro.position;
        if(enemyName != "Water Monster")
        {
            Animate("Transition");
        }
        else
        {
            waterEffect.Stop();
        }
        yield return new WaitForSeconds(yieldTime);
        if (enemyName != "Water Monster")
        {
            Animate("Fast");
        }
        else
        {
            waterEffect.Play();
        }
        PlayAudio("Fast");
        ai.speed = aggroSpeed;
        while (Vector3.Distance(transform.position, soundAggro.position) > 5f)
        {
            yield return new WaitForSeconds(.25f);
        }

        float seconds = 0f;
        while(seconds < yieldTime)
        {
            ai.speed = 0f;
            yield return new WaitForSeconds(1f);
            seconds++;
            if(inLineOfSight && !playerMov.isHiding)
            {
                break;
            }
        }

        if (!inLineOfSight || (inLineOfSight && playerMov.isHiding))
        {
            print("Did not find player");
            ai.speed = normalSpeed;
            state = AIState.Patrol;
        }
        soundAggroCor = null;
    }


    //Combat
    void AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            if (inLineOfSight && !playerMov.isHiding)
            {
                if (attackCor == null)
                {
                    attackCor = StartCoroutine("Attack");
                }
            }
        }
    }
    IEnumerator Attack()
    {
        ai.speed = 0;
        healthController.HurtPlayer(0.6f);
        Animate("Attack");
        yield return new WaitForSeconds(1f);
        ResetAnimate("Attack");
        Animate("Fast");
        ai.speed = aggroSpeed;
        attackCor = null;
    }


    /// <summary>
    /// OLD CODE
    /// </summary>

    /*
        void Update()
        {
            if (!losAggrod && !soundAggrod)
            {
                MoveToNextNode();
            }
            else if(soundAggrod)
            {
                ArriveAtSoundAggro(soundAggro);
            }
            else
            {
                LineOfSight();
            }

            *//*if (!aggrod && !soundAggrod)
            {
                MoveToNextNode();
            }
            */


    /*
    else if (aggrod)
    {
        var lookAtPos = player.position;
        lookAtPos.y = transform.position.y; //set y pos to the same as mine, so I don't look up/down
        transform.LookAt(lookAtPos);
        ai.destination = player.position;
        Chase();
    }
    else if (soundAggrod)
    {
        ArriveAtSoundAggro();
    }
    UpdateInSight();
    ChangeRangeCheck();
    AnimateSearch();

    //Audio
    if (audioMan != null && enemyName == "Hag")
    {
        audioMan.sounds[0].source.pitch = Mathf.Lerp(0, 1, ai.velocity.magnitude);
        audioMan.sounds[2].source.pitch = Mathf.Lerp(0, 1, ai.velocity.magnitude);
    }
    if (aiFootsteps != null)
    { aiFootsteps.velocity = ai.velocity.magnitude; aiFootsteps.isAggrod = aggrod; }*//*
}*/



    /*public void SightAggro()
    {
        bool inRange = false;
        RaycastHit hit;

        if (Physics.Linecast(transform.position, player.position, out hit, 3))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                inRange = true;
            }
        }

        if (enemyName != "Water Monster") //any enemy but corduroy
        {            
            if (inRange)
            {
                //if player isn't hiding
                if (!playerMov.isHiding)
                {
                    if (audioMan != null)
                    {
                        audioMan.StopAllCoroutines();
                        audioMan.sounds[5].volume = 1f;
                    }
                    PlayAudioForceEntirely("Chase");
                    inSight = true;
                    aggrod = true;
                    StartAggro();
                }
            }
        }
        else //is corduroy
        {
            if (inRange)
            {
                //if player is in water
                if (playerMov.isInWater)
                {
                    inSight = true;
                    aggrod = true;
                    StartAggro();
                }
            }
        }
    }*//*
    void StartAggro()
    {
        if(aggroCor == null)
        {
            aggroCor = StartCoroutine("Aggro");
        }
    }
    void StartDeaggro()
    {
        if(deaggroCor == null)
        {
            deaggroCor = StartCoroutine("Deaggro");
        }
    }
    IEnumerator Aggro()
    {
        ai.speed = 0;
        Animate("Transition");
        StopAudio("Idle");
        PlayAudio("Stop");
        yield return new WaitForSeconds(yieldTime);
        ResetAnimate("Transition");
        Animate("Fast");
        PlayAudio("Fast");
        ai.speed = aggroSpeed;
    }
    IEnumerator Deaggro()
    {
        float seconds = 0f;
        while(seconds < deaggroTime)
        {
            if(!inSight && soundAggrod)
            {
                ai.speed = 0;
                isSearching = true;
            }
            else if (!inSight && Vector3.Distance(transform.position, player.position) <= 4f)
            {
                ai.speed = 0;
                isSearching = true;
            }
            seconds++;
            yield return new WaitForSeconds(1f);

            if(inSight)
            {
                break;
            }                      
        }

        isSearching = false;
        if(!inSight)
        {
            ai.speed = 0;
            if(aggroCor != null)
            {
                StopCoroutine(aggroCor);
                aggroCor = null;
            }
            if(attackCor != null)
            {
                StopCoroutine(attackCor);
                attackCor = null;
            }
            aggrod = false;
            soundAggrod = false;
            ai.destination = nodes[currentNode].position;
            ResetAnimate("Fast");
            StopAudio("Fast");
            FadeOutAudio("Chase", true);
            PlayAudio("Idle");
            Animate("Slow");
            ai.speed = normalSpeed;
        }
        else
        {
            aggrod = true;
            StartAggro();
            ai.speed = aggroSpeed;
        }
        deaggroCor = null;
    }
    
    void ArriveAtSoundAggro()
    {
        if (Vector3.Distance(transform.position, ai.destination) < attackDistance)
        {
            StartDeaggro();
        }
    }


    //Combat
    void Chase()
    {        
        if(inSight)
        {
            if (Vector3.Distance(transform.position, player.position) < attackDistance)
            {
                StartAttack();
            }
        }
        else
        {
            StartDeaggro();
        }
        
    }
    void StartAttack()
    {
        if(attackCor == null)
        {
            attackCor = StartCoroutine("Attack");
        }
    }
    IEnumerator Attack()
    {
        ai.speed = 0;
        Animate("Attack");
        healthController.HurtPlayer(0.6f);
        yield return new WaitForSeconds(yieldTime);
        Animate("Fast");
        ai.speed = aggroSpeed;
        attackCor = null;
    }
    void UpdateInSight()
    {
        if (enemyName != "Water Monster") //any enemy but corduroy
        {
            if(playerMov.isHiding || Vector3.Distance(transform.position, player.position) >= deaggroDistance)
            {
                inSight = false;
            }
        }
        else //is corduroy
        {
            if (!playerMov.isInWater)
            {
                inSight = false;
            }
        }
    }

    //Animation
    void Animate(string animation)
    {
        if (anim != null)
        {
            anim.SetTrigger(animation);
        }
    }
    void ResetAnimate(string animation)
    {
        if (anim != null)
        {
            anim.ResetTrigger(animation);
        }
    }

    void AnimateSearch()
    {
        if(anim != null)
        {
            anim.SetBool("isSearching", isSearching);
        }    
    }

    //Audio
    void PlayAudio(string audioName)
    {
        if (audioMan != null)
        {
            audioMan.Play(audioName);
        }
    }
    void StopAudio(string audioName)
    {
        if (audioMan != null)
        {
            audioMan.Stop(audioName);
        }
    }
    void PlayAudioForceEntirely(string audioName)
    {
        if(audioMan != null)
        {
            audioMan.PlayForceEntirely(audioName);
        }
    }
    void FadeOutAudio(string audioName, bool stopSound)
    {
        if (audioMan != null)
        {
            audioMan.VolumeFadeOut(audioName, stopSound);
        }
    }*/

}
