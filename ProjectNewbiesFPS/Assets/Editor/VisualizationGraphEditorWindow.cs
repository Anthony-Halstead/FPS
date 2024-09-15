using UnityEditor;
using UnityEngine;

namespace Backbone.LevelGenerator.VisualizationGraph.Editor
{
    public class VisualizationGraphEditorWindow : EditorWindow
    {
        private CellGridManager cellGridManager;    // Manages cells and their states
        private DungeonSettingsManager settingsManager;  // Manages dungeon settings and generation
        private float leftPaneWidth = 200f;
        private readonly float minPaneWidth = 100f;
        private bool isResizing = false;
        private Vector2 settingsScrollPosition;
        private bool hotUpdateEnabled = false;      // Toggle for hot updating

        [MenuItem("Tools/Visualization Graph Editor")]
        private static void OpenWindow()
        {
            VisualizationGraphEditorWindow window = GetWindow<VisualizationGraphEditorWindow>("Visualization Graph Editor");
            window.minSize = new Vector2(600, 400);
        }

        private void OnEnable()
        {
            var dungeonGenerator = FindObjectOfType<DungeonGenerator>();
            if (dungeonGenerator == null)
            {
                Debug.LogError("DungeonGenerator component not found in the scene. Please add it to an object.");
            }

            settingsManager = new DungeonSettingsManager();
            settingsManager.Initialize(dungeonGenerator);

            cellGridManager = new CellGridManager();
        }

        private void OnGUI()
        {
            float rightPaneWidth = position.width - leftPaneWidth - 5f;

            // Handle left panel (Graph section)
            GUILayout.BeginArea(new Rect(0, 0, leftPaneWidth, position.height));
            GUILayout.Label("Graph", EditorStyles.boldLabel);

            cellGridManager.DrawGrid(leftPaneWidth, position.height);
            ProcessCellEvents(Event.current);

            GUILayout.EndArea();

            // Draw divider line
            EditorGUI.DrawRect(new Rect(leftPaneWidth, 0, 1, position.height), Color.black);

            // Handle right panel (Settings section)
            GUILayout.BeginArea(new Rect(leftPaneWidth + 5, 0, rightPaneWidth, position.height));
            GUILayout.Label("Settings", EditorStyles.boldLabel);

            settingsScrollPosition = GUILayout.BeginScrollView(settingsScrollPosition);
            {
                hotUpdateEnabled = EditorGUILayout.Toggle("Hot Update Enabled", hotUpdateEnabled);
                settingsManager.DrawSettings(GenerateDungeon);
            }
            GUILayout.EndScrollView();

            GUILayout.EndArea();

            HandleResizing();
        }

        private void HandleResizing()
        {
            Rect resizeRect = new Rect(leftPaneWidth, 0, 5f, position.height);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                isResizing = true;
            }

            if (isResizing && Event.current.type == EventType.MouseDrag)
            {
                leftPaneWidth = Mathf.Clamp(Event.current.mousePosition.x, minPaneWidth, position.width - minPaneWidth);
                Repaint();
            }

            if (Event.current.type == EventType.MouseUp)
            {
                isResizing = false;
            }
        }

        private void ProcessCellEvents(Event e)
        {
            cellGridManager.ProcessEvents(e, hotUpdateEnabled, HotUpdateDungeon);
        }

        private void HotUpdateDungeon()
        {
            var activeCells = cellGridManager.GetActiveCellPositions();
            settingsManager.GenerateDungeon(activeCells);
        }

        private void GenerateDungeon()
        {
            var activeCells = cellGridManager.GetActiveCellPositions();
            settingsManager.GenerateDungeon(activeCells);
        }
    }
}
