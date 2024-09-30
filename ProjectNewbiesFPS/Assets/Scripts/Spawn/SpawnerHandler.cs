using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
/// <summary>
/// Represents the concrete spawner that is placed in the game world
/// </summary>
public class SpawnerHandler : MonoBehaviour
{
    public static Action<Transform[]> OnSendSpawner;
    [Tooltip("Place any spawn strategy scriptable object here"),SerializeField] SpawnStrategy strategyTemplate;
    [Tooltip("Represents a timer that will be instanced from a scriptable object for this spawner specifically"),SerializeField] Timer timerTemplate;
    [Tooltip("Place any objects that derive from the Spawnable class here, these will be what is spawned"),SerializeField] Spawnable[] objsData;
    [Tooltip("Represents the length of time between each spawned object"),SerializeField] float spawnInterval = 1f;
    [Tooltip("Place the child spawn points transforms from this spawner here"),SerializeField] Transform[] spawnPoints;
    [Tooltip("Used to determine how the positioning of the spawnpoints behave. None means the transforms do not change from where they were positioned in the scene" +
        "Navmesh means the transforms are placed on the closest navmesh point from its default position."),SerializeField] SpawnPositionType spawnPositionType = SpawnPositionType.None;

    Spawner<Spawnable> spawner;
    Timer timerInstance;
    private bool hasSpawned;
    
    int counter;

    public void Awake()
    {
        SpawnStrategy instance = Instantiate(strategyTemplate);
        instance.SetPoints(spawnPoints);
        spawner = new Spawner<Spawnable>(new Factory<Spawnable>(objsData), instance);
        timerInstance = Instantiate(timerTemplate);

        timerInstance.OnStopTimer += () =>
        {
            if (counter++ >= spawnPoints.Length)
            {
                    timerInstance.StopTimer(this);
            }
            Spawn();
            timerInstance.StartTimer(this, spawnInterval);
        };

        if (spawnPositionType == SpawnPositionType.NavMesh)
            OnSendSpawner.Invoke(spawnPoints);
    }

  

    private void OnEnable() => timerInstance.StartTimer(this,spawnInterval);
    /// <summary>
    /// Spawns an instance of an object
    /// </summary>
    public void Spawn() => spawner.Spawn();
    /// <summary>
    /// Triggered from the spawn manager when somethin tells the spawn manager to trigger a timer/spawn attempt
    /// </summary>
    public void TriggerTimer() => timerInstance.StartTimer(this, spawnInterval);
}
public enum SpawnPositionType
{
    None,
    NavMesh,
}