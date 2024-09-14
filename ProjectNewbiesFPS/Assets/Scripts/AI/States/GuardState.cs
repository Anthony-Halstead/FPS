using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GaurdState",menuName ="AI/State/Guard")]
public class GuardState : State
{
    public State chaseState;
    public override void EnterState(AIController controller)
    {
     
    }
    public override void UpdateState(AIController controller)
    {
        if (controller.TargetIsVisible())
        {
            controller.TransitionToState(chaseState);
        } 
    }
    public override void ExitState(AIController controller)
    {
   
    }
}
