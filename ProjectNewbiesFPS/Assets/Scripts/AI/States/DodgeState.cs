using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(fileName = "DodgeState", menuName = "AI/State/Dodge")]
public class DodgeState : State
{

    [SerializeField] private float dodgeDistance = 5;
    [SerializeField] private float dodgeCooldown = 10f;
    public override void EnterState(AIController controller)
    {
        controller.Agent.stoppingDistance = 0f;
        controller.IsDodging = true;
        controller.HasDodged = false;
        controller.StartCoroutine(controller.GetDodgePositionInRange(dodgeDistance));     
    }
    public override void UpdateState(AIController controller)
    {
         if(controller.PathFound == true && !controller.HasDodged)
         {
            controller.Anim.SetTrigger("Dodge");
            controller.HasDodged = true;
         }
            

            if (!controller.IsDodging)
            {
                controller.DodgeCooldownTimer = dodgeCooldown;
                controller.PathFound = false;
                controller.TransitionToState(controller.PreviousState);
            }
        
           
    }
    public override void ExitState(AIController controller){}
}
