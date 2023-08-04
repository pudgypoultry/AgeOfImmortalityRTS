using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildable
{
    float BuildTime { get; set; }
    Vector3 CurrentPosition { get; set; }
    List<int> BuildCosts { get; set; }
    List<string> BuildMaterials { get; set; }
    void PlaceMe(Vector3 placementPosition);
    bool BuildMe(GameObject source);
}
