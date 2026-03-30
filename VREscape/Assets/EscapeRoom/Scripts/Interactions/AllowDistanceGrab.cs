using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Unity.VisualScripting;
using UnityEngine;

public class AllowDistanceGrab : MonoBehaviour
{
    [SerializeField] private GameObject distanceGrab;
    [SerializeField] private GameObject grab;
    [SerializeField] private Transform controller;

    [Header("Raycast")]
    [SerializeField] private float maxRayDistance = 10f;
    private LineRenderer lineRenderer;
    [SerializeField] private Color rayIdleColor = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private Color rayHitColor = new Color(0.3f, 0.8f, 1f, 0.9f);

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetGrabbable(false);
    }

    private void Update()
    {
        if (PlayerController.Instance.bButton)
        {
            lineRenderer.enabled = true;
            DistanceGrabRaycast();
        }
        else
        {
            lineRenderer.enabled = false;
            SetGrabbable(false);
        }
    }

    private void DistanceGrabRaycast()
    {
        Ray ray = new Ray(controller.position, controller.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            if (hit.collider.CompareTag("GrabbableProp"))
            {
                SetGrabbable(true);
                DrawRay(controller.position, hit.point, rayHitColor);

                return;
            }
        }

        SetGrabbable(false);
        DrawRay(controller.position, controller.position + controller.forward * maxRayDistance, rayIdleColor);
    }



    private void DrawRay(Vector3 start, Vector3 end, Color color)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void SetGrabbable(bool grabbable)
    {
        distanceGrab.SetActive(grabbable);
        grab.SetActive(!grabbable);
    }
}