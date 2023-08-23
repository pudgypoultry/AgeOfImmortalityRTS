using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDropOff
{
    Dictionary<ResourceTypes, bool> ResourceDropoff { get; set; }
    Vector3 CurrentPosition { get; set; }
    GameObject gameObject { get; }
    Transform transform { get; }
}