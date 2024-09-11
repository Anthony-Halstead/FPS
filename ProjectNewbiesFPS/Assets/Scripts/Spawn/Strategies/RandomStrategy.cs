using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Used to randomly select spawn transforms this should be placed on the spawner of your choice
/// </summary>
[CreateAssetMenu(fileName = "RandomStrategy", menuName = "SpawnStrategies/Random")]
public class RandomStrategy : SpawnStrategy
{
    List<Transform> unusedSpawnPoints;
    Transform[] spawnPoints;

    public override void SetPoints(Transform[] points)
    {
        spawnPoints = points;
        unusedSpawnPoints = new List<Transform>(spawnPoints);
    }

    public override Transform NextSpawnPoint()
    {
        if (!unusedSpawnPoints.Any())
        {
            unusedSpawnPoints = new List<Transform>(spawnPoints);
        }

        var randomIndex = Random.Range(0, unusedSpawnPoints.Count);
        Transform result = unusedSpawnPoints[randomIndex];
        unusedSpawnPoints.RemoveAt(randomIndex);
        return result;
    }

  
}
