using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class grenadeScripty : MonoBehaviour
{
    [SerializeField] float explosionRadius;
    public GameObject explosionFX;
    [SerializeField] float explostionForce;
    [SerializeField] int explosionDmg;

    public float explosionDelay = 3f;
    float cd;
    bool exploded = false;

    // Start is called before the first frame update
    void Start()
    {
        cd = explosionDelay;
    }

    // Update is called once per frame
    void Update()
    {
        cd -= Time.deltaTime;       //grenade delay before exploding
        if (cd <= 0f && !exploded)  //checks to make sure it didnt explode already
        {
            explode();              //calls the explode function and sets exploded to true
            exploded = true;
        }
    }

    void explode()
    {
        //Instantiate(explosionFX, transform.position, transform.rotation); this is for if and when we get an effect for the grenade explosion

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);  //checks for all the overlapping things in the provided radius of the grenade

        foreach (Collider nearbyObject in colliders)    //checks each nearby overlap sphere collider in the colliders array
        {
            if (nearbyObject.CompareTag("Enemy") == true)   // if that collider has the Enemy tag it deals damage to it based on the number of damage we want to deal to it
            {
                Debug.Log("ENEMY SPOTTED");
                IDamage dmg = nearbyObject.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.TakeDamage(explosionDmg, transform.position);   // deals X amount of dmg
                }
            }
        }
        Destroy(gameObject);    // destroys the grenade afterwards
    }
}
