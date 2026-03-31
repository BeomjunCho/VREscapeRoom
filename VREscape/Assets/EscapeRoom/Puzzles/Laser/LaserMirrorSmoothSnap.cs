using UnityEngine;

public class LaserMirrorSmoothSnap : MonoBehaviour
{
    void SnapToGrid()
    {
        float snapAngle = 45f;
        Vector3 currentRot = transform.eulerAngles;
        currentRot.y = Mathf.Round(currentRot.y / snapAngle) * snapAngle;

        transform.Rotate(currentRot, 0.3f);
    }
}
