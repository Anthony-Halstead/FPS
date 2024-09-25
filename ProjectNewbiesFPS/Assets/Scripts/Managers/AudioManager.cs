using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    [Range(0, 1)] public float musicVol;

    [Header("------------------- Character movement")]
    public AudioClip[] footStepsForest;
    public AudioClip[] footStepsWood;
    public AudioClip[] footStepsConcrete;
    public AudioClip[] footStepsRocky;
    [Range(0, 1)] public float footStepsVol;
    public AudioClip crouchDown;
    public AudioClip crouchUp;
    [Range(0, 1)] public float crouchVol;
    public AudioClip[] jump;
    [Range(0, 1)] public float jumpVol;

    [Header("------------------- Ranged SFX")]
    public AudioClip[] explosion;         //Bombs/traps
    [Range(0, 1)] public float explosionVol;
    public AudioClip[] playerThrow;
    [Range(0, 1)] public float throwVol;
    public AudioClip enemyShoot;
    [Range(0, 1)] public float enemyShootVol;

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
    [Range(0, 1)] public float trapVol;
    public AudioClip[] landForest;
    public AudioClip[] landWood;
    public AudioClip[] landConcrete;
    public AudioClip[] landRocky;
    [Range(0, 1)] public float landVol;
    public AudioClip[] hurt;
    [Range(0, 1)] public float hurtVol;
    public AudioClip[] dropBox;
    public AudioClip[] pickUp;
    [Range(0, 1)] public float pickUpVol;
    public AudioClip checkPointTrigger;
    [Range(0, 1)] public float checkPointVol;

    [Header("------------------- UI SFX")]
    public AudioClip menuUp;
    public AudioClip menuDown;
    public AudioClip menuClick;
    public AudioClip menuWin;
    public AudioClip menuLose;
    public AudioClip menuSlider;
    [Range(0, 1)] public float menuVol;

    [Header("------------------- Stats")]
    [Range(0, 1)] public float walkSpeedMod;
    [Range(0, 1)] public float sprintSpeedMod;

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

    public void playSFX(AudioClip clip, float vol = 0.5f, bool Loop = false)
    {
        sfxSource.clip = clip;
        
        if(Loop)
        {
            //Loops
            sfxSource.loop = true;
            sfxSource.volume = vol;
            sfxSource.Play();
        } else
        {
            //Plays once
            sfxSource.PlayOneShot(clip, vol);
        }
    }

    public void playMove(AudioClip clip, float vol = 0.5f, bool Loop = false)
    {
        movementSource.clip = clip;

        if (Loop)
        {
            //Loops
            movementSource.loop = true;
            movementSource.volume = vol;
            movementSource.Play();
        }
        else
        {
            //Plays once
            movementSource.PlayOneShot(clip, vol);
        }
    }

    public void playEnemy(AudioClip clip, float vol = 0.5f, bool Loop = false)
    {
        //Enemy Sounds (Should cover all without problems)
        enemySource.clip = clip;

        if (Loop)
        {
            enemySource.loop = true;
            enemySource.volume = vol;
            enemySource.Play();
        }
        else
        {
            enemySource.PlayOneShot(clip, vol);
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

    public void stopEnemy(bool stopNow = true)
    {
        enemySource.loop = false;

        if (stopNow)
        {
            enemySource.Stop();
        }
    }
}
