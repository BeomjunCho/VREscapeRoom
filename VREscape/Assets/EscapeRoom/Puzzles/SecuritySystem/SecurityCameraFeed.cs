using UnityEngine;

public class SecurityCameraFeed : MonoBehaviour
{
    [SerializeField] private SecurityCamera[] cams = new SecurityCamera[0];

    [SerializeField] private Renderer displayRenderer;
    [SerializeField] private int displayMatIndex = 1;

    private int camIndex = 0;
    private bool isZoomed;

    private void Start()
    {
        ChangeView();
    }

    public void ChangeView()
    {
        Debug.Log("Changing camera texture to "+ cams[camIndex].GetComponentInChildren<Camera>().targetTexture);

        Debug.Log("Changing camera material " + displayRenderer.materials[displayMatIndex]);

        camIndex = (camIndex + 1) % cams.Length;
        displayRenderer.materials[displayMatIndex].
            SetTexture("_RenderTexture", cams[camIndex].GetComponentInChildren<Camera>().targetTexture);

        

        isZoomed = false;
    }
    public void ToggleZoom()
    {
        isZoomed = !isZoomed;
        cams[camIndex].GetComponentInChildren<Camera>().focalLength = isZoomed ? 60f : 25f;
    }

}
