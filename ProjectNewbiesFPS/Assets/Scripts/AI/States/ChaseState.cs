using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(fileName = "ChaseState", menuName = "AI/State/Chase")]
public class ChaseState : State
{
    public State searchState;
    public State attackState;
    public override void EnterState(AIController controller)
    {
        controller.lastSeenPlayerPos = controller.playerPos;
        controller.target = GameManager.instance.player.transform.position;
        controller.lookTarget = GameManager.instance.player.transform.position;
        controller.StartRig();
    }
    public override void UpdateState(AIController controller)
    {
        if (!controller.TargetIsVisible())
        {
            controller.target = controller.lastSeenPlayerPos;
            controller.lookTarget = controller.lastSeenPlayerPos;
            controller.TransitionToState(searchState);
        }
        if (controller.TargetIsVisible() && controller.TargetInShootRange()) 
        {
            controller.TransitionToState(attackState);
        }
    }
    public override void ExitState(AIController controller)
    {
        
    }

   
}
