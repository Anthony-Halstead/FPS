using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [SerializeField] private WeaponObject _weaponObject;
    
    [SerializeField] private float gunSpeed;
    [SerializeField] private int shootDamage;
    [SerializeField] private int shootDist;
    [SerializeField] private float shootRate;
    [SerializeField] private int magazineSize;

    [SerializeField] private Transform firePoint;

    public string interactionText;
    // Start is called before the first frame update
    void Start()
    {
        gunSpeed = _weaponObject.fireSpeed;
        shootDamage = _weaponObject.dmg;
        shootDist = _weaponObject.dist;
        shootRate = _weaponObject.rate;
        magazineSize = _weaponObject.magazineCount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetFirePoint()
    {
        return firePoint;
    }

    public float GetGunSpeed()
    {
        return gunSpeed;
    }

    public int GetGunDamage()
    {
        return shootDamage;
    }

    public int GetShootDist()
    {
        return shootDist;
    }

    public float GetShootRate()
    {
        return shootRate;
    }

    public float GetMagazineSize()
    {
        return magazineSize;
    }

    public WeaponObject GetWeaponObject()
    {
        return _weaponObject;
    }

    public void Interact()
    {
        Destroy(gameObject);
    }
}
