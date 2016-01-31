using UnityEngine;
using System.Collections;
using DG.Tweening;

public class leaperEnemy : Enemy
{
    public float sightDistance;

    public float movementSpeed;
    [Range(0, 1)]
    public float trackingLazyness;

    public float minCirclingDistance;

    public float minTimeToLeap;
    public float maxTimeToLeap;

    public float leapForce;
    public float leapDamage;

    public float leapReadyTimer;
    private float currentReadyTimer;

    public float leapRecoveryTimer;
    private float currentRecoveryTimer;

    public float passiveDamage;

    private enum state
    {
        idle,
        pursuing,
        circling,
        leaping,
        knockback
    }
    private state currentState;
    private state previousState;

    private PlayerCharacter player;

    private float currentTimer;

    private Vector3 currentDirection;

    private Tween currentTween;
    void Start()
    {
        player = PlayerCharacter.instance;
        currentTimer = Random.Range(minTimeToLeap, maxTimeToLeap);

        rigid = GetComponent<Rigidbody>();

        currentRecoveryTimer = leapRecoveryTimer;
        currentReadyTimer = leapReadyTimer;

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
                if (triggerDistance(sightDistance))
                {
                    currentState = state.pursuing;
                }
                break;
            case state.pursuing:
                pursue();
                currentTimer = Random.Range(minTimeToLeap, maxTimeToLeap);
                if (triggerDistance(minCirclingDistance))
                {
                    currentState = state.circling;
                }

                break;
            case state.circling:
                circling();
                currentTimer -= Time.deltaTime;
                if(!triggerDistance(minCirclingDistance))
                {
                    currentState = state.pursuing;
                }
                if(currentTimer <= 0)
                {
                    currentTimer = Random.Range(minTimeToLeap, maxTimeToLeap);
                    currentState = state.leaping;
                }
                break;
            case state.leaping:
                leaping();
                break;
            case state.knockback:
                knockback();
                if(!inKnockback)
                {
                    currentState = state.idle;
                }
                break;
        }

        if(inKnockback)
        {
            currentTween.Complete();
            currentTween = null;
            currentState = state.knockback;
        }
    }

    void pursue()
    {
        Vector3 direction = player.transform.position - transform.position;
        currentDirection = Vector3.Lerp(currentDirection, direction, trackingLazyness);
        currentDirection.y = 0;
        rigid.velocity = currentDirection.normalized * movementSpeed;
        damage = passiveDamage;

    }

    void circling()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction = new Vector3(-direction.z, 0, direction.x);
        currentDirection = Vector3.Lerp(currentDirection, direction, trackingLazyness);
        currentDirection.y = 0;
        rigid.velocity = currentDirection.normalized * movementSpeed;
        damage = passiveDamage;
    }

    void leaping()
    {
        shake(currentReadyTimer);
        currentReadyTimer -= Time.deltaTime;
        if(currentReadyTimer <= 0)
        {
            damage = leapDamage;
            currentTween = null;
            Vector3 direction = player.transform.position - transform.position;
            rigid.AddForce(direction.normalized * leapForce);
            currentRecoveryTimer -= Time.deltaTime;

            if(currentRecoveryTimer <= 0)
            {
                currentReadyTimer = leapReadyTimer;
                currentRecoveryTimer = leapRecoveryTimer;
                currentState = state.pursuing;
            }
        }
    }

    bool triggerDistance(float targetDistance)
    {
        float dist = Vector3.Distance(transform.position, PlayerCharacter.instance.transform.position);

        if(dist < targetDistance)
        {
            return true;
        }
        else
        { return false; }
    }

    void shake(float length)
    {
        if(currentTween == null || !currentTween.IsPlaying())
        {
            currentTween = transform.DOShakePosition(length, new Vector3(.1f,0,.1f), 90, 90);
        }
    }
}
