using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform nodeParent;
    [SerializeField] private float deaggroTime = 5f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackTime = 3f;
    [SerializeField] private bool randomPath;
    [SerializeField] private bool reversePath;
    private List<Transform> nodes = new List<Transform>();

    //Patrol
    protected NavMeshAgent ai;
    int currentNode = 0;
    bool inReverse = false;

    //Aggro
    protected bool isAggrod = false;
    bool playerInSight = false;
    bool isSoundAggrod = false;
    bool idle = false;
  
    //Attack
    bool isAttacking = false;
    
    Transform player;
    PlayerMovement playerMov;

    void Awake()
    {
        ai = GetComponent<NavMeshAgent>();
        SetUpNodes();
        transform.position = nodes[0].position;
        MoveToNextNode();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMov = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerMovement>();
    }


    protected void Update()
    {
        if(!isAggrod)
        {
            MoveToNextNode();
        }
        else
        {
            ai.destination = player.position;
        }
        BeginDeaggro();
        AttackPlayer();
        ArriveAtSoundAggro();
        UpdateHiddenStatus();
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
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 3f && randomPath)
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
        
    //Enemy is aggrod, moves to location of sound queue.
    public void SoundAggro(Transform sound)
    {
        isAggrod = true;
        isSoundAggrod = true;

        float distance = Vector3.Distance(nodes[0].position, sound.position);
        Transform closestNode = nodes[0];
        foreach(Transform node in nodes)
        {
            if(Vector3.Distance(node.position, sound.position) < distance)
            {
                closestNode = node;
                distance = Vector3.Distance(node.position, sound.position);
            }
        }
        ai.destination = closestNode.position;
        BeginDeaggro();
    }

    //Enemy arrives at sound aggro location
    void ArriveAtSoundAggro()
    {
        if(isSoundAggrod)
        {
            isSoundAggrod = false;
            if(Vector3.Distance(transform.position, ai.destination) < .5f )
            {
                BeginDeaggro();
            }
        }
    }

    //If player is in sight and is within attack range, hit the player
    void AttackPlayer()
    {
        if(player != null)
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
        while (true)
        {
            if (Vector3.Distance(transform.position, player.position) > attackRange)
            {
                break;
            }
            print("Hit the player");
            yield return new WaitForSeconds(attackTime);
        }        
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SightAggro();
        }
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
            if(hit.collider.gameObject.tag == "Player")
            {
                inRange = true;
            }
        }


        //Check if the player is in sight range
        if (inRange)
        {
            //Check if player is hidden
            if (!playerMov.isHiding)
            {
                playerInSight = true;
                isAggrod = true;
            }
            else
            {
                playerInSight = false;
            }
        }        
    }

    void UpdateHiddenStatus()
    {
        if(playerInSight)
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
        if(!idle && isAggrod && !playerInSight)
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
        while(!playerInSight)
        {
            print("Searching");
            yield return new WaitForSeconds(1f);
            secondsElapsed++;

            if (secondsElapsed == deaggroTime)
            {
                isAggrod = false;
                ai.destination = nodes[currentNode].position;
                break;
            }
        }
        idle = false;
    }
}
