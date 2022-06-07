using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Animator anim;
    //Patrol
    [Header("Movement")]
    [SerializeField] private Transform nodeParent;
    [SerializeField] private bool randomPath;
    [SerializeField] private bool reversePath;
    [SerializeField] float normalSpeed = 2f;
    [SerializeField] float aggroSpeed = 6f;
    private List<Transform> nodes = new List<Transform>();

    protected NavMeshAgent ai;
    int currentNode = 0;
    bool inReverse = false;

    //Aggro
    [Header("Aggro")]
    [SerializeField] private float deaggroTime = 5f;
    [SerializeField] float yieldTime = 3f;
    [SerializeField] GameObject walkRange;
    [SerializeField] GameObject sprintRange;
    [SerializeField] GameObject crouchRange;
    protected bool isAggrod = false;
    bool playerInSight = false;
    bool isSoundAggrod = false;
    bool idle = false;

    //Attack
    [Header("Attack")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackTime = 3f;
    bool isAttacking = false;

    Transform player;
    PlayerMovement playerMov;
    [SerializeField] HealthController healthController;

    void Awake()
    {
        ai = GetComponent<NavMeshAgent>();
        SetUpNodes();
        transform.position = nodes[0].position;
        MoveToNextNode();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMov = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerMovement>();
        ai.speed = normalSpeed;
    }

    protected void Update()
    {
        if (!isAggrod && !isSoundAggrod)
        {
            MoveToNextNode();
        }
        else if (isAggrod)
        {
            ai.destination = player.position;
        }
        else if (isSoundAggrod)
        {
            ArriveAtSoundAggro();
        }
        ChangeRangeCheck();
        BeginDeaggro();
        AttackPlayer();
        UpdateHiddenStatus();

        if (isAttacking)
        {
            var lookAtPos = player.position;
            lookAtPos.y = transform.position.y; //set y pos to the same as mine, so I don't look up/down
            transform.LookAt(lookAtPos);
        }

    }
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
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 3f && randomPath)
        {
            currentNode = Random.Range(0, nodes.Count);
        }
        else if (Vector3.Distance(transform.position, nodes[currentNode].position) < 3f)
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

    IEnumerator BeginChase()
    {
        ai.speed = 0;
        anim.SetTrigger("Transition");
        yield return new WaitForSeconds(yieldTime);
        anim.SetTrigger("Fast");
        ai.speed = aggroSpeed;
    }

    //Enemy is aggrod, moves to location of sound queue.
    public void SoundAggro(Transform sound)
    {
        isSoundAggrod = true;
        ai.destination = sound.position;
        StartCoroutine("BeginChase");
    }

    //Enemy arrives at sound aggro location
    void ArriveAtSoundAggro()
    {
        if (Vector3.Distance(transform.position, ai.destination) < 5f)
        {
            BeginDeaggro();
        }
    }

    //If player is in sight and is within attack range, hit the player
    void AttackPlayer()
    {
        if (player != null)
        {
            if (playerInSight && Vector3.Distance(transform.position, player.position) <= attackRange && !isAttacking)
            {
                isAttacking = true;
                StartCoroutine("Hit");
            }
            else if (!playerInSight || Vector3.Distance(transform.position, player.position) > attackRange)
            {
                isAttacking = false;
            }
        }
    }
    IEnumerator Hit()
    {
        ai.speed = 0;
        while (true)
        {
            if (Vector3.Distance(transform.position, player.position) > attackRange)
            {
                break;
            }
            print("Hit the player");
            healthController.HurtPlayer(0.6f);
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(attackTime);
            anim.SetTrigger("Fast");
        }
        anim.SetTrigger("Fast");
        ai.speed = aggroSpeed;
    }

    //If player is within sight range and is not hidden, aggro on the player
    public void SightAggro()
    {
        bool inRange = false;

        //shoot ray to player to see if they're behind a wall
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Linecast(transform.position, player.position, out hit, 3))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                inRange = true;
            }
        }


        //Check if the player is in sight range
        if (inRange)
        {
            //Check if player is hidden
            if (!playerMov.isHiding && !isAggrod)
            {
                playerInSight = true;
                isAggrod = true;
                StartCoroutine("BeginChase");
            }
        }
        else if (!inRange && isAggrod)
        {
            if (Physics.Linecast(transform.position, player.position, out hit, 3))
            {
                if (hit.collider.gameObject.tag != "Player")
                {
                    playerInSight = false;
                    BeginDeaggro();
                }
            }
        }
    }

    void UpdateHiddenStatus()
    {
        if (playerInSight)
        {
            if (!playerMov.isHiding)
            {
                playerInSight = true;
            }
            else
            {
                playerInSight = false;
            }
        }
    }

    //If enemy is aggrod but player is not in sight
    void BeginDeaggro()
    {
        if (!idle && isAggrod && !playerInSight)
        {
            print("Starting deaggro");
            if (Vector3.Distance(transform.position, ai.destination) < 10f)
            {
                idle = true;
                StartCoroutine("LoseAggro");
            }
        }
    }

    //Wait a set amount of time before returning to patrol. Reaggro if player is in sight.
    IEnumerator LoseAggro()
    {
        int secondsElapsed = 0;
        while (!playerInSight)
        {
            print("Searching");
            yield return new WaitForSeconds(1f);
            secondsElapsed++;

            if (secondsElapsed == deaggroTime)
            {
                isAggrod = false;
                isSoundAggrod = false;
                ai.destination = nodes[currentNode].position;
                break;
            }
        }
        anim.SetTrigger("Slow");
        print("lost aggro");
        idle = false;
        ai.speed = normalSpeed;
    }
}
