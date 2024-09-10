using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------------------- Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("------------------- Music/Backgrounds")]
    public AudioClip bckGrndMusic;
    public AudioClip footStepWalking;
    public AudioClip footStepRunning;

    [Header("------------------- Ranged SFX")]
    public AudioClip explosion;         //Bombs/traps
    public AudioClip explosionLanding;
    public AudioClip shootPistol;
    public AudioClip shootMachineGun;
    public AudioClip shootShotGun;

    //Reloads
    public AudioClip reloadPistol;
    public AudioClip reloadMachineGun;
    public AudioClip reloadShotGun;
    public AudioClip shootBow;          //Shoot and reload

    [Header("------------------- Melee SFX")]
    public AudioClip meleeUnarmedHit;   //Fists
    public AudioClip meleeUnarmedMiss;
    public AudioClip meleeBluntHit;     //Bats, maces, bombs(if hit before explosion)
    public AudioClip meleeBluntMiss;
    public AudioClip meleeSharpHit;     //Sword, axes, daggers
    public AudioClip meleeSharpMiss; 
    public AudioClip meleePierce;       //Spears, arrows, darts, throwing stars

    [Header("------------------- Potions")]
    public AudioClip potionHealth;

    [Header("------------------- Misc")]
    public AudioClip trapPlace;
    public AudioClip trapActivate;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip takeDmg;

    public void Awake()
    {
        musicSource.clip = bckGrndMusic;
        musicSource.Play();
    }
}
