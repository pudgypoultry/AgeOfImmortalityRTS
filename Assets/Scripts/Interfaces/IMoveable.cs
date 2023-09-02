using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    bool IsMoveable { get; set; }
    float CurrentMovementSpeed { get; set; }
    Vector3 CurrentPosition { get; set; }
    Vector3 TargetPosition { get; set; }

    void SetPosition(GameObject source, Vector3 target);

    void MoveTo(GameObject source, Vector3 target);
}

