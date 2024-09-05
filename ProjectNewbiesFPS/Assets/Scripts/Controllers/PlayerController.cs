using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{

    [SerializeField] CharacterController charController;
    [SerializeField] private LayerMask ignoreMask;

    [SerializeField] private int HP;
    [SerializeField] private int speed;
    [SerializeField] private int sprintMod;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpSpeed;
    [SerializeField] private int gravity;

    [SerializeField] private float damageFlashDuration;


    [SerializeField] private int shootDamage;
    [SerializeField] private int shootDist;
    [SerializeField] private float shootRate;
    
    private Vector3 _moveDir;
    private Vector3 _playerVelocity;

    private int _jumpCount;

    private bool _isSprinting;
    private bool _isShooting;
    private Camera mainCam;
    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        
        movement();
        sprint();
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
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            _isSprinting = true;
        } else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            _isSprinting = false;
        }
    }

    IEnumerator shoot()
    {
        _isShooting = true;

        // for returning damage on what was hit
        RaycastHit hit;
        
        // fire raycast in camera forward by shootDist variable and return info from hit
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, shootDist, ~ignoreMask))
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
