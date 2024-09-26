using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState",menuName ="AI/State/Idle")]
public class IdleState : State
{
    public State chaseState;
    public State dodgeState;
    public State sweepState;
    public State holdGroundState;
    public State attackState;
    public State spawnReinforcement;
    public override void EnterState(AIController controller)
    {
        controller.Agent.stoppingDistance = 0.2f;
        // controller.Agent.angularSpeed = 0f;
        /*controller.target = controller.transform.position;
        controller.lookTarget = controller.transform.forward;*/

    }
    public override void UpdateState(AIController controller)
    {
        if(holdGroundState!=null&& controller.HoldCoverCooldownTimer <= 0)
        {
            controller.TransitionToState(holdGroundState);
        }
        if (chaseState != null && !controller.IsShooting && controller.IsTakingDamage && controller.TargetIsVisible())
        {
            controller.TransitionToState(chaseState);
        } 

        if (dodgeState != null && controller.DodgeCooldownTimer <= 0 && controller.IsTakingDamage)
        {
            controller.PreviousState = this; 
            controller.TransitionToState(dodgeState);
        }
        if(sweepState != null && controller.SweepCooldownTimer <= 0 && controller.TargetIsVisible())
        {
            controller.PreviousState = this;
            controller.TransitionToState(sweepState);
        }
        if (attackState != null && controller.TargetIsVisible() && controller.TargetInShootRange() && !controller.IsShooting)
        {
            controller.TransitionToState(attackState);
        }
        if (spawnReinforcement != null && controller.IsTakingDamage && controller.SpawnReinforcementCooldownTimer <= 0)
        {
            controller.PreviousState = this;
            controller.TransitionToState(spawnReinforcement);
        }
    }
    public override void ExitState(AIController controller)
    {
   
    }
}
