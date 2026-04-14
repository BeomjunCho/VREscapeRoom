using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float openAngle = 90f;

    private bool isOpen = false;
    private float targetY = -90f;
    private Transform player;

    public bool isLocked = false;

    private void Start()
    {
        transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
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
        isLocked = true;
    }
    public void Unlock()
    {
        isLocked = false;
    }

    public void ToggleDoor()
    {

        if (!isLocked)
        {
            if (isOpen)
            {
                targetY = -90f;
            }
            else
            {
                Vector3 toPlayer = player.position - transform.position;
                float dot = Vector3.Dot(transform.right, toPlayer);
                targetY = dot >= 0f ? -90f + openAngle : -90f - openAngle;
            }

            isOpen = !isOpen;
        }
        else
        {
            CancelInvoke(nameof(ResetNudge));
            targetY = -90f;
            targetY -= 5f;
            Invoke(nameof(ResetNudge), 0.15f);
        }

    }

    private void ResetNudge()
    {
        targetY += 5f;
    }
}