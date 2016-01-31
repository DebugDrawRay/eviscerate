using UnityEngine;
using System.Collections;
using InControl;
using DG.Tweening;

public class PlayerCharacter : MonoBehaviour
{
    private PlayerActions input;

    public enum state
    {
        wakeup,
        idle,
        move,
        attack,
        death,
        stabbed,
        knockback,
        falling
    }

    public state currentState;
    private state previousState;

    //Components
    private Rigidbody rigid;

    [Header("Movement")]
    public GameObject motor;
    public float movementSpeed;
    private float originalMovementSpeed;

    private Vector2 currentFacingDirection;

    private float facingDirection;

    private Vector2 lastDirection;

    [Header("Attacks")]
    public GameObject weapon;
    public float hitBoxOffset;
    public float attackPersistence;
    public float attackChargeThresh;
    public float maxAttackChargeForce;
    public float maxAttackCharge;

    public GameObject maxChargeEffect;

    public GameObject dashEffect;
    private GameObject currentDashEffect;

    private bool canCharge;
    private bool isCharging;

    private float currentAttackCharge;
    private float currentAttackPersistence;
    private GameObject currentWeapon;

    private Vector3 attackDir;

    [Header("Status")]
    public float health;
    public float maxHealth
    {
        get;
        private set;
    }
    public int artifactsCollected = 0;

    public float regenPeriod;
    private float currentRegen;

    private Tween currentTween;
    public float hitStrength;
    public int hitVibrado;
    public float hitRandomness;
    public float hitLength;

    [Header("Animation Loops")]
    private Animator anim;
    public AnimationClip[] movementLoops;

    [Header("Targeting")]
    public GameObject targetingIndicator;
    private GameObject currentIndicator;

    private int currentTargetIndex = 0;
    private bool followingTarget;
    private GameObject[] targets;

    [Header("Knockback")]
    public float knockbackForce;
    public float knockbackLength;
    private float currentKnockback;
    private bool inKnockback;

    public static PlayerCharacter instance
    {
        get;
        private set;
    }
    void Awake()
    {
        instance = this;
        originalMovementSpeed = movementSpeed;
        maxHealth = health;
    }
    void Start()
    {
        currentAttackPersistence = attackPersistence;
        setupInput();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        targets = GameObject.FindGameObjectsWithTag("Enemy");
        currentTargetIndex = targets.Length;
    }

    void setupInput()
    {
        input = new PlayerActions();

        input.attack.AddDefaultBinding(InputControlType.Action1);
        input.target.AddDefaultBinding(InputControlType.LeftBumper);

        input.right.AddDefaultBinding(InputControlType.LeftStickRight);
        input.left.AddDefaultBinding(InputControlType.LeftStickLeft);
        input.up.AddDefaultBinding(InputControlType.LeftStickUp);
        input.down.AddDefaultBinding(InputControlType.LeftStickDown);
    }
	
	// Update is called once per frame
	void Update ()
    {
        runStates();
        if(currentRegen > 0)
        {
            currentRegen -= Time.deltaTime;
        }
        if(currentRegen <= 0)
        {
            health = maxHealth;
        }
    }

    void runStates()
    {
        switch(currentState)
        {
            case state.idle:
                anim.SetBool("IsMoving", false);

                updateFacing(lastDirection.x, lastDirection.y);
                if (input.attack.IsPressed && !currentWeapon)
                {
                    currentState = state.attack;
                }
                else if(input.move.IsPressed && !currentWeapon)
                {
                    currentState = state.move;
                }
                break;
            case state.move:
                moveCharacter();
                anim.SetBool("IsAttacking", false);

                anim.SetBool("IsMoving", true);
                if (!input.move.IsPressed)
                {
                    currentState = state.idle;
                    rigid.velocity = Vector3.zero;
                }
                if(input.attack.IsPressed && !currentWeapon)
                {
                    currentState = state.attack;
                    rigid.velocity = Vector3.zero;

                }
                break;
            case state.attack:
                attackAction();
                anim.SetBool("IsMoving", false);
                anim.SetBool("IsAttacking", true);
                currentAttackPersistence = attackPersistence;
                previousState = currentState;
                if (!input.attack.IsPressed)
                {
                    currentState = state.idle;
                }
                break;
            case state.knockback:
                currentKnockback -= Time.deltaTime;
                if(currentKnockback <= 0)
                {
                    inKnockback = false;
                    currentState = state.idle;
                }
                break;
            case state.falling:
                currentState = state.idle;
                break;
        }

        allStates();
    }

    void allStates()
    {
        targetControl();
        checkWeapon();
    }

    void checkWeapon()
    {
        attackDir = (motor.transform.up.normalized * hitBoxOffset) + motor.transform.position;
        if (currentWeapon)
        {
            currentWeapon.transform.position = attackDir;
            currentAttackPersistence -= Time.deltaTime;
            if (currentAttackPersistence <= 0)
            {
                if(currentDashEffect)
                {
                    Destroy(currentDashEffect);
                    currentDashEffect = null;
                }
                isCharging = false;
                anim.SetBool("IsAttacking", false);
                Destroy(currentWeapon);
                currentWeapon = null;
                currentAttackPersistence = attackPersistence;

            }
        }
    }
    

