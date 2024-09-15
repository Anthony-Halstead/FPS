using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Backbone.LevelGenerator.Debugging;
#endif

namespace Backbone.LevelGenerator
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class Connector : MonoBehaviour
    {

        [SerializeField] private Vector2 size = Vector2.one * 4f;
        [SerializeField] Material mat = null;
        private GameObject icon = null; 


        private void OnDrawGizmos()
        {
            // Check if we are not in Play mode
            if (!EditorApplication.isPlaying)
            {
                Gizmos.color = Color.cyan;
                Vector3 center = transform.position + transform.up * size.y * 0.5f;
                Gizmos.DrawRay(center, transform.forward);

                Vector3 halfSizeX = transform.right * (size.x * 0.5f);
                Vector3 halfSizeY = transform.up * (size.y * 0.5f);

                Vector3 topLeft = center - halfSizeX + halfSizeY;
                Vector3 topRight = center + halfSizeX + halfSizeY;
                Vector3 bottomLeft = center - halfSizeX - halfSizeY;
                Vector3 bottomRight = center + halfSizeX - halfSizeY;

                // Draw the four lines connecting the corners
                Gizmos.DrawLine(topLeft, topRight);        // Top side
                Gizmos.DrawLine(topRight, bottomRight);    // Right side
                Gizmos.DrawLine(bottomRight, bottomLeft);  // Bottom side
                Gizmos.DrawLine(bottomLeft, topLeft);

                if (icon == null)
                {
                    icon = DebugExtensions.GenerateGeneric(PrimitiveType.Quad, mat, new ParentData { parent = transform, position = center,rotation = transform.rotation, scale = new Vector3(size.x, size.y, .1f) });
                }
                if (icon != null)
                    UpdateIcon(center);
            }
        }
        private void UpdateIcon(Vector3 center) { 
            icon.transform.position = center;
            icon.transform.rotation = transform.rotation;
            icon.transform.localScale = new Vector3(size.x, size.y,.1f);
        }
    }
     public struct ParentData
    {
        public Transform parent;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
#endif
}