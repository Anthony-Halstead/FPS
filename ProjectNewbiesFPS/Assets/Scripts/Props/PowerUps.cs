using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour, IPowerUps
{
    public GameObject player;
    // public PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        //  controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other == player)
        {
            Destroy(gameObject);
        }
    }
}
