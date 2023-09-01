using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static ResourceTypes;

[RequireComponent(typeof(NavMeshAgent))]

public class BaseUnit : Interactable, IDamageable, IMoveable, IProject, IAttacker, IBuilder
{

    protected bool isMoveable = true;
    protected bool isAttackable = true;
    protected bool isAttacking = false;
    protected bool bagFull = false;

    protected bool attackCommandIssued;
    protected bool isDead;
    [SerializeField]
    protected float buildTime = 1.0f;

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
    protected float baseBuildSpeed = 1.0f;
    protected float currentBuildSpeed;
    protected float baseBuildTimer = 1.0f;
    protected float buildTimer = 1.0f;
    [SerializeField]
    protected float baseGatherSpeed = 1.0f;
    protected float currentGatherSpeed;
    protected float baseGatherTimer = 1.0f;
    protected float gatherTimer = 1.0f;
    protected float baseGatherMultiplier = 1.0f;
    protected float currentGatherMultiplier;
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
    protected GameObject dropOffPoint;
    protected IDamageable currentTarget;
    protected IBuildable currentBuild;

    protected List<GameObject> currentGroup;

    protected Queue<Vector3> moveQueue = new Queue<Vector3>();
    protected Queue<IDamageable> attackQueue = new Queue<IDamageable>();
    protected Queue<IBuildable> buildQueue = new Queue<IBuildable>();
    protected Queue<IGatherable> gatherQueue = new Queue<IGatherable>();

    protected NavMeshAgent navAgent;

    [SerializeField]
    protected List<ResourceTypes> buildMaterials = new List<ResourceTypes>();
    [SerializeField]
    protected List<int> buildCosts = new List<int>();

    #region Shared Interface Properties
    public bool IsAttackable { get => isAttackable; set => isAttackable = value; }
    public float CurrentAttackSpeed { get => currentAttackSpeed; set => currentAttackSpeed = value; }
    public bool IsMoveable { get => isMoveable; set => isMoveable = value; }
    public float MeleeRange { get => meleeRange; set => meleeRange = value; }

    #endregion

    #region IDamageable Specific Properties
    public float BaseHP { get => baseHP; set => baseHP = value; }
    public float CurrentHP { get => currentHP; set => currentHP = value; }
    public float CurrentDefense { get => currentDefense; set => currentDefense = value; }
    public IDamageable CurrentTarget { get => currentTarget; set => currentTarget = value; }
    #endregion

