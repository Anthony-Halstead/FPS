using UnityEngine;
[CreateAssetMenu(fileName = "AttackState", menuName = "AI/State/Attack")]
public class AttackState : State
{
    [SerializeField] AttackType type;

    public override void EnterState(AIController controller)
    {
        /*   if (type == AttackType.DualWield)
               controller.SetDualWieldRig();
           else if (type == AttackType.Default)
                 controller.StartRig();*/
        //  controller.lookTarget = controller.playerPos;
        controller.lookTarget = controller.playerPos;
        controller.StartCoroutine(controller.Shoot());
    }
    public override void UpdateState(AIController controller)
    {
     
        if (!controller.IsShooting)
            controller.TransitionToState(controller.PreviousState);
    }
    public override void ExitState(AIController controller)
    {
       // controller.StopRig();
    }

   
}
public enum AttackType
{
    Default,
    DualWield,
}