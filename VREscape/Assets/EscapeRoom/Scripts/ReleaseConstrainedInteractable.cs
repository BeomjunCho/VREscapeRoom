using UnityEngine;

public class ReleaseConstrainedInteractable : MonoBehaviour
{
    public void OnRelease()
    {
        Vector3 pos = transform.localPosition;
        pos.z = 0f;
        transform.localPosition = pos;

        Vector3 rot = transform.localEulerAngles;
        rot.x = 0f;
        rot.y = 0f;
        transform.localEulerAngles = rot;
    }
}