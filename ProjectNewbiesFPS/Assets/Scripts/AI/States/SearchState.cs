using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SearchState", menuName = "AI/State/Search")]
public class SearchState : State
{
    public State chaseState;

    public override void EnterState(AIController controller)
    {
       controller.searchRoutine = controller.StartCoroutine(controller.SearchSpot());
    }
    public override void UpdateState(AIController controller)
    {
        controller.CurrentTime += Time.deltaTime;
        if(controller.CurrentTime >= controller.SearchLength)
        {
            controller.CurrentTime = 0;
            controller.TransitionToState(controller.DefaultState);
        }
        if (controller.TargetIsVisible())
        {
            controller.CurrentTime = 0;
            controller.TransitionToState(chaseState);
        }
        if(controller.searchRoutine == null)
        {
            controller.searchRoutine = controller.StartCoroutine(controller.SearchSpot());
        }

    }
    public override void ExitState(AIController controller)
    {
        controller.StopAllCoroutines();
    }

    
}
