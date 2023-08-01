using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float CurrentHP { get; set; }
    float CurrentDefense { get; set; }
    Vector3 CurrentPosition { get; set; }
    IDamageable CurrentTarget { get; set; }
    Transform transform { get; }
    GameObject gameObject { get; }

    bool AttackMe(GameObject source, float damageAmount);
}
