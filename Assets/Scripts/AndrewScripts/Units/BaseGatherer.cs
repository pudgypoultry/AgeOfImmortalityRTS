using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGatherer : BaseUnit, IGatherer
{

    protected IGatherable currentGather;
    protected ResourceTypes currentGatherType;
    protected Dictionary<ResourceTypes, int> gatherBag = new Dictionary<ResourceTypes, int>();
    protected Dictionary<ResourceTypes, int> gatherMaximums = new Dictionary<ResourceTypes, int>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gatherBag.Add(ResourceTypes.BLOOD, 0);
        gatherBag.Add(ResourceTypes.FAITH, 0);
        gatherBag.Add(ResourceTypes.FOOD, 0);
        gatherBag.Add(ResourceTypes.GOLD, 0);
        gatherBag.Add(ResourceTypes.WOOD, 0);
        gatherBag.Add(ResourceTypes.MADNESS, 0);
        gatherBag.Add(ResourceTypes.RAGE, 0);
        gatherBag.Add(ResourceTypes.STONE, 0);

        gatherMaximums.Add(ResourceTypes.BLOOD, 50);
        gatherMaximums.Add(ResourceTypes.FAITH, 50);
        gatherMaximums.Add(ResourceTypes.FOOD, 50);
        gatherMaximums.Add(ResourceTypes.GOLD, 50);
        gatherMaximums.Add(ResourceTypes.WOOD, 50);
        gatherMaximums.Add(ResourceTypes.MADNESS, 50);
        gatherMaximums.Add(ResourceTypes.RAGE, 50);
        gatherMaximums.Add(ResourceTypes.STONE, 50);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void GatherTarget(GameObject source, IGatherable target)
    {
        Debug.Log("Attempting to gather from " + target.transform.name);
        if (canGather)
        {

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                ClearAllQueues();
            }
            if (!gatherQueue.Contains(target))
            {
                Debug.Log("POOPPOOPPOOP");
                gatherQueue.Enqueue(target);
                actionQueue.Enqueue(ActionMode.GATHER);

                currentGather = gatherQueue.Peek();
                currentGatherType = currentGather.NodeType;
                int i = 0;
                for (i = 0; i < gatherQueue.Count; i++)
                {
                    Debug.Log("Queue Member #" + i + ": " + gatherQueue.ToArray()[i]);
                }
            }
        }

        Debug.Log("Current type gathering: " + currentGatherType);
        Debug.Log("Current amount of type: " + gatherBag[currentGatherType]);
        Debug.Log("Current maximum of type: " + gatherMaximums[currentGatherType]); 
    }

    public override void GatherBehavior()
    {
        navAgent.stoppingDistance = meleeRange;
        // Debug.Log("Current gather cooldown: " + gatherTimer);
        // Debug.Log("Current amount in bag: " + gatherBag[currentGatherType]);
        if (gatherBag[currentGatherType] >= gatherMaximums[currentGatherType])
        {
            bagFull = true;
        }
        else
        {
            // Debug.Log("hello hello hello hello helloo");
            bagFull = false;
        }

        if (!bagFull)
        {
            navAgent.destination = currentGather.transform.position;
            currentGatherType = currentGather.NodeType;
            dropOffPoint = null;


            if ((transform.position - currentGather.transform.position).magnitude <= navAgent.stoppingDistance)
            {
                gatherTimer -= Time.deltaTime * currentGatherSpeed;
                // Debug.Log("Gather cooldown?: " + gatherTimer);
                if (gatherTimer <= 0)
                {
                    // Debug.Log(name + " is currently attempting to gather: " + currentGather);
                    Debug.Log(name + " currently has " + gatherBag[currentGatherType] + " in their bag.");
                    // BuildMe returns true if it's done being built
                    int gatherAmount = currentGather.GatherMe(this.gameObject, currentGatherMultiplier);
                    if (gatherAmount == -1 || currentGather == null)
                    {
                        // Debug.Log("Factors: " + gatherAmount + " | " + currentGather);
                        gatherQueue.Dequeue();
                        actionQueue.Dequeue();
                        navAgent.destination = transform.position;
                    }

                    if (gatherAmount > 0)
                    { 
                        gatherBag[currentGatherType] += gatherAmount;
                    }

                    gatherTimer = baseGatherTimer;

                }

            }
        }

        else
        {
            // If no current dropOffPoint exists, find the one with correct type and minimum distance
            if (dropOffPoint == null)
            {
                float minDistance = 999999.9f;
                foreach (IDropOff dropPoint in player.dropOffPoints)
                {
                    if (dropPoint.ResourceDropoff[currentGatherType] == true && (dropPoint.CurrentPosition - transform.position).magnitude < minDistance)
                    {
                        minDistance = (dropPoint.CurrentPosition - transform.position).magnitude;
                        dropOffPoint = dropPoint.gameObject;
                    }
                }
                Debug.Log("DROP POINT FOUND");
            }

            navAgent.destination = dropOffPoint.transform.position;

            if ((transform.position - dropOffPoint.transform.position).magnitude <= navAgent.stoppingDistance)
            {
                Debug.Log("Dropped off resources!");
                player.AddResources(currentGatherType, gatherBag[currentGatherType]);
                gatherBag[currentGatherType] = 0;
            }

        }

        if (gatherQueue.Count > 0)
        {
            currentGather = gatherQueue.Peek();
            // Debug.Log("HEY I'MA GATHERIN FROM: " + currentGather);
            // Debug.Log("Current Target is: " + currentTarget);
        }

    }
}
