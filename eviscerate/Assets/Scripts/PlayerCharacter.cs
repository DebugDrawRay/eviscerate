using UnityEngine;
using System.Collections;
using InControl;
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
        stabbed
    }

    public state currentState;
    private state previousState;

    //Components
    private Rigidbody rigid;

    [Header("Movement")]
    public GameObject motor;
    public float movementSpeed;
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

    private bool canCharge;

    private float currentAttackCharge;
    private float currentAttackPersistence;
    private GameObject currentWeapon;

    [Header("Status")]
    public float health;
    public int artifactsCollected = 0;

    [Header("Animation Loops")]
    private Animator anim;
    public AnimationClip[] movementLoops;

    //Targeting
    private int currentTargetIndex = 0;
    private bool followingTarget;
    private GameObject[] targets;

    public static PlayerCharacter instance
    {
        get;
        private set;
    }
    void Awake()
    {
        instance = this;
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
        if (currentWeapon)
        {
            currentAttackPersistence -= Time.deltaTime;
            if (currentAttackPersistence <= 0)
            {
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
                Vector3 moveDir = (motor.transform.right * input.move.X) + (motor.transform.up * input.move.Y);
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
            if(currentAttackCharge >= attackChargeThresh)
            {
                canCharge = true;
            }
        }
        else
        {
            if(canCharge)
            {
                rigid.AddForce(motor.transform.up * (maxAttackChargeForce * Mathf.Clamp(currentAttackCharge, 0, maxAttackCharge)));
                canCharge = false;
            }
            currentAttackCharge = 0;
        }
        Vector3 attackDir = (motor.transform.up.normalized * hitBoxOffset) + motor.transform.position;

        if (!currentWeapon)
        {
            currentWeapon = Instantiate(weapon, attackDir, Quaternion.identity) as GameObject;
        }
        else
        {
            currentWeapon.transform.position = attackDir;
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
        Vector3 targetPosition = targets[index].transform.position;
        Vector3 origin = motor.transform.position;
        Vector3 dir = targetPosition - origin;
        updateFacing(dir.normalized.x, dir.normalized.z);
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot = Quaternion.Euler(90, rot.eulerAngles.y, rot.eulerAngles.z);
        motor.transform.rotation = rot;
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

    public void changeStatus(float damage, bool artifact)
    {
        health += damage;
        if(health <= 0)
        {
            currentState = state.death;
        }
        if(artifact)
        {
            artifactsCollected += 1;
        }
    }

}
