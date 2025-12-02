using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleMenu : MonoBehaviour
{
    public CanvasGroup optionButtonGroup;
    public CanvasGroup volumeSliderGroup;
    public Slider volumeSlider;
    public float fadeDuration = 0.5f;

    public void OnClickStart()
    {
        var sceneFader = FindFirstObjectByType<SceneFader>();

        if (sceneFader != null)
        {
            sceneFader.FadeToScene("TestScene");
        }
    }
    public void OnClickExit()
    {
        StartCoroutine(ExitWithFade());
    }
    private IEnumerator ExitWithFade()
    {
        SceneFader sceneFader = FindFirstObjectByType<SceneFader>();

        // フェードアウト開始
        sceneFader.FadeToScene(""); //フェードのみ実行するため、シーン名は空文字列にする

        // フェードが終わるまで待つ
        // SceneFaderにフェード時間のプロパティがある場合はそちらを利用
        float fadeDuration = sceneFader.fadeDuration; 
        yield return new WaitForSeconds(fadeDuration);

        // フェード後にアプリ終了
        Application.Quit();

#if UNITY_EDITOR
        // エディタ上では停止
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnClickOption()
    {
        // Option ボタンをフェードアウト
        StartCoroutine(FadeCanvasGroup(optionButtonGroup, 1f, 0f, fadeDuration, () =>
        {
            optionButtonGroup.interactable = false;
            optionButtonGroup.blocksRaycasts = false;
        }));

        // 音量スライダーをフェードイン
        StartCoroutine(FadeCanvasGroup(volumeSliderGroup, 0f, 1f, fadeDuration, () =>
        {
            volumeSliderGroup.interactable = true;
            volumeSliderGroup.blocksRaycasts = true;
        }));
        // 音量スライダーをフェードイン
        StartCoroutine(FadeCanvasGroup(volumeSliderGroup, 0f, 1f, fadeDuration, () =>
        {
            volumeSliderGroup.interactable = true;
            volumeSliderGroup.blocksRaycasts = true;
        }));

        // スライダー初期値を AudioManager の BGM 音量に設定
        if (volumeSlider != null)
        {
            volumeSlider.value = TitleAudioManager.Instance.bgmVolume;
            volumeSlider.onValueChanged.AddListener((v) =>
            {
                TitleAudioManager.Instance.bgmSource.volume = v;
                TitleAudioManager.Instance.bgmVolume = v;
            });
        }

    }
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration, System.Action onComplete = null)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, timer / duration);
            yield return null;
        }
        cg.alpha = end;
        onComplete?.Invoke();
    }
}
