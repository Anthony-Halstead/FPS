using UnityEngine;

[CreateAssetMenu(fileName = "HoldArea", menuName = "AI/Behavior/HoldArea")]
public class HoldArea : StateBehavior
{
    [SerializeField] AreaType areaType { get => areaType; set => areaType = value; }
    public override void ExecuteBehavior(AIController controller)
    {
       /* return areaType switch
        {
            AreaType.Player => controller.AreaPosition = controller.playerPos,
            AreaType.Defensive => controller.AreaPosition = controller.StartPosition,
            _ => Vector3.zero,
        };*/
    }
}
public enum AreaType
{
    None,
    Player,
    Defensive,
}