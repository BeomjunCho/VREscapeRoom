using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class RecalculateBounds : MonoBehaviour
{
    void OnEnable()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            mf.sharedMesh.RecalculateBounds();
        }
    }
}