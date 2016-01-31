using UnityEngine;
using System.Collections;

public class laserEnemy : Enemy
{

    public float triggerDistance;
    public float movementSpeed;
    public float trackingLazyness;

    public float passiveDamage;
    private Vector3 currentDirection;

    public GameObject projectile;

    public float firingInterval;
    private float currentInterval;

    private enum state
    {
        idle,
        shooting,
        knockback
    }

    private state currentState;

    void Start()
    {
        currentInterval = firingInterval;
        damage = passiveDamage;
    }
    void Update()
    {
        runStates();
    }

    void runStates()
    {
        switch (currentState)
        {
            case state.idle:
                rigid.velocity = Vector3.zero;
                float dist = Vector3.Distance(transform.position, PlayerCharacter.instance.transform.position);
                if (dist < triggerDistance)
                {
                    currentState = state.shooting;
                }
                break;
            case state.shooting:
                shootPlayer();
                float currentDist = Vector3.Distance(transform.position, PlayerCharacter.instance.transform.position);
                if (currentDist > triggerDistance)
                {
                    currentState = state.idle;
                }
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

    void shootPlayer()
    {
        float dist = Vector3.Distance(transform.position, PlayerCharacter.instance.transform.position);
        if (dist <= triggerDistance / 2)
        {
            Vector3 direction = transform.position - PlayerCharacter.instance.transform.position;
            currentDirection = Vector3.Lerp(currentDirection, direction, trackingLazyness);
            rigid.velocity = currentDirection.normalized * movementSpeed;
        }

        currentInterval -= Time.deltaTime;
        if(currentInterval <= 0)
        {
            GameObject newProj = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;

            Vector3 dir = PlayerCharacter.instance.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            rot = Quaternion.Euler(90, rot.eulerAngles.y, rot.eulerAngles.z);
            newProj.transform.rotation = rot;

            currentInterval = firingInterval;
        }
    }
}
