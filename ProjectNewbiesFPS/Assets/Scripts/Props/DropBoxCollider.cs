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
            audioManager.playSFX(audioManager.dropBox[Random.Range(0,audioManager.dropBox.Length)]);
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
