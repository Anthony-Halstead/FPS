using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Used to select spawn transforms in order from top to bottom for objects to be spawned from, this should be placed on the spawner of your choice
/// </summary>
[CreateAssetMenu(fileName = "SequentialStrategy", menuName = "SpawnStrategies/Sequential")]
public class SequentialStrategy : SpawnStrategy
{
    int index = 0;
    Transform[] spawnPoints;

    public override void SetPoints(Transform[] points)
    {
        spawnPoints = points;
    }
    public override Transform NextSpawnPoint()
    {
        Transform result = spawnPoints[index];
        index = (index+1) % spawnPoints.Length;
        return result;
    }


}
