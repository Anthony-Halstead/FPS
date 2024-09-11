using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Represents the abstract Spawner which can be created via a constructor
/// </summary>
/// <typeparam name="T"></typeparam>
public class Spawner<T> where T : Spawnable
{
   IFactory<T> factory;
   SpawnStrategy spawnPointStrategy;

    public Spawner(IFactory<T> factory, SpawnStrategy spawnPointStrategy)
    {
        this.factory = factory;
        this.spawnPointStrategy = spawnPointStrategy;
    }

    public T Spawn()
    {
        return factory.Create(spawnPointStrategy.NextSpawnPoint());
    }
}
