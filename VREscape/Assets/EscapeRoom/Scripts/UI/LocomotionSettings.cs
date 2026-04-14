using UnityEngine;
using UnityEngine.UI;

public class LocomotionSettings : MonoBehaviour
{
    [Header("Locomotion Mode")]

    [SerializeField] private Toggle teleport;
    [SerializeField] private GameObject[] teleportObjects = new GameObject[0];

    [SerializeField] private Toggle slide;
    [SerializeField] private GameObject[] slideObjects = new GameObject[0];

    [Header("Vignette")]
    [SerializeField] private Toggle vignette;
    [SerializeField] private GameObject vignetteObject;
    private void Awake()
    {
        slide.onValueChanged.AddListener(_ => SyncAll());
        teleport.onValueChanged.AddListener(_ => SyncAll());
        vignette.onValueChanged.AddListener(_ => SyncAll());
        SyncAll();
    }

    private void SyncAll()
    {
        SetLocomotionMode(slide.isOn);
        vignetteObject.SetActive(vignette.isOn);
    }
    private void SetLocomotionMode(bool locomotion)
    {
        foreach (GameObject obj in slideObjects) { obj.SetActive(locomotion); }
        foreach (GameObject obj in teleportObjects) { obj.SetActive(!locomotion); }

        UIManager.Instance.UpdateToggleColor(slide); 
        UIManager.Instance.UpdateToggleColor(teleport);
        UIManager.Instance.UpdateToggleColor(vignette, slide.isOn);
    }
}
