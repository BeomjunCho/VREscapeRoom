using TMPro;
using UnityEngine;
public class LaserSystem : SequenceCodeLock
{
    public string UnlockCode = "1234"; 
    [SerializeField] private TMP_Text displayText;
    protected override int MaxLength => UnlockCode.Length;

    [SerializeField] private LaserMirror[] mirrors;
    [SerializeField] private GameObject sparksVisualizer;
    protected override void Awake()
    {
        base.Awake();

        mirrors = GetComponentsInChildren<LaserMirror>();
        foreach (LaserMirror mirror in mirrors) mirror.LaserSystem = this;
    }

    protected override void CheckSequence()
    {
        if (currentSequence == UnlockCode)
        {
            IsLocked = false;
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
    }
}
