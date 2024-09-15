using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
#endif

#if UNITY_EDITOR
namespace Backbone.LevelGenerator.Debugging
{
    [ExecuteInEditMode]
    public static class DebugExtensions
    {
        
        public static GameObject GenerateGeneric(PrimitiveType type, Material mat = null, ParentData ? data = null )
        {
            GameObject shape = GameObject.CreatePrimitive(type);
            if (data.HasValue)
            {
                shape.transform.parent = data.Value.parent;
                shape.transform.position = data.Value.position;
                shape.transform.rotation = data.Value.rotation;
                shape.transform.localScale = data.Value.scale;
            }
            shape.TryGetComponent<Collider>(out Collider col);
            if (col != null)
            {
                Object.DestroyImmediate(col);
            }
            if (shape.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
            {
                if (mat != null)
                    renderer.material = mat;
            }
            return shape;
        }
    }
}
#endif
