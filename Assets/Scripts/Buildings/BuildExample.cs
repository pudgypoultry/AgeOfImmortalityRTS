using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildExample : MonoBehaviour
{
    public GameObject exampleBlueprint;

    public void SpawnExampleBlueprint()
    {
        Debug.Log("making example building");
        Instantiate(exampleBlueprint);
    }
}
