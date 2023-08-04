using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    bool IsSelectable { get; set; }
    bool IsSelected { get; set; }
    // bool IsMoveable { get; set; }
    // bool IsAttackable { get; set; }
    bool IsGroupable { get; set; }
    bool IsGrouped { get; set; }
    bool CanAttack { get; set; }
    bool CanBuild { get; set; }
    Vector3 CurrentPosition { get; set; }

    // void SetPosition(GameObject source, Vector3 target);
    void InteractWith(GameObject source);
    void PrimaryAction(GameObject source);
    void SecondaryAction(GameObject source);
    void JoinGroup(GameObject source, List<GameObject> groupToJoin);
    void LeaveGroup(GameObject source, List<GameObject> groupToLeave);
}
