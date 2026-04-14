using System.Collections;
using UnityEngine;

public class Sculpture : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private GameObject target;

    private Coroutine currentCoroutine;

    public void RaiseSculpture() => StartLerp(1f);
    public void LowerSculpture() => StartLerp(0f);

    private void StartLerp(float targetY)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(LerpScaleY(targetY));
    }

    private IEnumerator LerpScaleY(float targetY)
    {
        float startY = target.transform.localScale.y;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            target.transform.localScale = new Vector3(
                target.transform.localScale.x,
                Mathf.Lerp(startY, targetY, t),
                target.transform.localScale.z
            );
            yield return null;
        }

        target.transform.localScale = new Vector3(
            target.transform.localScale.x,
            targetY,
            target.transform.localScale.z
        );
    }
}