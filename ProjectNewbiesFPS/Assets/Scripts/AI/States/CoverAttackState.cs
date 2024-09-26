using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CoverAttackState", menuName = "AI/State/CoverAttack")]
public class CoverAttackState : State
{
    [SerializeField] float shootingLength = 2f;
    [Range(0,2),SerializeField] float coverPositionOffsetDistance = .2f;
    [SerializeField] AttackType type = AttackType.Default;
    public override void EnterState(AIController controller)
    {
        if (type is AttackType.DualWield)
            controller.SetDualWieldRig();
        else
            controller.StartRig();
        controller.StartCoroutine(controller.ShootFromCover(shootingLength,coverPositionOffsetDistance));
    }
    public override void UpdateState(AIController controller)
    {
        if(!controller.IsShooting)
            controller.TransitionToState(controller.PreviousState);
    }
    public override void ExitState(AIController controller){ 
        controller.StopRig(); }

 
}
