using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilder
{
    float CurrentBuildSpeed { get; set; }
    float MeleeRange { get; set; }

    void BuildTarget(GameObject source, IBuildable target);
    void BuildBehavior();

}
