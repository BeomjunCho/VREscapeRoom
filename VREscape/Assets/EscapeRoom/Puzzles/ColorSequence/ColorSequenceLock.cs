using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[RequireComponent(typeof(ColorSequence))]
public class ColorSequenceLock : SequenceCodeLock
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private List<CodeEntry> codes;

    private ColorSequence colorSequence;
    private CodeEntry matchedCode;

    [System.Serializable]
    public class CodeEntry
    {
        public string Code;
        public string DisplayOutput;
    }

    protected override int MaxLength => colorSequence.Length;

    protected override void Awake()
    {
        colorSequence = GetComponent<ColorSequence>();
        base.Awake();
    }

    protected override void AppendSequence(string inAction)
    {
        displayText.text = "";
        base.AppendSequence(inAction);
        if (currentSequence.Length == MaxLength)
        {
            RegisterInput("Enter");
        }
    }

    protected override void CheckSequence()
    {
        matchedCode = codes.Find(e => e.Code == currentSequence);

        if(matchedCode != null)
        {
            AudioManager.Instance.PlayOneShot(AudioCue.CodeUnlock);
            displayText.text = matchedCode.DisplayOutput;
        }
        else
        {
            AudioManager.Instance.PlayOneShot(AudioCue.UiNegative);
            displayText.text = "[ERROR]\nSequence not associated with system function";
        }
        ClearSequence();
    }

    protected override void UpdateDisplay()
    {
        colorSequence.UpdateDisplay(currentSequence);
    }
}