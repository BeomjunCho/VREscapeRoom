using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NumberSequenceLock : SequenceCodeLock
{
    public string UnlockCode = "1234";
    protected override int MaxLength => UnlockCode.Length;

    [SerializeField] private UnityEvent unlockEvent;

    // Display
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private MeshRenderer lightIncorrect;
    [SerializeField] private MeshRenderer lightCorrect;

    [SerializeField] private Material defaultLightMat;
    [SerializeField] private Material greenLightMat;
    [SerializeField] private Material redLightMat;

    protected override void CheckSequence()
    {
        if (currentSequence == UnlockCode)
        {
            IsLocked = false;
            unlockEvent.Invoke();
        }
        else
        {
            //IsLocked = true;
        }

        ClearSequence();
    }

    protected override void UpdateDisplay()
    {
        // Text
        displayText.text = currentSequence;

        // Light
        if (IsLocked)
        {
            lightIncorrect.material = redLightMat;
            lightCorrect.material = defaultLightMat;
        }
        else
        {
            lightIncorrect.material = defaultLightMat;
            lightCorrect.material = greenLightMat;
        }
    }
}
