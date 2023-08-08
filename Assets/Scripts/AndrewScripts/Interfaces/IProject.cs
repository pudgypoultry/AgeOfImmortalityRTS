using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceTypes;

public interface IProject
{
    float BuildTime { get; set; }
    List<ResourceTypes> BuildMaterials { get; }
    List<int> BuildCosts { get; set; }
    void PlaceMe(Vector3 placementPosition);
}

