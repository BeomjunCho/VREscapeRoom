using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class TestLineRenderer : MonoBehaviour
{
    [Header("Ray Settings")]
    [Tooltip("Maximum ray distance if nothing is hit.")]
    public float maxDistance = 100f;

    [Tooltip("Layers the ray can hit.")]
    public LayerMask hitMask = Physics.DefaultRaycastLayers;

    [Header("Line Settings")]
    public float lineWidth = 0.05f;
    public Color lineColor = Color.red;

    private LineRenderer _lr;
    private Vector3 _hitPoint;
    private bool _didHit;

    private void OnEnable()
    {
        _lr = GetComponent<LineRenderer>();
        ApplyLineStyle();
    }

    private void Update()
    {
        CastAndDraw();
    }

    private void CastAndDraw()
    {
        if (_lr == null) return;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        _didHit = Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, hitMask);
        Vector3 endPoint = _didHit ? hit.point : origin + direction * maxDistance;

        if (_didHit) _hitPoint = hit.point;

        _lr.positionCount = 2;
        _lr.SetPosition(0, origin);
        _lr.SetPosition(1, endPoint);
    }

    private void ApplyLineStyle()
    {
        if (_lr == null) return;

        _lr.startWidth = lineWidth;
        _lr.endWidth = lineWidth;
        _lr.useWorldSpace = true;

        // Create a simple material if none is assigned
        if (_lr.sharedMaterial == null)
        {
            _lr.material = new Material(Shader.Find("Sprites/Default"));
        }

        _lr.startColor = lineColor;
        _lr.endColor = lineColor;
    }
    public Vector3 GetHitPoint() => _didHit ? _hitPoint : Vector3.zero;

    public bool IsHitting() => _didHit;
}