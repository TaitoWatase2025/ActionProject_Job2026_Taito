using UnityEngine;
using System.Collections;

public class TitleAudioManager : MonoBehaviour
{
    public static TitleAudioManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource;
    public float bgmVolume = 0.5f;

    [Header("UI SE")]
    public AudioSource uiSource;
    public float uiVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // BGM 再生（フェードイン）
    public void PlayBGM(AudioClip clip, float fadeDuration)
    {
        if (bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.volume = 0f;
        bgmSource.loop = true;
        bgmSource.Play();
        StartCoroutine(FadeInBGM(fadeDuration));
    }

    private IEnumerator FadeInBGM(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, timer / duration);
            yield return null;
        }
        bgmSource.volume = bgmVolume;
    }
    public void StopBGM(float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutBGM(fadeDuration));
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }
        bgmSource.volume = 0f;
        bgmSource.Stop();
    }

    // UI SE 再生
    public void PlayUISE(AudioClip clip)
    {
        if (clip == null) return;
        uiSource.PlayOneShot(clip, uiVolume);
    }
}
