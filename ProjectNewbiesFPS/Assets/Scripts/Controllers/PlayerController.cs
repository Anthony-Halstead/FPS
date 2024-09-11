using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour, IDamage
{
    
    #region Variables
    
    [Header("References")]
    [SerializeField] CharacterController charController;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private Transform cameraPivotTransform;
    public AudioManager audioManager;

    [Header("Player Stats - General")]
    public int HP;
    public int HPMax;
    public float speed;
    public int money = 50;
    [SerializeField] private float originalSpeed;
    [SerializeField] private float sprintMod;
    [SerializeField] private float crouchMod;
    [SerializeField] private int visibility;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpSpeed;
    [SerializeField] private int gravity;
    
    [Header("Player Stats - Crouching")]
    [SerializeField] private float crouchTime;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float originalHeight;
    [SerializeField] private float newHeight;

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
    public int shootDamage;
    [SerializeField] private int shootDist;
    public float shootRate;
    [SerializeField] private GameObject gun;
    [SerializeField] private Transform hipPos, adsPos;
    [SerializeField] private float gunSpeed;
    
  //  [SerializeField] private float shootRate;
    [SerializeField] public int magazineSize;

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
    
    private Vector3 _moveDir;
    private Vector3 _playerVelocity;
    private float _currentVelocity;

    private int _jumpCount;

    public bool _isSprinting;
    public bool _isShooting;
    public bool _isCrouching;
    public bool _isLeaning;
    
    private Camera _mainCam;

    private float horizInput;
    private float vertInput;
    
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        originalAngle = cameraPivotTransform.localRotation.z;
        originalHeight = charController.height;
        originalSpeed = speed;
        newHeight = originalHeight;
        _mainCam = Camera.main;
        money = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        horizInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
        
        movement();
        sprint();
        crouch();
        headSway();
        leanCameraPivot();
        swapFire();
    }

    public void swapFire()
    {
        if (Input.GetMouseButton(1))
        {
            gun.transform.position = Vector3.Lerp(gun.transform.position, adsPos.position, Time.deltaTime * gunSpeed);
        }
        else
        {
            gun.transform.position = Vector3.Lerp(gun.transform.position, hipPos.position, Time.deltaTime * gunSpeed);
        }
    }

    public int getPlayerVisibility()
    {
        return visibility;
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

        if (Input.GetButtonDown("Jump") && _jumpCount < jumpMax)
        {
            audioManager.playSFX(audioManager.jump);

            _jumpCount++;
            _playerVelocity.y = jumpSpeed;
        }

        charController.Move(_playerVelocity * Time.deltaTime);
        _playerVelocity.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && !_isShooting)
        {
            StartCoroutine(shoot());
        }
    }

    // handles player sprinting
    void sprint()
    {
        
        if (Input.GetButtonDown("Sprint") && !_isCrouching)
        {
            audioManager.playSFX(audioManager.footStepRunning);

            speed *= sprintMod;
            _isSprinting = true;
        } else if (Input.GetButtonUp("Sprint") && !_isCrouching)
        {
            speed = originalSpeed;
            _isSprinting = false;
        }
    }

    // handles player crouching
    void crouch()
    {
        if (Input.GetButtonDown("Crouch") && !_isSprinting)
        {
            speed *= crouchMod;
            newHeight = crouchHeight;
            _isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch") && !_isSprinting)
        {
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

    void headBob()
    {
        // if player is moving
            // on a timer, smoothly move head up then down on one side
            // when that side has been complete, now do the same on the opposite side
    }
    
    #endregion
    
    #region Interactables    
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
    
    #endregion
    
    #region Shooting And Damage
    IEnumerator shoot()
    {
        _isShooting = true;

        // for returning damage on what was hit
        RaycastHit hit;
        
        // fire raycast in camera forward by shootDist variable and return info from hit
        if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage, transform.position);
            }

        }
        // Instantiate bullet
        //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        //Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        // Play bullet sound
        audioManager.playSFX(audioManager.shootPistol);

        // Instantiate muzzle flash
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
        Destroy(muzzleFlash, 0.1f);

        yield return new WaitForSeconds(shootRate);
        _isShooting = false;
    }

    public void takeDamage(int amount, Vector3 origin)
    {
        audioManager.playSFX(audioManager.playerHurt);

        HP -= amount;
        StartCoroutine(damageFlash());
        GameManager.instance.healthBar.fillAmount = HP / 10;
        if (HP <= 0)
        {
            
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
    #endregion
}
