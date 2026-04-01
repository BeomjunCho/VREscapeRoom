using UnityEngine;
using UnityEngine.UI;

public class LocomotionToggle : MonoBehaviour
{
    [SerializeField] private Toggle teleport;
    [SerializeField] private GameObject[] teleportObjects = new GameObject[0];

    [SerializeField] private Toggle slide;
    [SerializeField] private GameObject[] slideObjects = new GameObject[0];

    private void Awake()
    {
        slide.onValueChanged.AddListener(isOn => { if (isOn) SetLocomotionMode(true); });
        teleport.onValueChanged.AddListener(isOn => { if (isOn) SetLocomotionMode(false); });

        SetLocomotionMode(slide.isOn);
    }
    private void SetLocomotionMode(bool locomotion)
    {
        foreach (GameObject obj in slideObjects) { obj.SetActive(locomotion); }
        foreach (GameObject obj in teleportObjects) { obj.SetActive(!locomotion); }
    }
}
