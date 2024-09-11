using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for choosing how objects should be spawning from spawnpoints
/// </summary>
public abstract class SpawnStrategy : ScriptableObject
{
   public abstract Transform NextSpawnPoint();
    public abstract void SetPoints(Transform[] points);
}
