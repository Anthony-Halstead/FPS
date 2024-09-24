using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState",menuName ="AI/State/Idle")]
public class IdleState : State
{
    public State chaseState;
    public State dodgeState;
    public State sweepState;
    public override void EnterState(AIController controller)
    {
        controller.StopRig();
        controller.Agent.angularSpeed = 0f;
        controller.target = controller.transform.position;
        controller.lookTarget = controller.transform.forward;
        
    }
    public override void UpdateState(AIController controller)
    {
        if (chaseState != null && !controller.IsShooting && controller.IsTakingDamage && controller.TargetIsVisible())
        {
            controller.TransitionToState(chaseState);
        } 
        if (dodgeState != null && controller.DodgeCooldownTimer <= 0 && controller.IsTakingDamage)
        {
            controller.TransitionToState(dodgeState);
        }
        if(sweepState != null && controller.SweepCooldownTimer <= 0 && controller.TargetIsVisible())
        {
            controller.TransitionToState(sweepState);
        }
    }
    public override void ExitState(AIController controller)
    {
   
    }
}
