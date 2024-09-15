using UnityEngine;
using UnityEditor;
using System;

namespace Backbone.LevelGenerator.VisualizationGraph.Editor
{
    public class DungeonSettingsManager
    {
        public DungeonGenerator DungeonGenerator { get; private set; }

        public Vector2Int GridSize { get; private set; } = new Vector2Int(2, 2);
        public float CellSizeForDungeon { get; private set; } = 1f;
        public GameObject TilePrefab { get; private set; }
        public Quaternion TileRotation { get; private set; } = Quaternion.identity;
        public bool AutoCellSize { get; private set; } = false;
        public bool IsConnectorMode { get; private set; } = false;

        public void Initialize(DungeonGenerator dungeonGenerator)
        {
            DungeonGenerator = dungeonGenerator;
        }

        // Draw the settings panel
        public void DrawSettings(Action generateDungeonCallback)
        {
            AutoCellSize = EditorGUILayout.Toggle("Auto Cell Size", AutoCellSize);

            if (!AutoCellSize)
            {
                CellSizeForDungeon = EditorGUILayout.FloatField("Cell Size", CellSizeForDungeon);
            }

            TilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", TilePrefab, typeof(GameObject), false);
            TileRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Tile Rotation", TileRotation.eulerAngles));

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Dungeon"))
            {
                generateDungeonCallback?.Invoke();
            }

            IsConnectorMode = GUILayout.Toggle(IsConnectorMode, "Connector Mode");
        }

        // Trigger the dungeon generation in the DungeonGenerator
        public void GenerateDungeon(System.Collections.Generic.List<Vector2Int> activeCells)
        {
            DungeonGenerator.GenerateDungeonFromEditor(GridSize, CellSizeForDungeon, TilePrefab, TileRotation, AutoCellSize, activeCells);
        }
    }
}
