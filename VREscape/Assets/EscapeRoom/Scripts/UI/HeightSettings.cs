using UnityEngine;
using UnityEngine.UI;

public class HeightSettings : MonoBehaviour
{
    [SerializeField] private GameObject trackingSpace;

    [Header("Height Mode")]
    [SerializeField] private Toggle standing;
    [SerializeField] private Vector3 defaultStandingHeight = new Vector3(0, 0, 0);

    [SerializeField] private Toggle sitting;
    [SerializeField] private Vector3 defaultSittingHeight = new Vector3(0, 0.3f, 0);

    [Header("Height Adjustment")]
    [SerializeField] private Button heightUp;
    [SerializeField] private Button heightDown;
    [SerializeField] private float heightStep = 0.1f;
    [SerializeField] private float minHeight = -0.7f;
    [SerializeField] private float maxHeight = 0.7f;

    private void Awake()
    {
        standing.onValueChanged.AddListener(_ => SetHeight());
        sitting.onValueChanged.AddListener(_ => SetHeight());
        heightUp.onClick.AddListener(() => AdjustHeight(heightStep));
        heightDown.onClick.AddListener(() => AdjustHeight(-heightStep));
    }
    private void SetHeight()
    {
        trackingSpace.transform.localPosition = standing.isOn ? defaultStandingHeight : defaultSittingHeight;

        UIManager.Instance.UpdateToggleColor(standing);
        UIManager.Instance.UpdateToggleColor(sitting);
    }

    private void AdjustHeight(float step)
    {
        Vector3 pos = trackingSpace.transform.position;
        pos.y = Mathf.Clamp(pos.y + step, minHeight, maxHeight);
        trackingSpace.transform.position = pos;
    }
}
