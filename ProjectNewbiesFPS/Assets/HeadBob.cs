using System;
using Unity.VisualScripting;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public Transform cameraTransform;  // The camera to apply the headbobbing effect to
    
    [Header("Bob Speed")]
    public float bobbingSpeed;
    public float bobbingSpeedOrig;   // Speed of the bobbing effect
    public float bobbingSpeedCrouch = 5f;
    public float bobbingSpeedSprintMod = 14f;
    [Header("Bob Amount")]
    public float bobbingAmount = 0.025f; // Amount of vertical movement for headbobbing
    public float bobbingAmountOrig;
    public float bobbingAmountSprint = 0.05f;
    public float bobbingAmountCrouch = 0.01f;
    
    [Header("Misc Bob Settings")]
    public float midpoint = 0.6f;       // The base height of the camera when not moving

    private float timer = 0f;
    [SerializeField] private CharacterController characterController;
    private PlayerController _player;

    private void Start()
    {
        _player = characterController.GetComponent<PlayerController>();
        bobbingAmountOrig = bobbingAmount;
        bobbingSpeedOrig = bobbingSpeed;
    }

    void DetermineMovementType()
    {
        if (_player._isSprinting)
        {
            bobbingSpeed = bobbingSpeedSprintMod;
            bobbingAmount = bobbingAmountSprint;
        } else if (_player._isCrouching)
        {
            bobbingSpeed = bobbingSpeedCrouch;
            bobbingAmount = bobbingAmountCrouch;
        }
        else
        {
            bobbingSpeed = bobbingSpeedOrig;
            bobbingAmount = bobbingAmountOrig;
        }
    }

    void Update()
    {
        DetermineMovementType();
        HandleHeadBobbing();
    }

    void HandleHeadBobbing()
    {
                
        float waveslice = 0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Check if player is moving
        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0f;  // Reset the timer when not moving
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeedOrig * Time.deltaTime;

            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        // Apply the headbobbing effect
        if (waveslice != 0)
        {
            float bobbingEffect = waveslice * bobbingAmount;
            float totalAxes = Mathf.Clamp(Mathf.Abs(horizontal) + Mathf.Abs(vertical), 0f, 1f);
            bobbingEffect *= totalAxes;

            Vector3 newPosition = cameraTransform.localPosition;
            newPosition.y = midpoint + bobbingEffect;
            cameraTransform.localPosition = newPosition;
        }
        else
        {
            // Reset camera position when not moving
            Vector3 resetPosition = cameraTransform.localPosition;
            resetPosition.y = midpoint;
            cameraTransform.localPosition = resetPosition;
        }
    }
}
