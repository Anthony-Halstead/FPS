using UnityEngine;
using UnityEditor;

namespace Backbone.LevelGenerator
{
    [CustomEditor(typeof(DungeonGenerator))]
    public class DungeonGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Reference to the DungeonGenerator script
            DungeonGenerator generator = (DungeonGenerator)target;

            // Add a space before the button
            GUILayout.Space(10);

            // Add a button to the inspector
            if (GUILayout.Button("Generate Dungeon"))
            {
                // Call the GenerateDungeon method when the button is clicked
                //generator.GenerateDungeon();
            }
        }
    }
}
