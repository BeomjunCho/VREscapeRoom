using TMPro;
using UnityEngine;

[ExecuteAlways]
public class SequenceCodeButton : MonoBehaviour
{
    public string ButtonAction = "";
    public SequenceCodeLock LockRef;
    private void Awake()
    {
        if (ButtonAction == "")
        {
            ButtonAction = transform.gameObject.name;
        }
    }

    public void OnButtonClicked()
    {
        LockRef.RegisterInput(ButtonAction);
    }
}
