using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    //bullet
    public GameObject bullet;
    //bullet force and upward force for grenades
    public float shootForce, grenadeBounce;

    //gun stats
    public float shootRate, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    public bool allowInvoke = true;

    


    // Start is called before the first frame update
    void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

    }

    // Update is called once per frame
    void Update()
    {
        MyInput();

        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    private void MyInput()
    {
        //check if allowed to hold down button and take input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //reloading
        if (Input.GetKeyUp(KeyCode.R) && bulletsLeft > magazineSize && !reloading) Reload();
        //reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //middle of screen
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else targetPoint = ray.GetPoint(75);

        //calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //calculate spread
        float x = Random.Range(-spread, spread); ;
        float y = Random.Range(-spread, spread);

        //calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * grenadeBounce, ForceMode.Impulse);

        --bulletsLeft;
        ++bulletsShot;

        //Invoke resetShot function (if not already invoked)
        if (allowInvoke)
        {
            Invoke("ResetShot", shootRate);
            allowInvoke = false;
        }
        //if more than one bulletsPerTap ie Shotguns make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

        //Instantiate muzzle flash
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
       
    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
