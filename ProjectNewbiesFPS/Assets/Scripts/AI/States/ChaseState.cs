using UnityEngine;
[CreateAssetMenu(fileName = "ChaseState", menuName = "AI/State/Chase")]
public class ChaseState : State
{
    [SerializeField] float giveUpChaseDistance = 40f;
    public State searchState;
    public State attackState;
    public State spawnReinforcement;
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
        if(spawnReinforcement != null && controller.IsTakingDamage && controller.SpawnReinforcementCooldownTimer <= 0)
        {
            controller.PreviousState = this;
            controller.TransitionToState(spawnReinforcement);
        }
        if (controller.TargetIsVisible() && controller.TargetInShootRange() && !controller.IsShooting) 
        {
            controller.PreviousState = this;
            controller.TransitionToState(attackState);
        }
        if (!controller.TargetIsVisible() && !controller.TargetInShootRange() && Vector3.Distance(controller.transform.position, controller.playerPos) >= giveUpChaseDistance)
        {
            controller.TransitionToState(controller.DefaultState);
        }
    }
    public override void ExitState(AIController controller)
    {
        
    }

   
}
