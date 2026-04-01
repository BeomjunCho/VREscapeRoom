using UnityEngine;
using UnityEngine.UI;

public class LocomotionToggle : MonoBehaviour
{
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;

    [SerializeField] private Toggle teleport;
    [SerializeField] private GameObject[] teleportObjects = new GameObject[0];

    [SerializeField] private Toggle slide;
    [SerializeField] private GameObject[] slideObjects = new GameObject[0];

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

        teleport.targetGraphic.color = teleport.isOn
            ? selectedColor
            : normalColor;

        slide.targetGraphic.color = slide.isOn
            ? selectedColor
            : normalColor;

        vignette.targetGraphic.color = slide.isOn
           ? selectedColor
            : normalColor;
    }
}
