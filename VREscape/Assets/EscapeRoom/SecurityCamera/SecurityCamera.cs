using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class SecurityCamera : MonoBehaviour
{
    public RenderTexture renderTexture;

    private void Awake()
    {
        var cam = GetComponentInChildren<Camera>();
        if (cam.targetTexture == null && renderTexture != null)
            cam.targetTexture = renderTexture;
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
            renderTexture.Release();
    }
}

// 60 / 25

#if UNITY_EDITOR
[CustomEditor(typeof(SecurityCamera))]
public class SecurityCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var secCam = (SecurityCamera)target;
        var cam = secCam.GetComponentInChildren<Camera>();

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Render Texture"))
        {
            if (cam.targetTexture != null)
            {
                Debug.LogWarning("[SecurityCamera] A render texture is already assigned.");
                return;
            }

            if (!AssetDatabase.IsValidFolder("Assets/RenderTextures"))
                AssetDatabase.CreateFolder("Assets", "RenderTextures");

            var rt = new RenderTexture(256, 192, 24); 
            string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/EscapeRoom/SecurityCamera/RenderTextures/RT_{secCam.transform.name}.renderTexture");
            AssetDatabase.CreateAsset(rt, path);
            AssetDatabase.SaveAssets();

            Undo.RecordObjects(new Object[] { cam, secCam }, "Create Security Camera RT");
            cam.targetTexture = rt;
            secCam.renderTexture = rt;

            Debug.Log($"[SecurityCamera] Created '{path}'.");
        }
    }
}
#endif
