using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    private Vector2 input;

    // Update is called once per frame
    void Update()
    {
        swayWeapon();
    }

    void getInput()
    {
        // get the horizontal input
        input.x = Input.GetAxis("Horizontal");
        
        // get the vertical input
        input.y = Input.GetAxis("Vertical");
    }

    void swayWeapon()
    {
        // get input of player
        getInput();
        
        // for accessibility concerns, check global toggle for if sway effects are enabled (for motion sickness/personal preference purposes)
        
        // if (GameManager.instance.gameSettings.swayEffectsEnabled) {
        
        // rotate this to match movement input
        
        // interpolate between initial position and final position, which should be based on an amplitude
        // so that we can control how strong the sway is, instead of hard coding in a rotation.
        
        // }
    }
}
