using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField] NewEnemyAI ai;
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ai.SightAggro();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ai.SightAggro();
        }
    }
}
