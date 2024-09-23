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
    [SerializeField] AudioSource movementSource;
    [SerializeField] AudioSource enemySource;

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
    public AudioClip jump;

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
    public AudioClip land;
    public AudioClip playerHurt;
    public AudioClip enemyHurt;
    public AudioClip dropBox;
    public AudioClip UpgradePickUp;
    public AudioClip checkPointTrigger;

    [Header("------------------- UI SFX")]
    public AudioClip menuUp;
    public AudioClip menuDown;
    public AudioClip menuClick;
    public AudioClip menuWin;
    public AudioClip menuLose;
    public AudioClip menuSlider;

    [Header("------------------- Stats")]
    public float walkSpeedMod;
    public float sprintSpeedMod;

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

    public void playSFX(AudioClip clip, bool Loop = false)
    {
        sfxSource.clip = clip;
        
        if(Loop)
        {
            //Loops
            sfxSource.loop = true;
            sfxSource.Play();
        } else
        {
            //Plays once
            sfxSource.PlayOneShot(clip);
        }
    }

    public void playMove(AudioClip clip, bool Loop = false)
    {
        movementSource.clip = clip;

        if (Loop)
        {
            //Loops
            movementSource.loop = true;
            movementSource.Play();
        }
        else
        {
            //Plays once
            movementSource.PlayOneShot(clip);
        }
    }
    
    public void stopSFXLoop(bool stopNow = true)
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

    public void stopMoveLoop(bool stopNow = true)
    {
        //Same as the other stopLoop method
        movementSource.loop= false;

        if (stopNow)
        {
            movementSource.Stop();
        }
    }

    public void playEnemy(AudioClip clip, bool Loop = false)
    {
        //Enemy Sounds (Should cover all without problems)
        enemySource.clip = clip;

        if (Loop)
        {
            enemySource.loop = true;
            enemySource.Play();
        } else
        {
            enemySource.PlayOneShot(clip);
        }
    }

    public void stopEnemy(bool stopNow = true)
    {
        enemySource.loop = false;

        if (stopNow)
        {
            enemySource.Stop();
        }
    }
}
