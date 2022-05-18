using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private Transform nodeParent;
    [SerializeField] private float deaggroTime = 5f;
    [SerializeField] private float aggroRange = 3f;
    private List<Transform> nodes = new List<Transform>();

    NavMeshAgent ai;
    int currentNode = 0;
    bool isAggrod = false;
    bool playerInSight = false;
    bool idle = false;
    bool playerIsHidden = false;
    Transform player;

    void Awake()
    {
        ai = GetComponent<NavMeshAgent>();
        ai.speed = movementSpeed;
        
        SetUpNodes();
        MoveToNextNode();
    }


    void Update()
    {
        if(!isAggrod && Vector3.Distance(transform.position, nodes[currentNode].position) < 3f)
        {
            MoveToNextNode();
        }
        SightAggro();
        BeginDeaggro();
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
        currentNode++;
        if(currentNode >= nodes.Count)
        {
            currentNode = 0;
        }
        ai.destination = nodes[currentNode].position;
    }

    void SoundAggro(Transform node)
    {
        isAggrod = true;
        ai.destination = node.position;

        BeginDeaggro();
    }

    void SightAggro()
    {
        bool inRange = false;
        Collider[] hit = Physics.OverlapSphere(transform.position, aggroRange);
        foreach(Collider col in hit)
        {
            if(col.tag == "Player")
            {
                inRange = true;
                player = col.gameObject.transform;
            }
        }

        if (inRange && !playerIsHidden)
        {
            playerInSight = true;
            isAggrod = true;
            ai.destination = player.position;
        }
        else if(playerIsHidden)
        {
            playerInSight = false;
        }  
    }

    void BeginDeaggro()
    {
        if(!idle && isAggrod && !playerInSight)
        {
            if (Vector3.Distance(transform.position, ai.destination) < 3f)
            {
                idle = true;
                StartCoroutine("LoseAggro");
            }
        }               
    }

    IEnumerator LoseAggro()
    {
        int secondsElapsed = 0;
        while(!playerInSight)
        {
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
