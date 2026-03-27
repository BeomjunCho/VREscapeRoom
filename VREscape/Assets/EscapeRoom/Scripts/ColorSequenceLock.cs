using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ColorSequenceLock : SequenceCodeLock
{
    // Display
    [SerializeField] private TMP_Text displayText;

    [SerializeField] private Material defaultLightMat;
    [SerializeField] private Material greenLightMat;
    [SerializeField] private Material redLightMat;
    [SerializeField] private Material blueLightMat;

    [SerializeField] private MeshRenderer[] displayColors = new MeshRenderer[0];

    [System.Serializable]
    public class CodeEntry
    {
        public string Code;
        public string DisplayOutput;
    }

    [SerializeField] private List<CodeEntry> codes;
    private CodeEntry matchedCode;

    protected override int MaxLength => displayColors.Length;

    protected override void AppendSequence(string inAction)
    {
        base.AppendSequence(inAction);

        if (currentSequence.Length == MaxLength)
        {
            RegisterInput("Enter");
        }
    }

    protected override void CheckSequence()
    {
        matchedCode = codes.Find(e => e.Code == currentSequence);

        if (matchedCode != null)
        {
            displayText.text = matchedCode.DisplayOutput;
        }
        else
        {
            ClearSequence();
        }
    }

    protected override void UpdateDisplay()
    {
        for (int i = 0; i < displayColors.Length; i++)
        {
            if (i >= currentSequence.Length)
            {
                displayColors[i].material = defaultLightMat;
                continue;
            }

            displayColors[i].material = currentSequence[i] switch
            {
                'r' => redLightMat,
                'g' => greenLightMat,
                'b' => blueLightMat,
                _ => defaultLightMat
            };
        }
    }

    protected override void ClearSequence()
    {
        base.ClearSequence();
        displayText.text = "";
    }
}
