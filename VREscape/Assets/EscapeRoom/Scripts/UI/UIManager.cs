using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject centerEyeAnchor;
    [SerializeField] private GameObject menuCanvas;

    [SerializeField] private Vector3 menuOffset = new Vector3(0,0,0.5f);

    public Color selectedColor;
    public Color normalColor;
    private bool MenuInput => PlayerController.Instance.menu;
    private bool lastInput;
    private bool menuActive;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        HandleInput();

        menuCanvas.SetActive(menuActive);

        Transform head = centerEyeAnchor.transform;
        Vector3 euler = head.rotation.eulerAngles;
        float reducedPitch = Mathf.LerpAngle(0f, euler.x, 0.2f);
        Quaternion newRot = Quaternion.Euler(reducedPitch, euler.y, 0f);
        menuCanvas.transform.rotation = newRot;
        menuCanvas.transform.position = head.position + newRot * new Vector3(0, 0, menuOffset.z);

    }

    private void HandleInput()
    {
        if (MenuInput && !lastInput)
        {
            menuActive = !menuActive;
        }

        lastInput = MenuInput;
    }

    public void UpdateToggleColor(Toggle toggle)
    {
        toggle.targetGraphic.color = toggle.isOn
            ? selectedColor
            : normalColor;
    }

    public void UpdateToggleColor(Toggle toggle, bool active)
    {
        toggle.targetGraphic.color = active
            ? selectedColor
            : normalColor;
    }

}
