using UnityEngine;
using UnityEngine.UI;

public class RadialLayout : LayoutGroup
{
    public float fDistance;
    
    public float rotationOffset;
    public float subImageRotationOffset;

    [Range(0f, 360f)]
    public float minAngle, maxAngle, startAngle;

    protected override void OnEnable()
    {
        base.OnEnable();

        CalculateRadial();
    }

    public override void SetLayoutHorizontal() { }

    public override void SetLayoutVertical() { }

    public override void CalculateLayoutInputVertical() { CalculateRadial(); }

    public override void CalculateLayoutInputHorizontal() { CalculateRadial(); }
    
    #if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        CalculateRadial();
    }
    #endif
void CalculateRadial()
{
    m_Tracker.Clear();

    if (transform.childCount == 0) return;

    float fOffsetAngle = (maxAngle - minAngle) / (transform.childCount - 1);
    float fAngle = startAngle;

    for (int i = 0; i < transform.childCount; i++)
    {
        RectTransform child = (RectTransform)transform.GetChild(i);

        if (child != null)
        {
            m_Tracker.Add(this, child,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.Pivot |
                DrivenTransformProperties.Rotation);

            // Position the child in a radial layout
            Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
            child.localPosition = vPos * fDistance;
            child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

            // Calculate direction from child to the center of the wheel
            Vector3 directionToCenter = (Vector3.zero - child.localPosition).normalized;
            
            // Calculate angle towards the center, including the rotation offset
            float angleToCenter = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
            float totalAngle = angleToCenter + rotationOffset; // Include the rotation offset

            // Rotate the child object to face the center of the wheel with the rotation offset
            child.localRotation = Quaternion.Euler(0, 0, totalAngle);

            // Reset scaling of the child to avoid any scaling issues
            child.localScale = Vector3.one;

            // Handle the sub-image (weapon icon)
            if (child.childCount > 0)
            {
                RectTransform subImage = (RectTransform)child.GetChild(0);  // Assuming the first child is the icon
                if (subImage != null)
                {
                    // Rotate the sub-image to face the center with its own rotation offset
                    subImage.localRotation = Quaternion.Euler(0, 0, subImageRotationOffset);

                    // Reset scaling of the sub-image to prevent any distortions
                    subImage.localScale = Vector3.one;
                }
            }

            fAngle += fOffsetAngle;
        }
    }
}




}
