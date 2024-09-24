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
  
    [SerializeField] private float reloadTime;

    [SerializeField] private Transform firePoint;

    [SerializeField] private int startingAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int currentClip;
    [SerializeField] private int maxAmmo;

    public string interactionText;
    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = startingAmmo;
        gunSpeed = _weaponObject.fireSpeed;
        shootDamage = _weaponObject.dmg;
        shootDist = _weaponObject.dist;
        shootRate = _weaponObject.rate;
        magazineSize = _weaponObject.magazineCount;
        currentClip = magazineSize;
        reloadTime = _weaponObject.reloadTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetReloadTime()
    {
        return reloadTime;
    }

    public int GetStartingAmmo()
    {
        return startingAmmo;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
    public void UpdateCurrentAmmo(int bullets)
    {
        currentAmmo += bullets;
    }

    public void ReloadAmmo()
    {
        if (currentAmmo - magazineSize <= 0)
        {
            currentClip = currentAmmo;
            currentAmmo = 0;

        }
        else
        {
            currentClip = magazineSize;
            currentAmmo -= magazineSize;
        }
    }

    public void UpdateCurrentClip(int bullets)
    {
        currentClip += bullets;
    }
    

    public int GetCurrentClip()
    {
        return currentClip;
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

    public int GetMagazineSize()
    {
        return magazineSize;
    }

    public WeaponObject GetWeaponObject()
    {
        return _weaponObject;
    }

    public void SetStartingAmmo(int start)
    {
        startingAmmo = start;
    }

    public void SetCurrentAmmo(int current)
    {
        currentAmmo = current;
    }

    public void SetGunSpeed(float speed)
    {
        gunSpeed = speed;
    }

    public void SetGunDamage(int dmg)
    {
        shootDamage = dmg;
    }
    
    public void SetShootDist(int dist)
    {
        shootDist = dist;
    }

    public void SetShootRate(float rate)
    {
        shootRate = rate;
    }

    public void SetMagSize(int magSize)
    {
        magazineSize = magSize;
    }

    public void Interact()
    {
        Destroy(gameObject);
    }
}
