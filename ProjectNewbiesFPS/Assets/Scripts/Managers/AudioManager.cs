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
    public AudioClip shootBow;          //Shoot and reload

    //Reloads
    public AudioClip reloadPistol;
    public AudioClip reloadMachineGun;
    public AudioClip reloadShotGun;

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
    public AudioClip playerHurt;
    public AudioClip enemyHurt;
    public AudioClip dropBox;
    public AudioClip UpgradePickUp;

    [Header("------------------- UI SFX")]
    public AudioClip menuUp;
    public AudioClip menuDown;
    public AudioClip menuClick;
    public AudioClip menuWin;
    public AudioClip menuLose;
    public AudioClip menuSlider;

    public void Start()
    {
        musicSource.clip = bckGrndMusic;
        musicSource.Play();
    }

    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
