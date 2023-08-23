using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGatherable
{
    int ResourceAmount { get; set; }
    ResourceTypes NodeType { get; }
    int GatherMe(GameObject source, float gatherMultiplier);
    Transform transform { get; }
    GameObject gameObject { get; }
}
