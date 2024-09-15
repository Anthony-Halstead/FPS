using UnityEngine;
using System.Collections.Generic;

namespace Backbone.LevelGenerator
{
    public class DungeonGenerator : MonoBehaviour
    {
        private TileGrid grid;
        private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>();

        public void InitializeGrid(float cellSize)
        {
            if (grid == null)
            {
                grid = new TileGrid(cellSize);
            }
            else
            {
                grid.cellSize = cellSize; // Ensure the cell size is updated dynamically
            }
        }

        public void GenerateDungeonFromEditor(Vector2Int gridSize, float cellSize, GameObject tilePrefab, Quaternion tileRotation, bool autoCellSize, List<Vector2Int> activeCells)
        {
            if (autoCellSize)
            {
                // Calculate cell size based on prefab bounds
                cellSize = CalculateCellSizeFromPrefab(tilePrefab);
            }

            InitializeGrid(cellSize);

            // Remove inactive tiles first
            RemoveInactiveTiles(activeCells);

            // Place or update active tiles
            foreach (var cellPos in activeCells)
            {
                PlaceTileAtPosition(cellPos, tilePrefab, tileRotation);
            }
        }

        private void PlaceTileAtPosition(Vector2Int cellPos, GameObject tilePrefab, Quaternion tileRotation)
        {
            if (!activeTiles.ContainsKey(cellPos))
            {
                // Only place a tile if it's not already active
                Vector3 worldPosition = grid.CalculateWorldPosition(cellPos);
                GameObject tileInstance = Instantiate(tilePrefab, worldPosition, tileRotation, transform);
                activeTiles[cellPos] = tileInstance;
            }
        }

        private void RemoveInactiveTiles(List<Vector2Int> activeCells)
        {
            // Identify inactive tiles (tiles not in activeCells)
            List<Vector2Int> inactiveCells = new List<Vector2Int>();

            foreach (var tile in activeTiles)
            {
                if (!activeCells.Contains(tile.Key))
                {
                    inactiveCells.Add(tile.Key); // Mark as inactive
                }
            }

            // Remove all inactive tiles from the scene
            foreach (var cellPos in inactiveCells)
            {
                DestroyImmediate(activeTiles[cellPos]); // Remove the tile from the scene
                activeTiles.Remove(cellPos); // Remove the reference
            }
        }

        public float CalculateCellSizeFromPrefab(GameObject tilePrefab)
        {
            Renderer prefabRenderer = tilePrefab.GetComponent<Renderer>();
            if (prefabRenderer != null)
            {
                Bounds bounds = prefabRenderer.bounds;
                return Mathf.Max(bounds.size.x, bounds.size.z); // Use the largest dimension for spacing purposes
            }
            else
            {
                Debug.LogWarning("Tile Prefab does not have a Renderer component. Cannot auto-size cell.");
                return 1f;
            }
        }
    }
}
