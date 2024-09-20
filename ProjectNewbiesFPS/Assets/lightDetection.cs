using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightDetection : MonoBehaviour
{
    private bool playerInRange;
    private PlayerController player;
    
    [SerializeField] private float minVisDist;
    [SerializeField] private float maxVisDist; 

    [SerializeField] private Light light;
    [SerializeField] private SphereCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        collider = GetComponent<SphereCollider>();

        if (light.type == LightType.Point)
        {
            collider.radius = light.range;
        }
        
        maxVisDist = collider.radius;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (playerInRange && playerVisible())
        {
            float playerDistance = Vector3.Distance(collider.bounds.center, player.transform.position);
            float lightPercent = Mathf.InverseLerp(maxVisDist, minVisDist, playerDistance);
            player.modPlayerVisibility(lightPercent);
            Debug.Log("Light contribution: " + lightPercent);
        }
    }

    bool playerVisible()
    {
        RaycastHit hit;
        Vector3 playerDirection = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, playerDirection, out hit, light.range))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.GetComponent<PlayerController>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }
}
