using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float startAngle = -90f;
    [SerializeField] private float openAngle = 90f;

    [SerializeField] private bool invertSwing = false;

    private bool isOpen = false;
    private float targetY = -90f;
    private Transform player;

    public bool isLocked = false;

    private void Start()
    {
        targetY = startAngle;
        transform.localRotation = Quaternion.Euler(0f, startAngle, 0f);
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        float currentY = transform.localEulerAngles.y;
        transform.localRotation = Quaternion.Euler(
            0f,
            Mathf.LerpAngle(currentY, targetY, Time.deltaTime * speed),
            0f
        );
    }

    public void Lock()
    {
        StartCoroutine(PlayUnlockSFX());
        isLocked = true;
    }
    public void Unlock()
    {
        StartCoroutine(PlayUnlockSFX());
        isLocked = false;
    }

    public void ToggleDoor()
    {
        if (!isLocked)
        {
            AudioManager.Instance.PlayOneShot(AudioCue.Door);
            if (isOpen)
            {
                targetY = startAngle;
            }
            else
            {
                Vector3 toPlayer = player.position - transform.position;
                float dot = Vector3.Dot(transform.right, toPlayer);
                bool swingPositive = invertSwing ? dot > 0f : dot <= 0f;
                targetY = swingPositive ? startAngle - openAngle : startAngle + openAngle;
            }

            isOpen = !isOpen;
        }
        else
        {
            CancelInvoke(nameof(ResetNudge));
            targetY = startAngle; 
            targetY -= 5f;
            Invoke(nameof(ResetNudge), 0.15f);
        }
    }

    private void ResetNudge()
    {
        targetY = startAngle; 
    }

    private IEnumerator PlayUnlockSFX()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.15f));
        AudioManager.Instance.PlayOneShot3D(AudioCue.Door, transform.position);
    }
}