using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DestructibleCrate : DynamicBlocker
{
    public void Damage()
    {
        Destroy(gameObject);
    }
}
