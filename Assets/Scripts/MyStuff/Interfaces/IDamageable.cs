using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float CurrentHP { get; set; }
    float CurrentDefense { get; set; }
    Vector3 CurrentPosition { get; set; }
    GameObject CurrentTarget { get; set; }

    void AttackMe(GameObject source, float damageAmount);
}
