using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNode : Interactable, IGatherable
{
    [SerializeField]
    protected ResourceTypes nodeType;
    [SerializeField]
    protected int resourceAmount = 100;
    [SerializeField]
    protected int gatherRate = 10;

    public ResourceTypes NodeType { get => nodeType; }
    public int ResourceAmount { get => resourceAmount; set => resourceAmount = value; }

    public virtual int GatherMe(GameObject source, float gatherMultiplier)
    {
        Debug.Log(name + "is being gathered from");
        int returnAmount = Mathf.RoundToInt(gatherMultiplier * gatherRate);
        if (resourceAmount >= returnAmount)
        {
            resourceAmount -= returnAmount;
            return returnAmount;
        }
        else
        {
            returnAmount = resourceAmount;
            resourceAmount -= returnAmount;

            if (returnAmount <= 0)
            {
                Debug.Log("Hey fucka you I want two fork on de table");
                return -1;
            }

            return returnAmount;
        }

    }
}