    void moveCharacter()
    {
        if (input.move.IsPressed)
        {
            facingDirection = Vector2.Angle(Vector2.up, input.move.Value);
            if (input.move.Value.x < 0)
            {
                facingDirection = -facingDirection;
            }
            if (followingTarget)
            {
                Vector3 moveDir = (Vector3.right * input.move.X) + (Vector3.forward * input.move.Y);
                rigid.velocity = moveDir * movementSpeed;
            }
            else
            {
                updateFacing(input.move.X, input.move.Y);
                motor.transform.rotation = Quaternion.Euler(motor.transform.rotation.eulerAngles.x, facingDirection, 0);
                rigid.velocity = motor.transform.up * movementSpeed;
            }
        }
    }

    void attackAction()
    {
        if(input.attack.IsPressed)
        {
            currentAttackCharge += Time.deltaTime;
            if(currentAttackCharge >= attackChargeThresh && !canCharge)
            {
                Instantiate(maxChargeEffect, transform.position, Quaternion.Euler(90,0,0));
                canCharge = true;
            }
        }
        else
        {
            if(canCharge)
            {
                if(!currentDashEffect)
                {
                    currentDashEffect = Instantiate(dashEffect, transform.position, Quaternion.identity) as GameObject;
                    currentDashEffect.transform.SetParent(transform);
                }
                isCharging = true;
                rigid.AddForce(motor.transform.up * (maxAttackChargeForce * Mathf.Clamp(currentAttackCharge, 0, maxAttackCharge)));
                canCharge = false;
            }
            currentAttackCharge = 0;
        }

        if (!currentWeapon)
        {
            currentWeapon = Instantiate(weapon, attackDir, motor.transform.rotation) as GameObject;
        }
    }

    void targetControl()
    {
        if(currentTargetIndex < targets.Length)
        {
            followingTarget = true;
            followTarget(currentTargetIndex);
        }
        else
        {
            followingTarget = false;
            if(currentIndicator)
            {
                Destroy(currentIndicator);
                currentIndicator = null;
            }
        }

        if(input.target.WasPressed)
        {
            currentTargetIndex++;
            if(currentTargetIndex > targets.Length)
            {
                currentTargetIndex = 0;
            }
        }
    }

    void followTarget(int index)
    {
        if (targets[index].gameObject != null)
        {
            if (!currentIndicator)
            {
                currentIndicator = Instantiate(targetingIndicator, transform.position, Quaternion.Euler(90, 0, 0)) as GameObject;
            }
            Vector3 targetPosition = targets[index].transform.position;
            Vector3 origin = motor.transform.position;
            Vector3 dir = targetPosition - origin;
            updateFacing(dir.normalized.x, dir.normalized.z);
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);
            rot = Quaternion.Euler(90, rot.eulerAngles.y, rot.eulerAngles.z);
            motor.transform.rotation = rot;
            currentIndicator.transform.position = targetPosition + Vector3.up * 3;
        }
        else
        {
            Destroy(currentIndicator);
            currentIndicator = null;
            targets = GameObject.FindGameObjectsWithTag("Enemy");
            currentTargetIndex = targets.Length;
        }
    }

    void updateFacing(float x, float y)
    {           
        if(x > 0 && x > y)
        {
            anim.SetFloat("XAxis", 1);
            anim.SetFloat("YAxis", 0);
        }
        if(y > 0 && y > x)
        {
            anim.SetFloat("XAxis", 0);
            anim.SetFloat("YAxis", 1);
        }
        if(x < 0 && Mathf.Abs(x) > y)
        {
            anim.SetFloat("XAxis", -1);
            anim.SetFloat("YAxis", 0);
        }
        if(y < 0 && Mathf.Abs(y) > x)
        {
            anim.SetFloat("XAxis", 0);
            anim.SetFloat("YAxis", -1);
        }
    }

    public void changeStatus(float damage, bool artifact, GameObject source)
    {
        if (!inKnockback && damage < 0 && source.tag == "Enemy")
        {
            triggerCamShake();
            health += damage;
            currentRegen = regenPeriod;
            if (health <= 0)
            {
                currentState = state.death;
            }
            triggerKnockback(source.transform.position);
        }
        if(artifact)
        {
            artifactsCollected += 1;
        }
    }

    public void changeSpeed(float factor)
    {
        movementSpeed = originalMovementSpeed * factor;
    }

    void triggerKnockback(Vector3 source)
    {
        currentState = state.knockback;
        inKnockback = true;
        rigid.velocity = Vector3.zero;
        currentKnockback = knockbackLength;

        Vector3 dir = transform.position - source;
        dir.y = 0;
        rigid.AddForce(dir.normalized * knockbackForce);
    }

    public void triggerFall()
    {
        transform.position = GameController.instance.currentCheckpoint.position;
        health -= 1;
        currentRegen = regenPeriod;
        if(!isCharging)
        {
            currentState = state.falling;
        }
    }

    public void triggerCamShake()
    {
        if (currentTween == null || !currentTween.IsPlaying())
        {
            Vector3 str = new Vector3(hitStrength, 0, hitStrength);
            currentTween = Camera.main.transform.DOShakePosition(hitLength, str, hitVibrado, hitRandomness);
        }
    }

}
