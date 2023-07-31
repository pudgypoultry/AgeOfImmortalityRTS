using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class BaseUnitCopy : BaseUnit
{

    protected bool isMoveable = true;
    protected bool isAttackable = true;
    protected bool isAttacking = false;

    protected bool attackCommandIssued;

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
    protected float baseMovementSpeed = 1;
    protected float currentMovementSpeed;
    [SerializeField]
    protected float meleeRange = 1;
    [SerializeField]
    protected float projectileRange = 10;

    [SerializeField]
    protected float distanceTolerance = 0.5f;
    protected Vector3 targetPosition;
    protected GameObject currentTarget;

    protected List<GameObject> currentGroup;
    protected Queue<Vector3> moveQueue = new Queue<Vector3>();
    protected NavMeshAgent navAgent;



    #region Shared Interface Properties
    public bool IsAttackable { get => isAttackable; set => isAttackable = value; }

    public bool IsMoveable { get => isMoveable; set => isMoveable = value; }

    #endregion

    #region IDamageable Specific Properties
    public float CurrentHP { get => currentHP; set => currentHP = value; }
    public float CurrentDefense { get => currentDefense; set => currentDefense = value; }
    public GameObject CurrentTarget { get => currentTarget; set => currentTarget = value; }
    #endregion

    #region IMoveable Specific Properties
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public float CurrentMovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = value; }
    public Queue<Vector3> MoveQueue { get => moveQueue; set => moveQueue = value; }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = baseMovementSpeed;
        navAgent.stoppingDistance = meleeRange;
        ResetToBase();
    }

    // Update is called once per frame
    void Update()
    {
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
                Debug.Log("POOP " + ((transform.position - moveQueue.Peek()).magnitude - distanceTolerance));
            }

        }
        else if (!isAttacking)
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
    }

    public virtual void AttackMe(GameObject source, float damageAmount)
    {
        damageAmount = damageAmount * currentDefense;
        if (damageAmount > 0.5)
        {
            damageAmount = 1;
        }

        currentHP -= damageAmount;

        if (currentHP <= 0)
        {
            KillMe();
        }
    }

    protected virtual void KillMe()
    {
        Debug.Log(name + " has died!");
        Destroy(gameObject);
    }

    public virtual void SetPosition(GameObject source, Vector3 target) 
    {
        transform.position = target;
    }

    public virtual void MoveTo(GameObject source, Vector3 target)
    {
        isAttacking = false;
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            moveQueue.Clear();
        }
        moveQueue.Enqueue(target);
        int i = 0;
        foreach (Vector3 thing in moveQueue)
        {
            Debug.Log("     " + i++ + thing);
        }
    }

   
}
