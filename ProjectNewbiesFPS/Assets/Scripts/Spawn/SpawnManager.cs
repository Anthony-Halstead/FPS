using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Place in scene to manage all spawn points
/// spawn points should have layer of Spawner
/// </summary>

public class SpawnManager : MonoBehaviour
{

    public static SpawnManager instance;
    [Tooltip("Used to determine the radius that the overlapsphere can detect to"),SerializeField] float checkRadius = 5;
    [SerializeField] LayerMask spawnLayerMask;
    [Tooltip("Used to determine the max distance to the navmesh each spawner should be in order for the spawn points to be moved to the navmesh")
   , SerializeField] float maxDistanceToNavMesh = 7f;

    SpawnerHandler[] sceneSpawnHandlers;


    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        SpawnerHandler.OnSendSpawner += MoveSpawnPointsToNavMesh;
    }
    private void OnDisable()
    {
        SpawnerHandler.OnSendSpawner -= MoveSpawnPointsToNavMesh;
    }
    private void Start()
    {
        sceneSpawnHandlers = FindObjectsByType<SpawnerHandler>(FindObjectsSortMode.None);
        
    }
    /// <summary>
    /// Sets the spawn points of each spawner to the navmesh useful for setting the AI directly on the navmesh
    /// </summary>
    /// <param name="spawnPoints"></param>
    public void MoveSpawnPointsToNavMesh(Transform[] spawnPoints)
    {
        NavMeshHit hit;
        foreach (var spawnPoint in spawnPoints) {
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, maxDistanceToNavMesh, NavMesh.AllAreas))
            {
                spawnPoint.position = hit.position;
            }
        }    
    }
    /// <summary>
    /// Call this trigger from any code to have the spawn points timers to start
    /// selected spawnpoints are based off of players position
    /// </summary>
    public void TriggerEnemySpawn()
    {
        Collider[] hitColliders = Physics.OverlapSphere(GameManager.instance.player.transform.position, checkRadius, spawnLayerMask,QueryTriggerInteraction.Collide);
        foreach(var col in hitColliders)
        {
            col.TryGetComponent(out SpawnerHandler spawner);
            if(spawner == null) continue;
            spawner.TriggerTimer();
        }
    }
    /// <summary>
    /// Call this trigger from anywhere to activeate all the spawn points to spawn the objects in it
    /// </summary>
    public void TriggerAllSpawnPoints()
    {
        foreach(var spawner in sceneSpawnHandlers)
        {
            spawner.TriggerTimer();
        }
    }
}
