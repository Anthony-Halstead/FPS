using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class WeaponWheel : MonoBehaviour
{
    [SerializeField] private GameObject wheelPieceParent;
    [SerializeField] private GameObject wheelParent;

    [SerializeField] private GameObject wheelPiecePrefab;
    [SerializeField] private List<GameObject> wheelPieces;

    [SerializeField] private GameObject trackingCircle;
    [SerializeField] private RectTransform circleTransform;

    [SerializeField] private GameObject selectedPiece;
    [SerializeField] private RectTransform selectedPieceTransform;

    public float expandedScale = 1.2f;
    public float tweenDuration = 0.2f;

    private Vector3 originalScale;
    private GameObject previousSelectedPiece;

    private bool wheelOpen;

    void Start()
    {
        // Get the tracking circle's RectTransform if not set
        circleTransform = trackingCircle.GetComponent<RectTransform>();
    }

    void Update()
    {
        HandleInput();
        if (wheelOpen)
        {
            TrackMouseWithCircle();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButton(2))
        {
            DisplayWeaponWheel();
        }

        if (Input.GetMouseButtonUp(2))
        {
            HideWeaponWheel();
        }
    }

    public void DisplayWeaponWheel()
    {
        wheelParent.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        wheelOpen = true;
        Time.timeScale = 0f;
    }

    public void HideWeaponWheel()
    {
        UnGrowSelectedPiece();
        Cursor.lockState = CursorLockMode.Locked;
        wheelParent.SetActive(false);
        wheelOpen = false;
        Time.timeScale = 1f;
    }

    private void TrackMouseWithCircle()
    {
        // Move the circle to the mouse position
        Vector2 mousePosition = Input.mousePosition;
        circleTransform.position = mousePosition;
    }

    public void OnPieceHovered(GameObject hoveredPiece)
    {
        if (selectedPiece != hoveredPiece)
        {
            UnGrowSelectedPiece();
            selectedPiece = hoveredPiece;
            GrowSelectedPiece();
        }
    }


    public void GrowSelectedPiece()
    {
        if (selectedPiece == null) return;
        Debug.Log("Grow");
        selectedPieceTransform = selectedPiece.GetComponent<RectTransform>();

        // Store the original scale when selecting a new piece
        originalScale = selectedPieceTransform.localScale;

        // Animate to the expanded scale
        selectedPieceTransform.DOScale(expandedScale, tweenDuration).SetEase(Ease.OutBack);
    }

    public void UnGrowSelectedPiece()
    {
        if (previousSelectedPiece != null)
        {
            Debug.Log("Shrink");
            RectTransform previousTransform = previousSelectedPiece.GetComponent<RectTransform>();

            // Restore the original scale with a tween
            previousTransform.DOScale(originalScale, tweenDuration).SetEase(Ease.InBack);
        }

        // Update the previous selected piece
        previousSelectedPiece = selectedPiece;

    }

    public void AddNewWheelPiece(WeaponObject newWeapon)
    {
        for (int i = 0; i < wheelPieces.Count; i++)
        {
            if (wheelPieces[i].GetComponent<WeaponWheelSlot>().slotWeapon == newWeapon)
            {
                return;
            }
        }

        GameObject clone = Instantiate(wheelPiecePrefab, wheelPieceParent.transform.position, Quaternion.identity);
        clone.GetComponent<WeaponWheelSlot>().slotWeapon = newWeapon;
        wheelPieces.Add(clone);
    }

    public void RemoveWheelPiece(WeaponObject oldWeapon)
    {
        // Implementation for removing a weapon from the wheel
    }
}