using UnityEngine;
[CreateAssetMenu(fileName = "ChaseState", menuName = "AI/State/Chase")]
public class ChaseState : State
{
    public State searchState;
    public State attackState;
    public override void EnterState(AIController controller)
    {
        controller.lastSeenPlayerPos = controller.playerPos;
        controller.target = controller.playerPos;
        controller.lookTarget = controller.playerPos;
    }
    public override void UpdateState(AIController controller)
    {
        if (!controller.TargetIsVisible())
        {

            controller.PreviousState = this;
            controller.TransitionToState(searchState);
        }
        if (controller.TargetIsVisible() && controller.TargetInShootRange() && !controller.IsShooting) 
        {
            controller.TransitionToState(attackState);
        }
    }
    public override void ExitState(AIController controller)
    {
        
    }

   
}
