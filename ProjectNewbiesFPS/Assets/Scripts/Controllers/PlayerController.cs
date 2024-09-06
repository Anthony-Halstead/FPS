using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour, IDamage
{

    [SerializeField] CharacterController charController;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private Transform cameraPivotTransform;

    [SerializeField] private int HP;
    [SerializeField] private float speed;
    [SerializeField] private float sprintMod;
    [SerializeField] private float crouchMod;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpSpeed;
    [SerializeField] private int gravity;
    
    [SerializeField] private float crouchTime;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float originalHeight;
    [SerializeField] private float newHeight;

    [SerializeField] private float originalAngle;
    [SerializeField] private float leanAngle;
    [SerializeField] private float leanTime;
    
    [SerializeField] private float damageFlashDuration;


    [SerializeField] private int shootDamage;
    [SerializeField] private int shootDist;
    [SerializeField] private float shootRate;
    
    private Vector3 _moveDir;
    private Vector3 _playerVelocity;
    private float _currentVelocity;

    private int _jumpCount;

    private bool _isSprinting;
    private bool _isShooting;
    private bool _isCrouching;
    
    private Camera _mainCam;

    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        originalAngle = cameraPivotTransform.localRotation.z;
        originalHeight = charController.height;
        newHeight = originalHeight;
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        
        movement();
        sprint();
        crouch();
        leanCameraPivot();
    }

    void movement()
    {
        if (charController.isGrounded)
        {
            _playerVelocity = Vector3.zero;
            _jumpCount = 0;
        }
        
        _moveDir = Input.GetAxis("Horizontal") * transform.right +
                   Input.GetAxis("Vertical") * transform.forward;
        charController.Move(_moveDir * (speed * Time.deltaTime));

        if (Input.GetButtonDown("Jump") && _jumpCount < jumpMax)
        {
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

    void sprint()
    {
        
        if (Input.GetButtonDown("Sprint") && !_isCrouching)
        {
            speed *= sprintMod;
            _isSprinting = true;
        } else if (Input.GetButtonUp("Sprint") && !_isCrouching)
        {
            speed /= sprintMod;
            
            _isSprinting = false;
        }
    }

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
            speed /= crouchMod;
            newHeight = originalHeight;
            _isCrouching = false;
        }
        
        
        // updates the player character controller height to be the original height or crouch height, depending on if the crouch input is held
        charController.height = Mathf.SmoothDamp(charController.height, newHeight, ref _currentVelocity,crouchTime);

    }

    void leanCameraPivot()
    {
        if (Input.GetButton("LeanL"))
        {
            handleLean(leanAngle);
        } else if (Input.GetButton("LeanR"))
        {
            handleLean(-leanAngle);
        }
        else
        {
            handleLean(originalAngle);
        }
    }

    void handleLean(float angle)
    {
        Vector3 currentEulerAngles = _mainCam.transform.localEulerAngles;

        cameraPivotTransform.localRotation = Quaternion.Lerp(cameraPivotTransform.localRotation,
            Quaternion.Euler(0, 0, angle), Time.deltaTime * leanTime);
    }

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
                dmg.takeDamage(shootDamage);
            }
        }
        
        yield return new WaitForSeconds(shootRate);
        _isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(damageFlash());
        
        if (HP <= 0)
        {
            //GameManager.instance.youLose();
        }
    }

    IEnumerator damageFlash()
    {
        //GameManager.instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(damageFlashDuration);
        //GameManager.instance.damagePanel.SetActive(false);
    }
}
