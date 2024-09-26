using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachment : MonoBehaviour
{
    public enum AttachTypes
    {
        SILENCER,
        MAGAZINE,
        SCOPE,

        ATTACHMENT_TOTAL
    }

    [SerializeField] AttachTypes attachType;
    [Header("-----Static Increases-----")]
    [Range(-5, 5)][SerializeField] int flatDamage;
    [Range(-5, 5)][SerializeField] int flatRange;
    [Header("-----Multiplicative Increases-----")]
    [Range(-5, 5)][SerializeField] int modDamage;
    [Range(-5, 5)][SerializeField] int modRange;
   
    [SerializeField] Attachment[] attachTypes;


    public AttachTypes GetAttachType()
    {
        return attachType;
    }

    public int GetFlatDamage()
    {
        return flatDamage;
    }

    public int GetFlatRange()
    {
        return flatRange;
    }
}
