using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(fileName = "DodgeState", menuName = "AI/State/Dodge")]
public class DodgeState : State
{
    public State guardState;
    [SerializeField] private float dodgeDistance = 5;
    [SerializeField] private float dodgeCooldown = 10f;
    public override void EnterState(AIController controller)
    {
        controller.positionRoutine = controller.StartCoroutine(controller.GetRandomClearPositionInRange(dodgeDistance));
    }
    public override void UpdateState(AIController controller)
    {
        if (controller.PathFound)
        {
            controller.Anim.SetTrigger("Dodge");
            controller.PathFound = false;
        }
        if (controller.CanTransition)
        {
            controller.CanTransition = false;
            controller.DodgeCooldownTimer = dodgeCooldown;
            controller.TransitionToState(guardState);
        }     
    }
    public override void ExitState(AIController controller)
    {
       
    }


}
