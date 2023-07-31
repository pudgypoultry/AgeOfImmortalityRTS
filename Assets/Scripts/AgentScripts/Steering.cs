using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering
{
    // Start is called before the first frame update
    public float angular; // rotation 0 to 360
    public Vector3 linear; // instantaneous velocity
    
    public Steering()
    {
        angular = 0.0f;
        linear = new Vector3();
    }
}
