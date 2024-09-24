using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour, IDamage
{
    
    #region Variables
    
    [Header("References")]
    [SerializeField] CharacterController charController;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private Transform cameraPivotTransform;
    public Animator animator;
    

    [Header("Player Stats - General")]
    public int HP;
    public int HPMax;
    public float speed;
    public int money = 50;
    private Vector3 prevPos;
    [SerializeField] private float originalSpeed;
    [SerializeField] private float sprintMod;
    [SerializeField] private float crouchMod;
    [SerializeField] private float visibility;
    [SerializeField] private float sprintVisMod;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpSpeed;
    [SerializeField] private int gravity;
    
    
    [Header("Player Stats - Crouching")]
    [SerializeField] private float crouchTime;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float originalHeight;
    [SerializeField] private float newHeight;
    [SerializeField] private float crouchVisMod;

    [Header("Player Stats - Lean/Sway")]
    [SerializeField] private float originalAngle;
    [SerializeField] private float leanAngle;
    [SerializeField] private float leanTime;
    [SerializeField] private float leanMovingThreshold;
    [SerializeField] private Vector3 leanPos;
    [SerializeField] private float swayAngle;
    [SerializeField] private float swayTime;
    [SerializeField] private bool toggleSwayFwdBck;
    [SerializeField] private bool toggleSwayLeftRight;

    [Header("Player Stats - Shooting")] 
    [SerializeField] private List<WeaponObject> weaponsInInventory;
    [SerializeField] private List<GameObject> weaponsObjects;
    [SerializeField] private WeaponObject equippedWeapon;
    [SerializeField] private Weapon currentWeapon;
    public int shootDamage;
    [SerializeField] private int shootDist;
    public float shootRate;
    [SerializeField] private GameObject gun;
    [SerializeField] private Transform hipPos, adsPos, gunSpawnPos;
    [SerializeField] private float gunSpeed;
    public int bulletsLeft;
    [SerializeField] private Vector3 weaponDropOffset;
    
  //  [SerializeField] private float shootRate;
    public int magazineSize;
    public int ammoTotal;

    [Header("Interactable Settings")] 
    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask interactMask;
    [SerializeField] private GameObject currentHoveredInteractable;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce;
    [SerializeField] private float grenadeForce;

    [Header("Shooting Effects")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform muzzleFlashSpawnPoint;

    [Header("Damage Effects")]
    [SerializeField] private float damageFlashDuration;
    [Header("Night Vision")]
    [SerializeField] PostProcessVolume nightVision;

    private Vector3 _moveDir;
    private Vector3 _playerVelocity;
    private float _currentVelocity;

    private int _jumpCount;

    public bool _isSprinting;
    public bool _isWalking;
    public bool _isShooting;
    public bool _isCrouching;
    public bool _isLeaning;
    public bool _canInteract;
    public bool _isReloading;
    
    private Camera _mainCam;

    private float horizInput;
    private float vertInput;
    
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        if (weaponsInInventory != null)
        {
            equippedWeapon = weaponsInInventory[0];
            gun = weaponsObjects[0];
            setGunValuesToPlayerValues(equippedWeapon);
        }
        charController = GetComponent<CharacterController>();
        originalAngle = cameraPivotTransform.localRotation.z;
        originalHeight = charController.height;
        originalSpeed = speed;
        newHeight = originalHeight;
        _mainCam = Camera.main;
        money = 0;
        HP = HPMax;
        GameManager.instance.healthBar.fillAmount = (float)HP / HPMax;
        SpawnPlayer();
        bulletsLeft = gun.GetComponent<Weapon>().GetCurrentClip();
        ammoTotal = gun.GetComponent<Weapon>().GetCurrentAmmo();
    }

    public void SpawnPlayer()
    {
        charController.enabled = false;
        transform.position = GameManager.instance.playerSpawnPos.transform.position;
        charController.enabled = true;
        HP = HPMax;
        currentWeapon = gun.GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused) return;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        horizInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
        visibility = 0;
        
        movement();
        sprint();
        crouch();
        headSway();
        leanCameraPivot();
        swapFire();
        checkForInteractable();
        addNewWeapon();
        dropCurrentWeapon();
        swapWeapon();
        SetNightVision();
        prevPos = GameManager.instance.player.transform.position;
        
    }

    public void swapFire()
    {
        if (gun == null) return;
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("ads", true);
            _mainCam.fieldOfView = 50f;
        }
        else
        {
            animator.SetBool("ads", false);
            _mainCam.fieldOfView = 60f;
        }
    }

    public float getPlayerVisibility()
    {
        return visibility * 100;
    }

    public void modPlayerVisibility(float lightPercent)
    {
        visibility = Mathf.Max(visibility, lightPercent);
        

        if (_isCrouching)
        {
            visibility -= (crouchVisMod / 100);
        }

        if (_isSprinting)
        {
            visibility += (sprintVisMod / 100);
        }

        visibility = Mathf.Clamp(visibility, 0, 1);
    }

    #region Movement

    void movement()
    {
        if (charController.isGrounded)
        {
            _playerVelocity = Vector3.zero;
            _jumpCount = 0;
        }
        
        _moveDir = horizInput * transform.right +
                   vertInput * transform.forward;
        charController.Move(_moveDir * (speed * Time.deltaTime));

        //Player has moved
        if (prevPos != GameManager.instance.player.transform.position && !_isSprinting && !_isWalking && charController.isGrounded)
        {
            StartCoroutine(walking());
        }

        if (Input.GetButtonDown("Jump") && _jumpCount < jumpMax)
        {
            AudioManager.instance.playSFX(AudioManager.instance.jump);

            _jumpCount++;
            _playerVelocity.y = jumpSpeed;
        }

        charController.Move(_playerVelocity * Time.deltaTime);
        _playerVelocity.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && !_isShooting && !_isReloading)
        {
            StartCoroutine(shoot());
        }

        if (Input.GetButton("Reload") && !_isReloading && !_isShooting && currentWeapon.GetCurrentAmmo() > 0)
        {
            if (currentWeapon.GetCurrentClip() < currentWeapon.GetMagazineSize())
            {
                StartCoroutine(reload());
            }
        } 
    }

    IEnumerator walking()
    {
        _isWalking = true;
        AudioManager.instance.playMove(AudioManager.instance.footStepWalking);
        yield return new WaitForSeconds(AudioManager.instance.footStepWalking.length);
        _isWalking = false;
    }

    // handles player sprinting
    void sprint()
    {
        
        if (Input.GetButtonDown("Sprint") && !_isCrouching)
        {
            //Play sprint loop
            AudioManager.instance.playMove(AudioManager.instance.footStepRunning, true);


            speed *= sprintMod;
            _isSprinting = true;
        } else if (Input.GetButtonUp("Sprint") && !_isCrouching)
        {
            AudioManager.instance.stopMoveLoop();

            speed = originalSpeed;
            _isSprinting = false;
        }
    }

    // handles player crouching
    void crouch()
    {
        if (Input.GetButtonDown("Crouch") && !_isSprinting)
        {
            AudioManager.instance.playMove(AudioManager.instance.crouchDown);

            speed *= crouchMod;
            newHeight = crouchHeight;
            _isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch") && !_isSprinting)
        {
            AudioManager.instance.playMove(AudioManager.instance.crouchUp);

            speed = originalSpeed;
            newHeight = originalHeight;
            _isCrouching = false;
        }
        
        
        // updates the player character controller height to be the original height or crouch height, depending on if the crouch input is held
        charController.height = Mathf.SmoothDamp(charController.height, newHeight, ref _currentVelocity,crouchTime);

    }

    #endregion

    #region Head/Camera Movement
    // handles player leaning via camera pivot and lean buttons input
    void leanCameraPivot()
    {
        bool isMoving = Mathf.Abs(horizInput) > leanMovingThreshold || Mathf.Abs(vertInput) > leanMovingThreshold;
        
        
        if (isMoving && !_isCrouching)
        {
            handleLean(0,0,originalAngle, leanTime, Vector3.zero);
            _isLeaning = false;
        }
        else
        {
            if (Input.GetButton("LeanL"))
            {
                
                handleLean(0,0,leanAngle, leanTime, -leanPos);
                _isLeaning = true;
            } else if (Input.GetButton("LeanR"))
            {
                handleLean(0,0,-leanAngle, leanTime, leanPos);
                _isLeaning = true;
            }
            else
            {
                handleLean(0,0,originalAngle, leanTime, Vector3.zero);
                _isLeaning = false;
            }
        }
    }
    
    // sways player head based on movement input
    void headSway()
    {
        // check for game option to disable sway effects
        // check if player is leaning and return
        if (_isLeaning) return;

        if (toggleSwayLeftRight)
        {
            if (horizInput > 0.25f)
            {
                handleLean(0,0,-swayAngle, swayTime, Vector3.zero);
            } else if (horizInput < -0.25f)
            {
                handleLean(0,0,swayAngle, swayTime, Vector3.zero);
            }
        }

        if (toggleSwayFwdBck)
        {
            if (vertInput > 0.25f)
            {
                handleLean(swayAngle,0,0,swayTime, Vector3.zero);
            }
            else if (vertInput < -0.25f)
            {
                handleLean(-swayAngle,0,0,swayTime, Vector3.zero); 
            }
        }
 
    }

    // helper function for doing leaning calculations
    void handleLean(float _xAngle, float _yAngle, float _zAngle, float _leanTime, Vector3 _leanPos)
    {
        cameraPivotTransform.localRotation = Quaternion.Lerp(cameraPivotTransform.localRotation,
            Quaternion.Euler(_xAngle, _yAngle, _zAngle), Time.deltaTime * _leanTime);
        cameraPivotTransform.localPosition =
            Vector3.Lerp(cameraPivotTransform.localPosition, _leanPos, Time.deltaTime * leanTime);
    }
    
    #endregion
    
    #region Interactables

    void checkForInteractable()
    {
        RaycastHit hit;

        if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out hit, interactDistance, ~interactMask))
        {
            _canInteract = true;
            currentHoveredInteractable = hit.transform.gameObject;
            GameManager.instance.ToggleInteractionUI(true, currentHoveredInteractable.GetComponent<Weapon>().interactionText);
            Debug.Log(hit.transform.name);
        }
        else
        {
            _canInteract = false;
            currentHoveredInteractable = null;
            GameManager.instance.ToggleInteractionUI(false, "");
        }
    }
    void grappleEnemy()
    {
        // Enter Grapple State
                
        // If Key 1, knock out.
        // Else if Key 2, Kill.
    }

    void climbLadder()
    {
        // Enter climbing state
        // When player reaches x height, remove player from ladder climb state
    }

    void addNewWeapon()
    {

        if (currentHoveredInteractable == null) return;
        WeaponObject newWeapon;
        if(currentHoveredInteractable.GetComponent<Weapon>() )
        {
            newWeapon = currentHoveredInteractable.GetComponent<Weapon>().GetWeaponObject();
        }
        else { return; }
        
        if (Input.GetButtonDown("Interact"))
        {
            if (weaponsInInventory.Contains(newWeapon)) return;
            if (gun != null)
            {
                gun.SetActive(false);
            }
            weaponsInInventory.Add(newWeapon);
            GameObject clone = Instantiate(newWeapon.prefab, gunSpawnPos.transform.position, Quaternion.identity);
            clone.transform.parent = gunSpawnPos;
            clone.transform.localRotation = Quaternion.Euler(0,0,0);
            gun = clone;
            currentWeapon = gun.GetComponent<Weapon>();
            bulletsLeft = currentWeapon.GetCurrentAmmo();
            equippedWeapon = newWeapon;
            weaponsObjects.Add(clone);
            SendPickupDataToPlayerWeapon();
            setGunValuesToPlayerValues(equippedWeapon);
            Destroy(currentHoveredInteractable);
        }
        
    }

    void SendPickupDataToPlayerWeapon()
    {
        Weapon pickedUpWeapon = currentHoveredInteractable.GetComponent<Weapon>();
        Weapon spawnedWeapon = gun.GetComponent<Weapon>();
        
        spawnedWeapon.SetCurrentAmmo(pickedUpWeapon.GetCurrentAmmo());
        spawnedWeapon.SetStartingAmmo(pickedUpWeapon.GetStartingAmmo());
        spawnedWeapon.SetGunDamage(pickedUpWeapon.GetGunDamage());
        spawnedWeapon.SetGunSpeed(pickedUpWeapon.GetGunSpeed());
        spawnedWeapon.SetMagSize(pickedUpWeapon.GetMagazineSize());
        spawnedWeapon.SetShootDist(pickedUpWeapon.GetShootDist());
        spawnedWeapon.SetShootRate(pickedUpWeapon.GetShootRate());
    }

    void swapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            swap(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            swap(1);
        }
    }

    void swap(int weaponIndex)
    {
        if (weaponsInInventory.Count < weaponIndex) return; 
        equippedWeapon = weaponsInInventory[weaponIndex];
        gun.SetActive(false);
        gun = weaponsObjects[weaponIndex];
        currentWeapon = gun.GetComponent<Weapon>();
        gun.SetActive(true);
        setGunValuesToPlayerValues(equippedWeapon);

    }

    void setGunValuesToPlayerValues(WeaponObject newWeapon)
    {
        gunSpeed = newWeapon.fireSpeed;
        shootRate = newWeapon.rate;
        shootDamage = newWeapon.dmg;
        shootDist = newWeapon.dist;
        magazineSize = newWeapon.magazineCount;
        firePoint = gun.GetComponent<Weapon>().GetFirePoint();
    }

    void dropCurrentWeapon()
    {
        if (gun == null) return;
        if (Input.GetButtonDown("Drop"))
        {
            Vector3 dropPosition = _mainCam.transform.position + transform.TransformDirection(weaponDropOffset);
            
            GameObject clone = Instantiate(equippedWeapon.prefab_rb, dropPosition, Quaternion.identity);
            
            Rigidbody rb = clone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = _mainCam.transform.forward; 
            }

            weaponsInInventory.Remove(equippedWeapon);
            weaponsObjects.Remove(gun);
            Destroy(gun);

            if (weaponsInInventory.Count == 0)
            {
                equippedWeapon = null;
            }
            else
            {
                equippedWeapon = weaponsInInventory[0];
                gun = weaponsObjects[0];
                gun.SetActive(true);
            }
        }
    }
    
    #endregion
    
    #region Shooting And Damage
    
    IEnumerator shoot()
    {
        bulletsLeft = gun.GetComponent<Weapon>().GetCurrentClip();
        if (bulletsLeft > 0)
        {

            gun.GetComponent<Weapon>().UpdateCurrentClip(-1);
            animator.SetTrigger("shoot");
            _isShooting = true;

            // for returning damage on what was hit
            RaycastHit hit;

            // fire raycast in camera forward by shootDist variable and return info from hit
            if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out hit, shootDist, ~ignoreMask))
            {
                bulletsLeft = gun.GetComponent<Weapon>().GetCurrentClip();
                ammoTotal = gun.GetComponent<Weapon>().GetCurrentAmmo();

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.TakeDamage(shootDamage, transform.position);
                }

            }
            // Instantiate bullet
            //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            //Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            // Play bullet sound
            AudioManager.instance.playSFX(AudioManager.instance.shootPistol);

            // Instantiate muzzle flash
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(muzzleFlash, 0.1f);

            yield return new WaitForSeconds(shootRate);
            _isShooting = false;
        }
    }

    IEnumerator reload()
    {
        _isReloading = true;
        animator.SetTrigger("reload");
        
        yield return new WaitForSeconds(gun.GetComponent<Weapon>().GetReloadTime());
        
        gun.GetComponent<Weapon>().ReloadAmmo();
        _isReloading = false;
        bulletsLeft = gun.GetComponent<Weapon>().GetCurrentClip();
        ammoTotal = gun.GetComponent<Weapon>().GetCurrentAmmo();
    }

    public void TakeDamage(int amount, Vector3 origin)
    {
        AudioManager.instance.playSFX(AudioManager.instance.playerHurt);

        HP -= amount;
        StartCoroutine(damageFlash());
        GameManager.instance.healthBar.fillAmount = (float)HP / HPMax;
        if (HP <= 0)
        {
            GameManager.instance.healthBar.fillAmount = (float)HP / HPMax;
            GameManager.instance.LoseGame();
        }
    }

    IEnumerator damageFlash()
    {
        GameManager.instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(damageFlashDuration);
        GameManager.instance.damagePanel.SetActive(false);
    }
    void ThrowGrenade()
    {
        // Instantiate grenade
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);
        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();
        if (grenadeRb != null)
        {
            grenadeRb.AddForce(firePoint.forward * grenadeForce, ForceMode.Impulse);
        }
    }

    public void UpdateTotalAmmo(int amount)
    {
        gun.GetComponent<Weapon>().UpdateCurrentAmmo(amount);
        ammoTotal = gun.GetComponent<Weapon>().GetCurrentAmmo();
    }
    #endregion

    #region Night Vision
    public void SetNightVision()
    {
        if (Input.GetButtonDown("NightVision"))
        {
            if (nightVision.weight > 0)
            {
                nightVision.weight = 0;
            }
            else
                nightVision.weight = 1f;
        }
    }
    #endregion
}
