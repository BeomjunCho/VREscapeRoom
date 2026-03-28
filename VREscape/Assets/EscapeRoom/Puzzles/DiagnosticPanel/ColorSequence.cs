using UnityEngine;

public class ColorSequence : MonoBehaviour
{

    [SerializeField] private string defaultDisplay;

    [SerializeField] private Material defaultLightMat;
    [SerializeField] private Material greenLightMat;
    [SerializeField] private Material redLightMat;
    [SerializeField] private Material blueLightMat;
    [SerializeField] private MeshRenderer[] displayColors = new MeshRenderer[0];

    public int Length => displayColors.Length;

    public void OnValidate()
    {
        UpdateDisplay(defaultDisplay);
    }

    public void UpdateDisplay(string currentSequence)
    {
        for (int i = 0; i < displayColors.Length; i++)
        {
            displayColors[i].material = i >= currentSequence.Length
                ? defaultLightMat
                : currentSequence[i] switch
                {
                    'r' => redLightMat,
                    'g' => greenLightMat,
                    'b' => blueLightMat,
                    _ => defaultLightMat
                };
        }
    }
}