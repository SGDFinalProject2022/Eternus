using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HagAI : EnemyAI
{
    [SerializeField] float normalSpeed = 2f;
    [SerializeField] float aggroSpeed = 6f;

    void Update()
    {
        base.Update();
        if (isAggrod)
        {
            //play aggrod sound
            ai.speed = aggroSpeed;
        }
        else
        {
            //play normal sound
            ai.speed = normalSpeed;
        }
    }
}
