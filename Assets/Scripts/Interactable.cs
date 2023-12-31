using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, ISelectable
{
    [SerializeField]
    public int playerID = 0;
    [SerializeField]
    public PlayerController player;
    protected bool isSelectable;
    [SerializeField]
    protected bool isSelected;
    protected bool isGroupable;
    protected bool isGrouped;
    [SerializeField]
    protected bool canAttack;
    [SerializeField]
    protected bool canBuild;
    [SerializeField]
    protected bool canGather;


    protected Vector3 currentPosition;

    [SerializeField]
    protected GameObject selectorCircle;

    #region ISelectable Properties
    public bool IsSelectable { get => isSelectable; set => isSelectable = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }
    public bool IsGroupable { get => isGroupable; set => isGroupable = value; }
    public bool IsGrouped { get => isGrouped; set => isGrouped = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public bool CanBuild { get => canBuild; set => canBuild = value; }
    public bool CanGather { get => canGather; set => canGather = value; }
    public int PlayerID { get => playerID; set => playerID = value; }
    public Vector3 CurrentPosition { get => currentPosition; set => currentPosition = value; }
    #endregion

    protected virtual void Start()
    {
        selectorCircle.SetActive(false);
        if (player == null)
        {
            foreach (PlayerController controller in FindObjectsOfType<PlayerController>())
            {
                if (controller.playerID == playerID)
                {
                    player = controller;
                }
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentPosition = transform.position;
    }

    public virtual void AssignPlayer()
    {
        foreach (PlayerController controller in FindObjectsOfType<PlayerController>())
        {
            if (controller.playerID == playerID)
            {
                player = controller;
            }
        }
    }


    public virtual void InteractWith(GameObject source)
    {
        Debug.Log(name + " is attempting to be interacted with by " + source.name);
    }

    public virtual void PrimaryAction(GameObject source)
    {
        Debug.Log(name + " is attempting to perform its primary action.");
    }

    public virtual void SecondaryAction(GameObject source)
    {
        Debug.Log(name + " is attempting to perform its secondary action.");
    }

    public virtual void JoinGroup(GameObject source, List<GameObject> groupToJoin)
    {
        groupToJoin.Add(gameObject);
    }

    public virtual void LeaveGroup(GameObject source, List<GameObject> groupToJoin)
    {
        groupToJoin.Remove(gameObject);
    }

    public virtual void Select()
    {
        isSelected = true;
        selectorCircle.SetActive(true);
    }

    public virtual void Deselect()
    {
        isSelected = false;
        selectorCircle.SetActive(false);
    }
}
