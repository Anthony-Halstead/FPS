using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PatrolState", menuName = "AI/State/Patrol")]
public class PatrolState : State
{
    public State chaseState;
    public override void EnterState(AIController controller)
    {
        controller.Agent.stoppingDistance = 1f;
        controller.StartCoroutine(controller.Patrol());
    }
    public override void UpdateState(AIController controller)
    {
     
        if (chaseState != null && (controller.IsTakingDamage || controller.TargetIsVisible()))
        {
            controller.TransitionToState(chaseState);
        }
    }
    public override void ExitState(AIController controller)
    {
        
    }

  
}
