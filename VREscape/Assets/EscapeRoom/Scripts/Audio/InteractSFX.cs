using UnityEngine;

public class InteractSFX : MonoBehaviour
{
    [SerializeField] private AudioCue cue;
    public void OnInteract()
    {
        AudioManager.Instance.PlayOneShot(cue);
    }
}
