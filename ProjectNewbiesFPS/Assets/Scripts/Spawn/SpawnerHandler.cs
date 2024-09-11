using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    Spawner<Spawnable> spawner;
    Timer timerInstance;
    
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
                counter = 0;
                return;
            }
            Spawn();
            timerInstance.StartTimer(this,spawnInterval);
        };
        OnSendSpawner.Invoke(spawnPoints);
    }
    private void Start() => timerInstance.StartTimer(this,spawnInterval);
    /// <summary>
    /// Spawns an instance of an object
    /// </summary>
    public void Spawn() => spawner.Spawn();
    /// <summary>
    /// Triggered from the spawn manager when somethin tells the spawn manager to trigger a timer/spawn attempt
    /// </summary>
    public void TriggerTimer() => timerInstance.StartTimer(this, spawnInterval);
}
