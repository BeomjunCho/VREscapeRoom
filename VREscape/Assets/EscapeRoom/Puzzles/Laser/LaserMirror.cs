using UnityEngine;

//[ExecuteAlways]
public class LaserMirror : MonoBehaviour
{
    [HideInInspector] public LaserSystem LaserSystem;
    [SerializeField] private GameObject emitterObject;

    [SerializeField] private GameObject mirrorObject;
    private float resetSpeed = 2f;
    private bool isResetting;

    [Header("Ray Settings")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitMask = Physics.DefaultRaycastLayers;

    public bool EmitLaser;

    private LineRenderer lineRenderer;

    private LaserMirror lastHitMirror;
    private LaserMirror previousMirror;

    private bool lastHitReceiver;
    private Vector3 receiverLocation;

    public void ResetPosition()
    {
        isResetting = true;
    }

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
        if (isResetting)
        {
            mirrorObject.transform.localRotation = Quaternion.Lerp(
                mirrorObject.transform.localRotation,
                Quaternion.identity,
                resetSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(mirrorObject.transform.localRotation, Quaternion.identity) < 0.1f)
            {
                mirrorObject.transform.localRotation = Quaternion.identity;
                isResetting = false;
            }
        }

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

        // Trigger ending
        bool hitReceiver = didHit && hit.collider.CompareTag("LaserReceiver");

        if (hitReceiver)
            LaserSystem.receiverLocation = hit.point;

            if (hitReceiver && !lastHitReceiver)
            LaserSystem.OnReceiverHit();
        else if (!hitReceiver && lastHitReceiver)
            LaserSystem.OnReceiverLost();

        lastHitReceiver = hitReceiver;

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

    public void ActivateLaser()
    {
        if (EmitLaser) return;

        EmitLaser = true;
        LaserSystem.AppendSequence(this);
        AudioManager.Instance.PlayOneShot(AudioCue.LaserHit);
    }

    public void DeactivateLaser()
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

        lastHitReceiver = false;
    }
}