    #region IMoveable Specific Properties
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public float CurrentMovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = value; }
    public Queue<Vector3> MoveQueue { get => moveQueue; set => moveQueue = value; }
    #endregion

    #region IProject Specific Properties
    public float BuildTime { get => buildTime; set => buildTime = value; }
    public List<ResourceTypes> BuildMaterials { get => buildMaterials; set => buildMaterials = value; }
    public List<int> BuildCosts { get => buildCosts; set => buildCosts = value; }

    #endregion

    #region IAttacker Specific Properties
    public float CurrentAttack { get => currentAttack; set => currentAttack = value; }

    public float ProjectileRange { get => projectileRange; set => projectileRange = value; }
    #endregion

    #region IBuilder Specific Properties
    public float CurrentBuildSpeed { get => currentBuildSpeed; set => currentBuildSpeed = value; }
    #endregion

    #region IGatherer Specific Properties
    public float CurrentGatherSpeed { get => currentGatherSpeed; set => currentGatherSpeed = value; }
    #endregion

    public enum ActionMode
    {
        ACTION,
        ATTACK,
        BUILD,
        GATHER,
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
            // Debug.Log("Made it here");
        }

        else if (currentActionMode == ActionMode.BUILD)
        {
            BuildBehavior();
        }

        else if (currentActionMode == ActionMode.GATHER)
        {
            GatherBehavior();
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
        currentGatherSpeed = baseGatherSpeed;
        currentGatherMultiplier = baseGatherMultiplier;
        currentBuildSpeed = baseBuildSpeed;
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
        moveQueue.Enqueue(new Vector3(target.x, transform.position.y, target.z));
        actionQueue.Enqueue(ActionMode.MOVE);

    }

    public virtual void AttackTarget(GameObject source, IDamageable target)
    {
        if (canAttack)
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
                Debug.Log(currentTarget);
                int i = 0;
                for (i = 0; i < attackQueue.Count; i++)
                {
                    // Debug.Log("Queue Member #" + i + ": " + attackQueue.ToArray()[i]);
                }
            }
        }


        // Debug.Log("Done this job");
    }

    public virtual void BuildTarget(GameObject source, IBuildable target)
    {
        if (canBuild)
        {
            
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                ClearAllQueues();
            }
            if (!buildQueue.Contains(target))
            {
                buildQueue.Enqueue(target);
                actionQueue.Enqueue(ActionMode.BUILD);

                currentBuild = buildQueue.Peek();
                int i = 0;
                for (i = 0; i < buildQueue.Count; i++)
                {
                    // Debug.Log("Queue Member #" + i + ": " + attackQueue.ToArray()[i]);
                }
            }
        }

    }

    public virtual void GatherTarget(GameObject source, IGatherable target)
    {
        Debug.Log("I should be inherited this shouldn't be happening");
    }

    protected virtual void MoveBehavior()
    {
        navAgent.destination = moveQueue.Peek();
        navAgent.stoppingDistance = distanceTolerance;
        if ((transform.position - moveQueue.Peek()).magnitude <= navAgent.stoppingDistance)
        {
            moveQueue.Dequeue();
            actionQueue.Dequeue();
        }
    }

    public virtual void AttackBehavior()
    {
        attackTimer -= Time.deltaTime * currentAttackSpeed;
        navAgent.destination = currentTarget.CurrentPosition;
        navAgent.stoppingDistance = meleeRange;

        if ((transform.position - currentTarget.CurrentPosition).magnitude <= navAgent.stoppingDistance)
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

    public virtual void BuildBehavior()
    {
        
        navAgent.destination = currentBuild.CurrentPosition;
        navAgent.stoppingDistance = meleeRange;


        if ((transform.position - currentBuild.CurrentPosition).magnitude <= navAgent.stoppingDistance)
        {
            buildTimer -= Time.deltaTime * currentBuildSpeed;
            if (buildTimer <= 0)
            {
                Debug.Log(name + " is currently attempting to build: " + currentBuild);
                
                Debug.Log(currentBuild + " is done building: " + currentBuild.BuildMe(this.gameObject, currentBuildSpeed));
                // BuildMe returns true if it's done being built
                if (currentBuild.BuildMe(this.gameObject, currentBuildSpeed) || currentBuild == null)
                {
                   
                    buildQueue.Dequeue();
                    actionQueue.Dequeue();
                    navAgent.destination = transform.position;
                }

                buildTimer = baseBuildTimer;
            }

            if (buildQueue.Count > 0)
            {
                currentBuild = buildQueue.Peek();
                // Debug.Log("Current Target is: " + currentTarget);
            }
        }
    }

    public virtual void GatherBehavior()
    {


    }

    public virtual void ClearAllQueues()
    {
        attackQueue.Clear();
        moveQueue.Clear();
        buildQueue.Clear();
        gatherQueue.Clear();
        actionQueue.Clear();

    }

    public void PlaceMe(Vector3 placementPosition)
    {
        Instantiate(gameObject, placementPosition, Quaternion.identity);
    }

    // Overload for future use of a rally point
    public void PlaceMe(Vector3 placementPosition, Vector3 rallyPosition)
    {
        BaseUnit newUnitCopy = Instantiate(gameObject, placementPosition, Quaternion.identity).GetComponent<BaseUnit>();
        newUnitCopy.MoveTo(gameObject, rallyPosition);
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
