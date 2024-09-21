using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
/// <summary>
/// Used to detect the player interacting with some trigger which then fires an event. 
/// It is recommended that any events placed in the evts field are tightly related.
/// For isntance an animation,partical effect and audio all being triggered together.
/// Place this script on the trigger object. 
/// </summary>
public class TriggerEvent : MonoBehaviour
{
    [Tooltip("Represents the object that will be effected by the trigger"), SerializeField] GameObject obj;
    [Tooltip("Place any events you want to be fired here."),SerializeField] EventBehavior[] evts;
    [Tooltip("Used to detect range of spawners, if you are not attempting to get any spawners set this to 0 " +
        "so the GetNearbySpawners method does not fire"),SerializeField] float spawnerCheckRange = 0f;
    [Tooltip("Represents the number of times these events can be triggered"),Range(1,100), SerializeField] int triggerCounter = 1;
    private Animator animator;
    private List<SpawnerHandler> handlers = new List<SpawnerHandler>();
    private void Start()
    {
        if(obj != null)
        {
            //If dynamic data is needeed following the trigger of the event instantiate new scriptable object instance here
            animator = obj.GetComponentInParent<Animator>();
            if (spawnerCheckRange > 0)
                handlers = GetNearbySpawners();
        }
        else
        {
            Debug.LogError("An object that is being effected must be placed in the obj field");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerCounter > 0)
        {
            Context context = new Context
            {
                animator = animator,
                handlers = handlers,
            };
            if(evts.Count() > 0)
            {
                foreach (var evt in evts)
                {
                    evt.Trigger(context);
                }
            }
            triggerCounter--;
        }
    }
    //TODO Impliment this when needed (OnTriggerExit)
    /*   private void OnTriggerExit(Collider other)
       {
                    triggerCounter--;
       }*/
    //TODO impliment this when needed (Manual Trigger)
    /*public void ManualTrigger()
    {
                    triggerCounter--;
    }*/
    private List<SpawnerHandler> GetNearbySpawners()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, spawnerCheckRange);

        foreach(var col in colliders)
        {
            SpawnerHandler handler = col.GetComponent<SpawnerHandler>();
            if(handler != null)
            {
                handlers.Add(handler);
            }
        }
        return handlers;
    }
}
public struct Context
{
   public Animator animator;
   public List<SpawnerHandler> handlers; 
}
