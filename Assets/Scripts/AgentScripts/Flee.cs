using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : AgentBehavior
{
    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        // Flipped from Seek
        steering.linear = transform.position - target.transform.position;
        steering.linear.Normalize();
        steering.linear = steering.linear * agent.maxAccel;
        return steering;
    }
}
