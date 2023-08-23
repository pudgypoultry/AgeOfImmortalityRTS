using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceTypes;

public interface IBuildable
{
    bool IsPlaced { get; set; }
    float BuildTime { get; set; }
    Vector3 CurrentPosition { get; set; }
    List<int> BuildCosts { get; }
    List<ResourceTypes> BuildMaterials { get; }
    void PlaceMe(Vector3 placementPosition);
    bool BuildMe(GameObject source, float buildAmount);
}
