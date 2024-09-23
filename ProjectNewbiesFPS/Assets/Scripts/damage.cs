using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, melee }
    
    [SerializeField] private damageType type;
    [SerializeField] private Rigidbody rb;

    public int damageAmount;
    [SerializeField] private int speed;
    [SerializeField] private int destroyTime;

    [SerializeField] private bool shouldTrack;

    private void Start()
    {
        if (type == damageType.moving)
        {
            if (!shouldTrack)
            {
                rb.velocity = transform.forward * speed;
            }
            else
            {
                rb.velocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed;
            }
            
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        
        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.TakeDamage(damageAmount, Vector3.zero);
            
            if (type == damageType.moving)
            {
                Destroy(gameObject);
            }
        }


    }
}
