using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationRigController : MonoBehaviour
{
    [SerializeField] Rig rig;

    private void Awake()
    {
        rig ??= GetComponentInChildren<Rig>();
    }
    private void Start()
    {
       
    }
    public void StartRig()
    {
        rig.weight = 1.0f;
    }
    public void StopRig()
    {
        rig.weight = 0f;
    }
}
