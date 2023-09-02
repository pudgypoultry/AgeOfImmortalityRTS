using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    float CurrentAttack { get; set; }
    float MeleeRange { get; set; }
    float ProjectileRange { get; set; }

    void AttackTarget(GameObject source, IDamageable target);
    void AttackBehavior();

}
