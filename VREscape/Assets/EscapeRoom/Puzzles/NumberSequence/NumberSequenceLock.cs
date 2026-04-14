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
            OnUnlock();
            AudioManager.Instance.PlayOneShot(AudioCue.CodeUnlock);
        }
        else
        {
            AudioManager.Instance.PlayOneShot(AudioCue.UiNegative);
        }

        ClearSequence();
    }

    protected override void OnUnlock()
    {
        IsLocked = false;
        unlockEvent.Invoke();
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
