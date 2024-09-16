using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AssetList", fileName = "AssetList")]
public class AssetList : ScriptableObject
{

    public List<GameObject> enemies;
    public List<GameObject> pickups;
}
