using UnityEngine;
using System.Collections;

public class blobEnemy : Enemy
{
    public float triggerDistance;
    public float movementSpeed;
    public float trackingLazyness;

    public float passiveDamage;
    private Vector3 currentDirection;

    private enum state
    {
        idle,
        following,
        knockback
    }

    private state currentState;

    void Start()
    {
        damage = passiveDamage;
    }
    void Update()
    {
        runStates();
    }

    void runStates()
    {
        switch(currentState)
        {
            case state.idle:
                float dist = Vector3.Distance(transform.position, PlayerCharacter.instance.transform.position);
                if(dist < triggerDistance)
                {
                    currentState = state.following;
                }
                break;
            case state.following:
                followPlayer();
                break;
            case state.knockback:
                knockback();
                if (!inKnockback)
                {
                    currentState = state.idle;
                }
                break;
        }

        if (inKnockback)
        {
            currentState = state.knockback;
        }
    }

    void followPlayer()
    {
        Vector3 direction = PlayerCharacter.instance.transform.position - transform.position;
        currentDirection = Vector3.Lerp(currentDirection, direction, trackingLazyness);
        rigid.velocity = currentDirection.normalized * movementSpeed;
    }
}
