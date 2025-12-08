using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Death : MonoBehaviour
{
    public Volume globalVolume;
    private ColorAdjustments colorAdjust;
    void Start()
    {
        globalVolume.profile.TryGet(out colorAdjust);
    }

    public void OnPlayerDeath()
    {
        StartCoroutine(FadeToGrayscale(2f));
    }

    IEnumerator FadeToGrayscale(float duration)
    {
        float startSat = colorAdjust.saturation.value;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            colorAdjust.saturation.value = Mathf.Lerp(startSat, -100f, elapsed / duration);
            yield return null;
        }
    }
}
