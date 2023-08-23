using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGatherer
{
    float CurrentGatherSpeed { get; set; }
    float MeleeRange { get; set; }
    void GatherTarget(GameObject source, IGatherable target);
    void GatherBehavior();
}
