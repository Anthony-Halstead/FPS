using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    [Header("------------------- Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("------------------- Music/Backgrounds")]
    public AudioClip musicForest;
    public AudioClip musicTown;
    public AudioClip musicIndustrial;
    public AudioClip musicBoss;

    [Header("------------------- Character movement")]
    public AudioClip footStepWalking;
    public AudioClip footStepRunning;
    public AudioClip crouchDown;
    public AudioClip crouchUp;

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


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        //Play Music
        musicSource.clip = musicForest;
        musicSource.Play();
    }

    public void playSFX(AudioClip clip)
    {
        //Play Sound effect once (can play over itself)
        sfxSource.clip = clip;
        sfxSource.PlayOneShot(clip);
    }
    public void playLoop(AudioClip clip, bool canLap = true)
    {
        //Loop Sound effect
        sfxSource.clip = clip;
        sfxSource.loop = true;
        
        if(canLap)
        {
            //If true PlayOneShot()
            sfxSource.PlayOneShot(clip);
        } else
        {
            //If false Play()
            sfxSource.Play();
        }
    }
    public void stopLoop(bool stopNow = true)
    {
        //Stop a looped sound
        sfxSource.loop = false;

        if (stopNow)
        {
            //If true will stop the sound instantly
            sfxSource.Stop();
        }
        //Else will stop when its done playing
    }
}
