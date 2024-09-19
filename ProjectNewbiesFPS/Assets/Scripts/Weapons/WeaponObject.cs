using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon", fileName = "Weapon")]
public class WeaponObject : ScriptableObject
{
    public GameObject prefab;
    public GameObject prefab_rb;

    public Sprite sprite;

    public string name;

    public int dmg;
    public int dist;
    public float rate;
    public float fireSpeed;
    public int magazineCount;

    public AudioClip[] shootSFX;
    public int shootVol;
}
