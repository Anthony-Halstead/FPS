using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Backbone.LevelGenerator.VisualizationGraph
{
    public class CellGridManager
    {
        private List<Cell> cells;
        private float cellSize = 50f;
        private float cellPadding = 1f;
        private Color defaultColor = Color.gray;
        private Color activeColor = Color.green;
        private Color connectorColor = Color.cyan;
        private Color gridLineColor = Color.black;

        private int gridWidth;
        private int gridHeight;

        private bool isLeftMouseButtonHeld = false;   // Track if the left mouse button is held
        private bool isRightMouseButtonHeld = false;  // Track if the right mouse button is held

        public CellGridManager()
        {
            cells = new List<Cell>();
        }

        public void DrawGrid(float paneWidth, float paneHeight)
        {
            gridWidth = Mathf.CeilToInt(paneWidth / cellSize) + 1;
            gridHeight = Mathf.CeilToInt(paneHeight / cellSize) + 1;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Rect rect = new Rect(x * cellSize + cellPadding, y * cellSize + cellPadding, cellSize - 2 * cellPadding, cellSize - 2 * cellPadding);

                    if (cells.Count <= x * gridHeight + y)
                    {
                        cells.Add(new Cell(rect, defaultColor, activeColor, connectorColor));
                    }

                    cells[x * gridHeight + y].rect = rect;
                    cells[x * gridHeight + y].Draw();
                }
            }

            DrawGridLines(paneWidth, paneHeight);
        }

        private void DrawGridLines(float paneWidth, float paneHeight)
        {
            Handles.BeginGUI();
            Handles.color = gridLineColor;

            for (int y = 0; y <= gridHeight; y++)
            {
                float yPos = y * cellSize;
                Handles.DrawLine(new Vector3(0, yPos), new Vector3(paneWidth, yPos));
            }

            for (int x = 0; x <= gridWidth; x++)
            {
                float xPos = x * cellSize;
                Handles.DrawLine(new Vector3(xPos, 0), new Vector3(xPos, paneHeight));
            }

            Handles.EndGUI();
        }

        public void ProcessEvents(Event e, bool hotUpdateEnabled, System.Action hotUpdateAction)
        {
            Vector2 mousePosition = e.mousePosition;

            // Handle cell activation/deactivation during mouse hold
            if (isLeftMouseButtonHeld || isRightMouseButtonHeld)
            {
                foreach (var cell in cells)
                {
                    Rect cellRect = cell.rect;

                    if (cellRect.Contains(mousePosition))
                    {
                        if (isLeftMouseButtonHeld)
                        {
                            cell.isActive = true;
                        }
                        else if (isRightMouseButtonHeld)
                        {
                            cell.ResetCell();
                        }

                        if (hotUpdateEnabled)
                        {
                            hotUpdateAction?.Invoke();
                        }

                        GUI.changed = true;
                    }
                }
            }

            // Detect when the mouse buttons are pressed or released
            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0) isLeftMouseButtonHeld = true;   // Left mouse button pressed
                if (e.button == 1) isRightMouseButtonHeld = true;  // Right mouse button pressed
            }
            else if (e.type == EventType.MouseUp)
            {
                if (e.button == 0) isLeftMouseButtonHeld = false;  // Left mouse button released
                if (e.button == 1) isRightMouseButtonHeld = false; // Right mouse button released
            }
        }

        public List<Vector2Int> GetActiveCellPositions()
        {
            List<Vector2Int> activeCells = new List<Vector2Int>();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (cells[x * gridHeight + y].isActive)
                    {
                        activeCells.Add(new Vector2Int(x, y));
                    }
                }
            }

            return activeCells;
        }
    }
}
