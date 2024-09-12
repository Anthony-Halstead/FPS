using System;
using System.Collections;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

/// <summary>
/// Timer class scriptable object, by default this runs like a countdown timer. 
/// However, it can be overriden to provide functions for other types of timers as well
/// </summary>
[CreateAssetMenu(fileName = "DefaultTimer", menuName = "Timers/Default")]
public class Timer : ScriptableObject
{
    protected float duration;
    protected float elapsedTime;
    protected bool isRunning;

    protected Coroutine timer;
    public Action OnStartTimer = delegate { };
    public Action OnStopTimer = delegate { };
    public virtual void StartTimer<T>(T caller, float duration) where T : MonoBehaviour
    {
        if(timer != null)
        {
            caller.StopCoroutine(timer);
        }

        this.duration = duration;
        elapsedTime = 0;
        isRunning = true;
        OnStartTimer.Invoke();
        timer = caller.StartCoroutine(TimerCoroutine());
    }
    public virtual void StopTimer<T>(T caller) where T : MonoBehaviour
    {
        if(timer != null)
        {
            caller.StopCoroutine(timer);
            timer = null;   
        }
        isRunning = false;
        elapsedTime = 0;
    }
    public virtual IEnumerator TimerCoroutine() 
    {
       
        while(elapsedTime < duration)
        {
            if (isRunning)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
        OnStopTimer.Invoke();
    }

}
