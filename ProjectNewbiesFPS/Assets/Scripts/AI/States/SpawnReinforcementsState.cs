using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SpawnReinforcementsState", menuName = "AI/State/SpawnReinforcements")]
public class SpawnReinforcementsState : State
{
    [SerializeField] private float spawnCooldown = 40f;
    [SerializeField] private float spawnDistanceCheck = 15f;
    public override void EnterState(AIController controller)
    {
        controller.CallReinforcements(spawnDistanceCheck);
    }
    public override void UpdateState(AIController controller)
    {
        controller.SpawnReinforcementCooldownTimer = spawnCooldown;
        controller.TransitionToState(controller.PreviousState);
    }
    public override void ExitState(AIController controller)
    {

    }
 
}
