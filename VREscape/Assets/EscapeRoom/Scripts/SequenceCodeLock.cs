
using TMPro;
using UnityEngine;

public abstract class SequenceCodeLock : MonoBehaviour
{
    [HideInInspector] public bool IsLocked = true;
    protected string currentSequence = "";
    protected abstract int MaxLength { get; }

    protected virtual void Awake()
    {
        SequenceCodeButton[] buttons = GetComponentsInChildren<SequenceCodeButton>();

        foreach (SequenceCodeButton button in buttons)
        {
            button.LockRef = this;
        }

        UpdateDisplay();
    }

    public void RegisterInput(string inAction)
    {
        if (inAction == "Enter")
            CheckSequence();
        else if (inAction == "Clear")
            ClearSequence();
        else
            AppendSequence(inAction);

        UpdateDisplay();
    }


    protected virtual void AppendSequence(string inAction)
    {
        if (currentSequence.Length < MaxLength)
            currentSequence += inAction;
    }

    protected virtual void ClearSequence()
    {
        currentSequence = "";
        UpdateDisplay();
    }

    protected virtual void CheckSequence(){}
    protected virtual void UpdateDisplay(){}
}
