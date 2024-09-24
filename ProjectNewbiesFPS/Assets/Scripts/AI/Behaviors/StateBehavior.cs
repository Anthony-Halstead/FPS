using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBehavior : ScriptableObject
{
    public abstract void ExecuteBehavior(AIController controller);
}
