using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class BaseUnit : Interactable, IDamageable, IMoveable
{

    protected bool isMoveable = true;
    protected bool isAttackable = true;
    protected bool isAttacking = false;

    protected bool attackCommandIssued;
    protected bool isDead;

    [SerializeField]
    protected float baseHP = 10;
    private float currentHP;
    [SerializeField]
    protected float baseDefense = 1;
    protected float currentDefense;
    [SerializeField]
    protected float baseAttack = 1;
    protected float currentAttack;
    [SerializeField]
    protected float baseAttackSpeed = 1.0f;
    protected float currentAttackSpeed;
    protected float baseAttackTimer = 1.0f;
    protected float attackTimer = 1.0f;
    [SerializeField]
    protected float baseMovementSpeed = 1;
    protected float currentMovementSpeed;
    [SerializeField]
    protected float meleeRange = 1;
    [SerializeField]
    protected float projectileRange = 10;

    [SerializeField]
    protected float distanceTolerance = 0.5f;
    protected Vector3 targetPosition;
    protected IDamageable currentTarget;

    protected List<GameObject> currentGroup;
    protected Queue<Vector3> moveQueue = new Queue<Vector3>();
    protected Queue<IDamageable> attackQueue = new Queue<IDamageable>();
    protected NavMeshAgent navAgent;



    #region Shared Interface Properties
    public bool IsAttackable { get => isAttackable; set => isAttackable = value; }
    public float CurrentAttackSpeed { get => currentAttackSpeed; set => currentAttackSpeed = value; }
    public bool IsMoveable { get => isMoveable; set => isMoveable = value; }

    #endregion

    #region IDamageable Specific Properties
    public float CurrentHP { get => currentHP; set => currentHP = value; }
    public float CurrentDefense { get => currentDefense; set => currentDefense = value; }
    public IDamageable CurrentTarget { get => currentTarget; set => currentTarget = value; }
    #endregion

    #region IMoveable Specific Properties
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public float CurrentMovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = value; }
    public Queue<Vector3> MoveQueue { get => moveQueue; set => moveQueue = value; }
    #endregion


    public enum ActionMode
    {
        ACTION,
        ATTACK,
        BUILD,
        IDLE,
        MOVE
    }

    public ActionMode currentActionMode = ActionMode.IDLE;
    protected Queue<ActionMode> actionQueue = new Queue<ActionMode>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = baseMovementSpeed;
        navAgent.stoppingDistance = meleeRange;
        ResetToBase();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (actionQueue.Count > 0)
        {
            /* 
            if (currentActionMode != actionQueue.Peek())
            {
                Debug.Log("Changing from " + currentActionMode + " mode to " + actionQueue.Peek() + " mode.");
            }
            */
            currentActionMode = actionQueue.Peek();
            // Debug.Log("Current Mode: " + currentActionMode);
        }
        else
        {
            currentActionMode = ActionMode.IDLE;
        }

        if (currentActionMode == ActionMode.ATTACK)
        {
            AttackBehavior();
        }

        else if (currentActionMode == ActionMode.MOVE)
        {
            MoveBehavior();
        }

        else if (currentActionMode != ActionMode.IDLE)
        {
            navAgent.destination = transform.position;
        }


    }

    public virtual void ResetToBase()
    {
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentHP = baseHP;
        currentMovementSpeed = baseMovementSpeed;
        currentAttackSpeed = baseAttackSpeed;
    }

    // Public function that allows other units to attack this
    public virtual bool AttackMe(GameObject source, float damageAmount)
    {
        damageAmount = damageAmount * currentDefense;
        if (damageAmount > 0.5)
        {
            damageAmount = 1;
        }

        currentHP -= damageAmount;
        // Debug.Log(name + "'s currentHP: " + currentHP);
        if (currentHP <= 0)
        {
            KillMe();
            return true;
        }

        return false;
    }

    protected virtual void KillMe()
    {
        // Debug.Log(name + " has died!");
        if (!isDead)
        {
            isDead = true;
            Destroy(gameObject, 0.2f);
        }

    }

    public virtual void SetPosition(GameObject source, Vector3 target) 
    {
        transform.position = target;
    }

    public virtual void MoveTo(GameObject source, Vector3 target)
    {

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            ClearAllQueues();
        }
        moveQueue.Enqueue(target);
        actionQueue.Enqueue(ActionMode.MOVE);

    }

    public virtual void AttackTarget(GameObject source, IDamageable target)
    {

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            ClearAllQueues();
        }
        if (!attackQueue.Contains(target))
        {
            attackQueue.Enqueue(target);
            actionQueue.Enqueue(ActionMode.ATTACK);

            currentTarget = attackQueue.Peek();
            int i = 0;
            for (i = 0; i < attackQueue.Count; i++)
            { 
                // Debug.Log("Queue Member #" + i + ": " + attackQueue.ToArray()[i]);
            }
        }

        // Debug.Log("Done this job");
    }

    protected virtual void MoveBehavior()
    {
        navAgent.destination = moveQueue.Peek();
        if ((transform.position - moveQueue.Peek()).magnitude <= distanceTolerance)
        {
            moveQueue.Dequeue();
            actionQueue.Dequeue();
        }
    }

    protected virtual void AttackBehavior()
    {
        attackTimer -= Time.deltaTime * baseAttackSpeed;
        navAgent.destination = currentTarget.CurrentPosition;

        if ((transform.position - currentTarget.CurrentPosition).magnitude <= meleeRange)
        {
            if (attackTimer <= 0)
            {
                // Debug.Log("Made it to the damage step");
                if (currentTarget.AttackMe(this.gameObject, currentAttack) || currentTarget == null)
                {
                    attackQueue.Dequeue();
                    actionQueue.Dequeue();
                    navAgent.destination = transform.position;
                }

                attackTimer = baseAttackTimer;
            }

            if (attackQueue.Count > 0)
            {
                currentTarget = attackQueue.Peek();
                // Debug.Log("Current Target is: " + currentTarget);
            }
        }
    }

    public virtual void ClearAllQueues()
    {
        attackQueue.Clear();
        moveQueue.Clear();
        actionQueue.Clear();
    }

    /*
        if (currentTarget == null)
        {
            isAttacking = false;
        }

        if (moveQueue.Count > 0 && !isAttacking)
        {
            navAgent.destination = moveQueue.Peek();
            // IF STATEMENT NOT TRIPPING
            if ((transform.position - moveQueue.Peek()).magnitude <= distanceTolerance && moveQueue.Count > 1)
            {
                moveQueue.Dequeue();
                // Debug.Log("POOP " + ((transform.position - moveQueue.Peek()).magnitude - distanceTolerance));
            }

        }
        else if (!isAttacking && attackQueue.Count == 0)
        {
            navAgent.destination = transform.position;
        }

        if (isAttacking)
        {

            attackTimer -= Time.deltaTime * baseAttackSpeed;
            // Debug.Log("Poop");
            navAgent.destination = currentTarget.CurrentPosition;

            if ((transform.position - currentTarget.CurrentPosition).magnitude <= meleeRange)
            {
                if (currentTarget != null)
                {
                    if (attackTimer <= 0)
                    {
                        Debug.Log("Made it to the damage step");
                        if (currentTarget.AttackMe(this.gameObject, currentAttack))
                        {
                            isAttacking = false;
                            if (attackQueue.Count == 1)
                            {
                                attackQueue.Clear();
                            }
                        }

                        attackTimer = baseAttackTimer;
                    }
                    if (attackQueue.Count > 1 && attackQueue[0] == null && attackQueue[1] != null)
                    {
                        attackQueue.RemoveAt(0);
                        currentTarget = attackQueue[0];
                        Debug.Log("Current Target is: " + currentTarget);
                    }
                    navAgent.destination = transform.position;

                }
                else
                {
                    Debug.Log(name + " is giving up");
                }
            }
        }

        else if (!isAttacking && attackQueue.Count > 1)
        {
            isAttacking = true;
            attackQueue.RemoveAt(0);
            currentTarget = attackQueue[0];
            Debug.Log("Current Target is: " + currentTarget);
        }
        */
}
