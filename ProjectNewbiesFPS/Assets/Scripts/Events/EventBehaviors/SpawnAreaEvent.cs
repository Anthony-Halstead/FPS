using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SpawnAreaEvent", menuName = "EventTriggers/SpawnTriggers/SpawnAreaEvent")]
public class SpawnAreaEvent : EventBehavior
{
    [SerializeField] int spawnIndex;
    public override void Trigger(Context context)
    {
        SpawnManager.instance.TriggerSpawnArea(spawnIndex);
    }
}
