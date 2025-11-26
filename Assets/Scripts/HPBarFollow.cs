using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPBarFollow : MonoBehaviour
{
    public Transform target;
    public Camera cam;
    public Vector3 offset = new Vector3(0, 0.5f, 0);

    public Image hpFill;
    public Image hpDelayed;

    public float maxDistance = 20f;
    public float minScale = 0.5f;
    public float maxScale = 1f;

    public float delay = 0.3f;
    public float delayedLerpSpeed = 0.5f;

    private Coroutine delayedRoutine;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
        {
            canvas.worldCamera = cam;
        }
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        transform.position = target.position + offset;
        transform.LookAt(transform.position + cam.transform.forward);

        float dist = Vector3.Distance(cam.transform.position, transform.position);
        bool visible = dist <= maxDistance;
        if (gameObject.activeSelf != visible)
            gameObject.SetActive(visible);

        transform.localScale = Vector3.one * Mathf.Lerp(maxScale, minScale, Mathf.Clamp01(dist / maxDistance));
    }

    public void SetHP(float current, float max)
    {
        if (hpFill == null || hpDelayed == null || max <= 0f) return;

        float normalized = Mathf.Clamp01(current / max);
        hpFill.fillAmount = normalized;

        if (delayedRoutine != null) StopCoroutine(delayedRoutine);
        delayedRoutine = StartCoroutine(DelayedHP(normalized));
    }
    private IEnumerator DelayedHP(float targetAmount)
    {
        if (hpDelayed == null) yield break;
        yield return new WaitForSeconds(delay);

        while (hpDelayed.fillAmount > targetAmount)
        {
            hpDelayed.fillAmount = Mathf.Lerp(hpDelayed.fillAmount, targetAmount, Time.deltaTime * delayedLerpSpeed);
            if (Mathf.Abs(hpDelayed.fillAmount - targetAmount) < 0.001f)
            {
                hpDelayed.fillAmount = targetAmount;
                break;
            }
            yield return null;
        }
    }
}
