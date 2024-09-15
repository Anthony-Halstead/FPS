using UnityEngine;
using System.Collections.Generic;

namespace Backbone.LevelGenerator
{
    public class TileGrid
    {
        public float cellSize; // Size of each cell (e.g., 1x1 unit)
        public Dictionary<Vector2Int, Tile> tiles;

        public TileGrid(float cellSize)
        {
            this.cellSize = cellSize; // Ensure the cell size is passed correctly
            tiles = new Dictionary<Vector2Int, Tile>();
        }

        // Calculate the world position for a given grid position
        public Vector3 CalculateWorldPosition(Vector2Int gridPos)
        {
            // Use the cell size to calculate the position in the world
            return new Vector3(gridPos.x * cellSize, 0, gridPos.y * cellSize);
        }
    }

}
