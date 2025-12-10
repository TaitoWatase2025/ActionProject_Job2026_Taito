using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("フェード設定")]
    [SerializeField] private CanvasGroup fadeImageCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    [Header("イントロテキスト設定")]
    [SerializeField] private List<TextMeshProUGUI> introTextLines;
    [SerializeField] private float textDisplayDelay = 2.5f;
    [SerializeField] private float textFadeInDuration = 0.8f;

    [Header("ボタン設定")]
    [SerializeField] private CanvasGroup buttonGroup;
    [SerializeField] private float buttonFadeDuration = 0.5f;
    void Start()
    {
        StartScreenFadeIn();

        StartIntroSequence();
    }
    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration, Action onComplete = null)
    {
        float timer = 0f;
        group.alpha = from;
        group.interactable = group.blocksRaycasts = to > 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, timer / duration);
            yield return null;
        }
        group.alpha = to;
        onComplete?.Invoke();
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float from, float to, float duration)
    {
        float timer = 0f;
        Color c = text.color;
        c.a = from;
        text.color = c;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, timer / duration);
            text.color = c;
            yield return null;
        }
        c.a = to;
        text.color = c;
    }
    public void StartScreenFadeOut(Action onComplete = null) => StartCoroutine(FadeCanvasGroup(fadeImageCanvasGroup, 0f, 1f, fadeDuration, onComplete));
    public void StartScreenFadeIn(Action onComplete = null) => StartCoroutine(FadeCanvasGroup(fadeImageCanvasGroup, 1f, 0f, fadeDuration, onComplete));

    public void StartIntroSequence()
    {
        introTextLines.ForEach(t => t.color = new Color(t.color.r, t.color.g, t.color.b, 0f));
        StartCoroutine(IntroSequenceCoroutine());
    }

    private IEnumerator IntroSequenceCoroutine()
    {
        foreach (var text in introTextLines)
        {
            yield return StartCoroutine(FadeText(text, 0f, 1f, textFadeInDuration));
            yield return new WaitForSeconds(textDisplayDelay);
        }

        if (buttonGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(buttonGroup, 0f, 1f, buttonFadeDuration));

    }
    public void OnContinueButtonClicked() => StartScreenFadeOut(() =>
    {
        SceneManager.LoadScene("GameScene");
    });
}
