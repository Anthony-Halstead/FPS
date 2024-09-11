using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBoxCollider : MonoBehaviour
{
    public AudioManager audioManager;
   

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Floor"))
    //    {
    //        isGrounded = true;
    //    }
    //}

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            audioManager.playSFX(audioManager.dropBox);
            Debug.Log("Collided");

            //Activate Animation

            GameManager.instance.InstantiateUpgrades();

            Destroy(gameObject);

        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Floor"))
    //    {
    //        isGrounded = true;
    //    }
    //}
}
