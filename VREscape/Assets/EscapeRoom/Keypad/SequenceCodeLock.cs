
using TMPro;
using UnityEngine;

public class SequenceCodeLock : MonoBehaviour
{
    public string UnlockCode = "1234";
    private string currentSequence = "";

    // Display
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private MeshRenderer lightIncorrect;
    [SerializeField] private MeshRenderer lightCorrect;

    private void Awake()
    {
        SequenceCodeButton[] buttons = GetComponentsInChildren<SequenceCodeButton>();

        foreach (SequenceCodeButton button in buttons)
        {
            button.LockRef = this;
        }
    }

    public void RegisterInput(string inAction)
    {
        if (inAction == "Enter")
        {
            CheckSequence();
        }
        else if (inAction == "Clear")
        {
            ClearSequence();
        }
        else
        {
            AppendSequence(inAction);
        }

        UpdateDisplay();
    }

    private void AppendSequence(string inAction)
    {
        if(currentSequence.Length < UnlockCode.Length)
        {
            currentSequence += inAction;
        }
    }

    private void ClearSequence()
    {
        currentSequence = "";
    }

    private void CheckSequence()
    {
        if(currentSequence == UnlockCode)
        {
            Debug.Log("Correct code!");
        }
        else
        {
            Debug.Log("Incorrect code");
        }

        ClearSequence();
    }

    private void UpdateDisplay()
    {
        displayText.text = currentSequence;
    }
}
