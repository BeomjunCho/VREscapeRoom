using UnityEngine;

//[ExecuteAlways]
public class LaserMirror : MonoBehaviour
{
    [HideInInspector] public LaserSystem LaserSystem;
    [SerializeField] private GameObject emitterObject;

    [Header("Ray Settings")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitMask = Physics.DefaultRaycastLayers;

    public bool EmitLaser;

    private LineRenderer lineRenderer;

    private LaserMirror lastHitMirror;
    private LaserMirror previousMirror;

    private void OnEnable()
    {
        if (emitterObject == null)
        {
            return;
        }

        lineRenderer = emitterObject.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!EmitLaser)
        {
            DeactivateLaser();
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

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);

        LaserMirror hitMirror = null;
        if (didHit && hit.collider.CompareTag("LaserMirror"))
        {
            LaserMirror hm = hit.collider.GetComponentInParent<LaserMirror>();
            if (hm != previousMirror)
                hitMirror = hm;
        }

        if (didHit && hit.collider.CompareTag("LaserReceiver"))
        {
            LaserSystem.RegisterInput("Enter");
        }

        // Deactivate previous mirror if it changed
        if (lastHitMirror != null && lastHitMirror != hitMirror)
            lastHitMirror.DeactivateLaser();

        // Activate new mirror
        if (hitMirror != null)
        {
            hitMirror.previousMirror = this;
            hitMirror.ActivateLaser();
        }

        lastHitMirror = hitMirror;
    }

    private void ActivateLaser()
    {
        if (EmitLaser) return;

        EmitLaser = true;
        LaserSystem.AppendSequence(this);
    }

    private void DeactivateLaser()
    {
        if (!EmitLaser) return;

        EmitLaser = false;
        LaserSystem.RemoveSequence(this);

        previousMirror = null;

        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (lastHitMirror != null)
        {
            lastHitMirror.DeactivateLaser();
            lastHitMirror = null;
        }
    }
}