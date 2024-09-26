using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HoldGroundState", menuName = "AI/State/HoldGround")]
public class HoldGroundState : State
{
   
    [SerializeField] float coverSearchRange = 10f;
    [SerializeField] LayerMask coverMask;
    [SerializeField] int maxAttempts = 10;
    [SerializeField] float holdCoverCooldown = 15f;
    [SerializeField] State idleState;
    [SerializeField] AreaType areaType = AreaType.None;
    public override void EnterState(AIController controller)
    {
        controller.Agent.stoppingDistance = .2f;
        controller.PositionRoutine = controller.StartCoroutine(controller.GetOpenCoverPosition(coverSearchRange, coverMask, maxAttempts,areaType));    
    }
    public override void UpdateState(AIController controller)
    {

        if (controller.PathFound == true)
        {
            //controller.lookTarget = controller.playerPos;
            controller.PathFound = false;      
            controller.HoldCoverCooldownTimer = holdCoverCooldown;
            controller.TransitionToState(idleState);
        }
    }
    public override void ExitState(AIController controller)
    {
           
        
    }
   
}
public enum AreaType
{
    None,
    Player,
    Defensive,
}