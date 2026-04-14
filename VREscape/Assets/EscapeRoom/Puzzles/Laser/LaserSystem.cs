using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
public class LaserSystem : SequenceCodeLock
{
    public string UnlockCode = "1234"; 
    [SerializeField] private TMP_Text displayText;
    protected override int MaxLength => 100;

    [SerializeField] private LaserMirror[] mirrors;
    [SerializeField] private GameObject sparksVisualizer;

    [SerializeField] private UnityEvent laserOn;
    [SerializeField] private UnityEvent laserOff;

    protected override void Awake()
    {
        base.Awake();

        mirrors = GetComponentsInChildren<LaserMirror>();
        foreach (LaserMirror mirror in mirrors) { mirror.LaserSystem = this; }

        mirrors[0].DeactivateLaser();
    }

    public void ActivateLaserSystem()
    {
        mirrors[0].DeactivateLaser();

        foreach (LaserMirror mirror in mirrors)
        {
            if(mirror != mirrors[0])
                mirror.ResetPosition();
        }

        mirrors[0].ActivateLaser();
    }

    protected override void CheckSequence()
    {
        if (currentSequence == UnlockCode)
        {
            IsLocked = false;
            Debug.Log($"SUCCESS!\nCurrent sequence {currentSequence} IS the correct sequence {UnlockCode}");
            displayText.color = Color.green;
            laserOn.Invoke();
        }
        else
        {
            IsLocked = true;

            Debug.Log($"FAILURE!\nCurrent sequence {currentSequence} IS NOT the correct sequence {UnlockCode}");
            displayText.color = Color.red;
        }

    }

    protected override void UpdateDisplay()
    {
        // Text
        displayText.text = currentSequence;
    }

    public void AppendSequence(LaserMirror mirror)
    {
        base.AppendSequence(GetMirrorID(mirror).ToString());
        //Debug.LogWarning("Appended! Current sequence: " + currentSequence);
        UpdateDisplay();
    }

    public void RemoveSequence(LaserMirror mirror)
    {
        currentSequence = currentSequence.Replace(GetMirrorID(mirror).ToString(), "");
        //Debug.LogWarning("Removed! Current sequence: " + currentSequence);
        UpdateDisplay();
    }

    public int GetMirrorID(LaserMirror m)
    {
        return System.Array.IndexOf(mirrors, m)+1;
    }

    public void OnReceiverHit()
    {
        displayText.text = currentSequence + "8";
        //laserOn.Invoke(); //comment out
        CheckSequence();
    }

    public void OnReceiverLost()
    {
        UpdateDisplay();
        laserOff.Invoke();
        displayText.color = Color.white;
    }
}
