using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceTypes;

public class BaseBuilding : Interactable, IBuildable, IDamageable
{

    protected bool isDead = false;
    protected bool isPlaced = false;

    public int selectionPriority = 100;

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

    public BuildState currentState = BuildState.CONSTRUCTING;

    [SerializeField]
    public List<GameObject> buildOptions = new List<GameObject>();
    public List<GameObject> buildQueue = new List<GameObject>();
    public float buildProgress = 0;
    [SerializeField]
    protected GameObject summonPositionObject;
    public Vector3 summonPosition;
    public ProjectBarManager progressBar;
    public BuildBarManager buildBar;

    #region IDamageable Specific Properties
    public float BaseHP { get => baseHP; set => baseHP = value; }
    public float CurrentHP { get => currentHP; set => currentHP = value; }
    public float CurrentDefense { get => currentDefense; set => currentDefense = value; }
    #endregion

    #region IBuildable Specific Properties
    public bool IsPlaced { get => isPlaced; set => isPlaced = value; }
    public float BuildTime { get => buildTime; set => buildTime = value; }
    public float BaseBuildTime { get => baseBuildTime; set => baseBuildTime = value; }
    public List<int> BuildCosts { get => buildCosts; }
    public List<ResourceTypes> BuildMaterials { get => buildMaterials; }
    #endregion

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
        ChangeState();
        summonPosition = summonPositionObject.transform.position;
        currentHP = baseHP;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        BuildQueueBehavior();
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
        ChangeState();
        if (DoneBeingBuilt())
        {
            currentState = BuildState.BUILT;
            
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
        // Debug.Log("DOIN IT: " + currentState);
        
        if (currentState == BuildState.CONSTRUCTING)
        {
            // Debug.Log("Poooooop");
            constructingPrefab.SetActive(true);
            builtPrefab.SetActive(false);
            destroyedPrefab.SetActive(false);
            if (progressBar != null)
            {
                buildBar.buildBar.gameObject.SetActive(true);
                progressBar.progressBar.gameObject.SetActive(false);
            }

        }

        if (currentState == BuildState.BUILT)
        {
            constructingPrefab.SetActive(false);
            builtPrefab.SetActive(true);
            destroyedPrefab.SetActive(false);
            if (progressBar != null)
            {
                buildBar.buildBar.gameObject.SetActive(false);
                progressBar.progressBar.gameObject.SetActive(true);
            }
        }

        if (currentState == BuildState.DESTROYED)
        {
            constructingPrefab.SetActive(false);
            builtPrefab.SetActive(false);
            destroyedPrefab.SetActive(true);
            if (progressBar != null)
            {
                buildBar.buildBar.gameObject.SetActive(true);
                progressBar.progressBar.gameObject.SetActive(false);
            }
        }
    }

    public virtual void AddProject(GameObject projectToAdd)
    {
        if (projectToAdd.GetComponent<IProject>() != null)
        {
            buildQueue.Add(projectToAdd);
        }

    }

    public virtual void RemoveProject(GameObject projectToRemove)
    {
        if (buildQueue.Contains(projectToRemove))
        {
            if (buildQueue.IndexOf(projectToRemove) == 0)
            {
                buildProgress = 0;
            }
            buildQueue.Remove(projectToRemove);

        }

    }

    protected virtual void BuildQueueBehavior()
    {

        if (buildQueue.Count > 0 && currentState == BuildState.BUILT)
        {
            progressBar.projectTarget = buildQueue[0].GetComponent<IProject>();
            buildProgress += Time.deltaTime;
            if (buildQueue[0].GetComponent<IProject>() != null && buildProgress > buildQueue[0].GetComponent<IProject>().BuildTime)
            {
                GameObject createdGuy = Instantiate(buildQueue[0], summonPosition, Quaternion.identity);
                createdGuy.GetComponent<IProject>().PlayerID = playerID;
                createdGuy.GetComponent<Interactable>().AssignPlayer();
                buildQueue.RemoveAt(0);
                buildProgress = 0;
            }
        }
    }

    public virtual void StartProject(int projectOptionNumber)
    {
        if (buildOptions[projectOptionNumber].GetComponent<IProject>() != null)
        {
            IProject currentProject = buildOptions[projectOptionNumber].GetComponent<IProject>();
            if (player.CheckResources(currentProject.BuildMaterials.ToArray(), currentProject.BuildCosts.ToArray()))
            {
                player.SpendResources(currentProject.BuildMaterials.ToArray(), currentProject.BuildCosts.ToArray());
                buildQueue.Add(buildOptions[projectOptionNumber]);
            }
        }

        else
        {
            Debug.Log("You fuckin goofed kiddo");
        }

    }
}
