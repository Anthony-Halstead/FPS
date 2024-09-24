using UnityEngine;
[CreateAssetMenu(fileName = "SweepAttackState", menuName = "AI/State/SweepAttack")]
public class SweepAttackState : State
{
    public State idleState;
    [SerializeField] float sweepCooldown = 10f;
    [SerializeField] float sweepSpeed = 5f;
    [SerializeField] float sweepAngle = 45f;


    public override void EnterState(AIController controller)
    {

        //controller.lookTarget = controller.DefaultRigTarget.position;
        controller.SetDualWeildRig();
        controller.StartCoroutine(controller.SweepAttack(CalculateSweepPoints(controller),sweepSpeed));
    }
    public override void UpdateState(AIController controller)
    {
        if (!controller.IsShooting)
        {
            controller.SweepCooldownTimer = sweepCooldown;
            controller.TransitionToState(idleState);
        }
            
    }
    public override void ExitState(AIController controller)
    {

    }
    private Vector3[] CalculateSweepPoints(AIController controller)
    {
        Vector3[] points = new Vector3[2];

        Vector3 playerDir = new Vector3(controller.PlayerDirection.x, 0, controller.PlayerDirection.z);

        Vector3 rightPoint = Quaternion.Euler(0, sweepAngle / 2, 0) * controller.PlayerDirection;
        Vector3 leftPoint = Quaternion.Euler(0,-sweepAngle/2,0)* controller.PlayerDirection;

        Vector3 pointOne = controller.transform.position + rightPoint;
   
        Vector3 pointTwo = controller.transform.position + leftPoint;
    
        points[0] = pointOne;
        points[1] = pointTwo;
        return points;
    }  
}
