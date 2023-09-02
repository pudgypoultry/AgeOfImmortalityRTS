using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDropOff : BaseBuilding, IDropOff
{

    protected Dictionary<ResourceTypes, bool> resourceDropOff = new Dictionary<ResourceTypes, bool>();
    public List<ResourceTypes> validDropOffs = new List<ResourceTypes>();
    public Dictionary<ResourceTypes, bool> ResourceDropoff { get => resourceDropOff; set => resourceDropOff = value; }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        player.dropOffPoints.Add(this);
        resourceDropOff.Add(ResourceTypes.BLOOD, false);
        resourceDropOff.Add(ResourceTypes.FAITH, false);
        resourceDropOff.Add(ResourceTypes.FOOD, false);
        resourceDropOff.Add(ResourceTypes.WOOD, false);
        resourceDropOff.Add(ResourceTypes.GOLD, false);
        resourceDropOff.Add(ResourceTypes.MADNESS, false);
        resourceDropOff.Add(ResourceTypes.RAGE, false);
        resourceDropOff.Add(ResourceTypes.STONE, false);

        foreach (ResourceTypes validType in validDropOffs)
        {
            resourceDropOff[validType] = true;
        }
        // Debug.Log("Them apples");
    }

    protected override void Update()
    {
        base.Update();
    }

    protected void OnDestroy() 
    { 
        player.dropOffPoints.Remove(this);
    }

}
