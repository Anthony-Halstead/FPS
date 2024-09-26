using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SearchState", menuName = "AI/State/Search")]
public class SearchState : State
{
    [SerializeField] float timeToWait = 1f;
    [Min(0),SerializeField, Tooltip("How long should the AI stay in the search state?")] float searchLength = 20f;
    
    public override void EnterState(AIController controller)
    {
        controller.target = controller.lastSeenPlayerPos;
        controller.lookTarget = controller.lastSeenPlayerPos;
        controller.StartCoroutine(controller.SearchSpot(timeToWait, searchLength));
    }
    public override void UpdateState(AIController controller)
    {
       
        if(!controller.IsSearching)
        {
            controller.TransitionToState(controller.DefaultState);
        }
        if (controller.TargetIsVisible())
        {
            controller.TransitionToState(controller.PreviousState);
        }
    }
    public override void ExitState(AIController controller) {}  
}
