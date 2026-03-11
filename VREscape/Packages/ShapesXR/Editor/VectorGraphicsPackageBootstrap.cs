#if UNITY_EDITOR && !UNITY_6000_3_OR_NEWER
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace ShapesXR.Import
{
	// Adds com.unity.vectorgraphics package for unity versions < 6000.3 since it can't be added as dependency in >= 6000.3
    [InitializeOnLoad]
    internal static class VectorGraphicsPackageBootstrap
    {
        private const string VectorGraphicsPackageName = "com.unity.vectorgraphics";

        static VectorGraphicsPackageBootstrap()
        {
            var list = Client.List(true);
            EditorApplication.update += () =>
            {
                if (!list.IsCompleted) return;
                EditorApplication.update -= null;

                if (list.Result.Any(p => p.name == VectorGraphicsPackageName))
                    return;

                Client.Add(VectorGraphicsPackageName);
                Debug.Log("Added missing Vector Graphics package (com.unity.vectorgraphics)");
            };
        }
    }
}
#endif