using ShapesXR.ImportCore.Utils;
using UnityEditor;
using UnityEngine;
using AssetDatabase = UnityEditor.AssetDatabase;

namespace ShapesXR.Editor.Editor
{
    public class AssetDatabaseWrapper : IAssetDatabase
    {
        public T? LoadAssetAtPath<T>(string assetPath) where T : Object => AssetDatabase.LoadAssetAtPath<T>(assetPath);

        public void CreateAsset(Object asset, string path) => AssetDatabase.CreateAsset(asset, path);

        public void ImportAsset(string path, int importAssetOption = 0) => AssetDatabase.ImportAsset(path, (ImportAssetOptions)importAssetOption);

        public void SaveAsset(string path)
        {
            var guid = AssetDatabase.GUIDFromAssetPath(path);
            AssetDatabase.SaveAssetIfDirty(guid);
        }
    }
}