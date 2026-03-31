using UnityEngine;

//[ExecuteAlways]
public class LaserMirror : MonoBehaviour
{
    [SerializeField] private GameObject emitterObject;

    [Header("Ray Settings")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitMask = Physics.DefaultRaycastLayers;

    public bool EmitLaser;

    private LineRenderer lineRenderer;

    private LaserMirror lastHitMirror;

    private void OnEnable()
    {
        if (emitterObject == null)
        {
            return;
        }

        lineRenderer = emitterObject.GetComponent<LineRenderer>();
    }

    private void OnDisable()
    {
        DisableLaser();
    }

    private void Update()
    {
        if (!EmitLaser)
        {
            DisableLaser();
            return;
        }

        RaycastLaser();
    }

    private void RaycastLaser()
    {
        if (lineRenderer == null) return;

        Vector3 origin = emitterObject.transform.position;
        Vector3 direction = emitterObject.transform.forward;

        bool didHit = Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, hitMask);
        Vector3 endPoint = didHit ? hit.point : origin + direction * maxDistance;

        // Draw laser
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);

        // Get next mirror
        LaserMirror hitMirror = null;
        if (didHit && hit.collider.CompareTag("Mirror"))
        {
            hitMirror = hit.collider.GetComponentInParent<LaserMirror>();
        }

        // Deactivate
        if (lastHitMirror != null && lastHitMirror != hitMirror)
        {
            lastHitMirror.EmitLaser = false;
        }

        if (hitMirror != null)
        {
            hitMirror.EmitLaser = true;
        }

        lastHitMirror = hitMirror;
    }

    private void DisableLaser()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (lastHitMirror != null)
        {
            lastHitMirror.EmitLaser = false;
            lastHitMirror = null;
        }
    }
}