using UnityEditor;
using UnityEngine;

namespace Backbone.LevelGenerator.VisualizationGraph
{
    [System.Serializable]
    public class Cell
    {
        public Rect rect;
        public bool isActive;
        private bool isConnectorActive;
        private Color defaultColor;
        private Color activeColor;
        private Color connectorColor;

        public Cell(Rect rect, Color defaultColor, Color activeColor, Color connectorColor)
        {
            this.rect = rect;
            this.defaultColor = defaultColor;
            this.activeColor = activeColor;
            this.connectorColor = connectorColor;
            isActive = false;
            isConnectorActive = false;
        }

        // Draw the cell with the appropriate color based on its state
        public void Draw()
        {
            Color colorToUse = defaultColor;
            if (isConnectorActive)
            {
                colorToUse = connectorColor;
            }
            else if (isActive)
            {
                colorToUse = activeColor;
            }

            EditorGUI.DrawRect(rect, colorToUse);
        }

        // Set the cell into connector mode (turns it cyan)
        public void SetConnectorMode(bool active)
        {
            isConnectorActive = active;
            isActive = false;  // Ensure normal activation is turned off
        }

        // Reset the cell to its default state
        public void ResetCell()
        {
            isActive = false;
            isConnectorActive = false;
        }
    }
}
