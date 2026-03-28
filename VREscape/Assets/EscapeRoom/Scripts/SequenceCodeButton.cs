using TMPro;
using UnityEngine;

[ExecuteAlways]
public class SequenceCodeButton : MonoBehaviour
{
    public string ButtonAction = "";
    [HideInInspector] public SequenceCodeLock LockRef;

    private TMP_Text buttonText;

    private void Awake()
    {
        if (ButtonAction == "")
        {
            ButtonAction = transform.gameObject.name;
        }

        buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = ButtonAction;
        }
    }

    public void OnButtonClicked()
    {
        LockRef.RegisterInput(ButtonAction);
    }
}
