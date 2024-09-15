using UnityEngine;

namespace Backbone.LevelGenerator
{
    [System.Serializable]
    public class Tile
    {
        public GameObject prefab;          // Prefab to use for the tile
        public Vector2Int gridPosition;    // Position of the tile in the grid
        public Quaternion rotation;        // Rotation of the tile

        public Tile(GameObject prefab, Vector2Int gridPosition, Quaternion rotation)
        {
            this.prefab = prefab;
            this.gridPosition = gridPosition;
            this.rotation = rotation;
        }
    }
}
