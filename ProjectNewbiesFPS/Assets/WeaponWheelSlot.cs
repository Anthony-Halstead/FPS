using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponWheelSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public WeaponObject slotWeapon;

    [SerializeField] private WeaponWheel _weaponWheel;

    public Image iconImage;
    // Start is called before the first frame update
    void Start()
    {
        iconImage.sprite = slotWeapon.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _weaponWheel.OnPieceHovered(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _weaponWheel.OnPieceHovered(this.gameObject);
    }
}
