using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class SecurityCamera : MonoBehaviour
{
    public RenderTexture RenderTexture;

    private void Awake()
    {
        var cam = GetComponentInChildren<Camera>();
        if (cam.targetTexture == null && RenderTexture != null)
        {
            cam.targetTexture = RenderTexture;
        }
    }

    private void OnDestroy()
    {
        if (RenderTexture != null)
            RenderTexture.Release();
    }
}

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

            var rt = new RenderTexture(960, 540, 24); 
            string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/EscapeRoom/Puzzles/SecuritySystem/RenderTextures/RT_{secCam.transform.name}.renderTexture");
            AssetDatabase.CreateAsset(rt, path);
            AssetDatabase.SaveAssets();

            Undo.RecordObjects(new Object[] { cam, secCam }, "Create Security Camera RT");
            cam.targetTexture = rt;
            secCam.RenderTexture = rt;

            Debug.Log($"[SecurityCamera] Created '{path}'.");
        }
    }
}
#endif
