using UnityEngine;
using System.Collections;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource;
    public float bgmVolume = 0.5f;

    [Header("SE Sources")]
    public AudioSource sfxSource;  // 足音・着地・攻撃などの汎用SE
    public float sfxVolume = 1f;

    [Header("Footstep Clips")]
    public AudioClip playerWalkFootstep;
    public AudioClip playerRunFootstep;
    public AudioClip enemyWalkFootstep;
    public AudioClip enemyRunFootstep;

    [Header("Landing Clips")]
    public AudioClip playerLand;
    public AudioClip enemyLand;

    [Header("Attack Clips")]
    public AudioClip weaponHit;
    public AudioClip weaponMiss;
    public AudioClip guardHit;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            bgmVolume= PlayerPrefs.GetFloat("BGM_VOLUME", 0.5f);// 前回の設定を読み込み
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #region BGM Methods
    public void PlayBGM(AudioClip clip, float fadeDuration = 1f)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.volume = 0f;
        bgmSource.loop = true;
        bgmSource.Play();
        StartCoroutine(FadeInBGM(fadeDuration));
    }

    public void StopBGM(float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutBGM(fadeDuration));
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
    #endregion
    #region SE Methods
    public void PlaySFX(AudioClip clip, Vector3? position = null)
    {
        if (clip == null) return;

        if (position.HasValue)
        {
            // 3Dサウンドとして再生
            AudioSource.PlayClipAtPoint(clip, position.Value, sfxVolume);
        }
        else
        {
            // 2Dサウンドとして再生
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }
    #endregion
    #region Helper Methods
    public void PlayFootstep(bool isPlayer, bool isRunning, Vector3 position)
    {
        AudioClip clip = null;
        if (isPlayer)
            clip = isRunning ? playerRunFootstep : playerWalkFootstep;
        else
            clip = isRunning ? enemyRunFootstep : enemyWalkFootstep;

        PlaySFX(clip, position);
    }

    public void PlayLanding(bool isPlayer, Vector3 position)
    {
        AudioClip clip = isPlayer ? playerLand : enemyLand;
        PlaySFX(clip, position);
    }

    public void PlayAttackHit(bool hit, Vector3 position)
    {
        PlaySFX(hit ? weaponHit : weaponMiss, position);
    }

    public void PlayGuardHit(Vector3 position)
    {
        PlaySFX(guardHit, position);
    }
}
#endregion
