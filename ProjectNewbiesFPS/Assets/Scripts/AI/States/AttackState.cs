using UnityEngine;
[CreateAssetMenu(fileName = "AttackState", menuName = "AI/State/Attack")]
public class AttackState : State
{
    public State chaseState;
    public override void EnterState(AIController controller)
    {
       // controller.CanMove = false;
        controller.StartCoroutine(controller.shoot());
    }
    public override void UpdateState(AIController controller)
    {
        if (!controller.IsShooting)
            controller.TransitionToState(chaseState);
    }
    public override void ExitState(AIController controller)
    {
      //  controller.CanMove = true;
    }

   
}
