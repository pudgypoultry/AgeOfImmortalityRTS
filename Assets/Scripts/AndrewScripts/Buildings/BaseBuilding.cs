using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceTypes;

public class BaseBuilding : Interactable, IBuildable, IDamageable
{

    protected bool isDead = false;
    protected bool isPlaced = false;

    [SerializeField]
    protected float baseHP = 10;
    private float currentHP;
    [SerializeField]
    protected float baseDefense = 1;
    protected float currentDefense;
    [SerializeField]
    protected float baseBuildTime = 5.0f;
    protected float buildTime = 0;
    [SerializeField]
    protected List<int> buildCosts = new List<int>();
    [SerializeField]
    protected List<ResourceTypes> buildMaterials = new List<ResourceTypes>();

    [SerializeField]
    protected GameObject constructingPrefab;
    [SerializeField]
    protected GameObject builtPrefab;
    [SerializeField]
    protected GameObject destroyedPrefab;

    protected BuildState currentState = BuildState.CONSTRUCTING;

    [SerializeField]
    public List<GameObject> buildOptions = new List<GameObject>();

    #region IDamageable Specific Properties
    public float CurrentHP { get => currentHP; set => currentHP = value; }
    public float CurrentDefense { get => currentDefense; set => currentDefense = value; }
    #endregion

    #region IBuildable Specific Properties
    public bool IsPlaced { get => isPlaced; set => isPlaced = value; }
    public float BuildTime { get => buildTime; set => buildTime = value; }
    public List<int> BuildCosts { get => buildCosts; }
    public List<ResourceTypes> BuildMaterials { get => buildMaterials; }
    #endregion

    /*
    public enum ResourceTypes
    {
        FOOD,
        GOLD,
        MADNESS,
        RAGE,
        STONE,
        WOOD
    }
    */

    public enum BuildState
    { 
        CONSTRUCTING,
        BUILT,
        DESTROYED
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

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

    public virtual void DestroyMe()
    { 
        currentState = BuildState.DESTROYED;
        buildTime = 0;
        baseBuildTime = baseBuildTime / 2;
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

    public virtual void PlaceMe(Vector3 placementPosition)
    { 
        transform.position = placementPosition;
        
    }

    public virtual bool DoneBeingBuilt()
    {
        if (buildTime >= baseBuildTime)
        {
            return true;
        }

        return false;
    }
    public virtual bool BuildMe(GameObject source, float buildAmount)
    {
        buildTime += buildAmount;

        if (DoneBeingBuilt())
        {
            currentState = BuildState.BUILT;
            ChangeState();
        }
        
        if (currentState != BuildState.BUILT)
        {
            return false;
        }

        return true;
    }

    public virtual bool CanBeBuilt(PlayerController player)
    {
        return player.CheckResources(buildMaterials.ToArray(), buildCosts.ToArray());
    }

    public virtual bool BeginBuild(PlayerController player, Vector3 placementPosition)
    {
        if (CanBeBuilt(player))
        {
            player.SpendResources(buildMaterials.ToArray(), buildCosts.ToArray());
            PlaceMe(placementPosition);
            return true;
        }

        return false;
    }

    protected virtual void ChangeState()
    {
        if (currentState == BuildState.CONSTRUCTING)
        {
            constructingPrefab.SetActive(true);
            builtPrefab.SetActive(false);
            destroyedPrefab.SetActive(false);
        }

        if (currentState == BuildState.BUILT)
        {
            constructingPrefab.SetActive(false);
            builtPrefab.SetActive(true);
            destroyedPrefab.SetActive(false);
        }

        if (currentState == BuildState.DESTROYED)
        {
            constructingPrefab.SetActive(false);
            builtPrefab.SetActive(false);
            destroyedPrefab.SetActive(true);
        }
    }
}
