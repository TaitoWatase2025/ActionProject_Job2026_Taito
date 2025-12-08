using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup fadeCanvasGroup;
    public TMP_Text errorText;

    [Header("設定")]
    public string titleSceneName = "TitleScene";
    public float fadeDuration = 1f;
    public float typeDelay = 0.03f;
    public float textHoldTime = 1f;
    public float dotInterval = 0.5f;
    public int maxDots = 3;
    public float disconnectDuration = 3f;

    [Header("グリッチ設定")]
    public float glitchChance = 0.2f; // 20%の確率でランダム文字
    public string glitchChars = "#$%&*@!?<>"; // グリッチ文字

    private void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.DOFade(0f, fadeDuration)
                .OnComplete(() => fadeCanvasGroup.blocksRaycasts = false);
        }

        if (errorText != null)
            errorText.alpha = 0f;
    }

    public void HandlePlayerDeath()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (errorText == null) yield break;

        errorText.alpha = 1f;

        // 一行目
        string firstLine = "!! PLAYER TERMINATED !!";
        yield return StartCoroutine(TypewriterGlitchDisplay(errorText, firstLine, typeDelay));

        yield return new WaitForSeconds(textHoldTime);

        // 二行目 Disconnecting…アニメ
        string baseText = "Disconnecting";
        string fullText = firstLine + "\n"; // 一行目固定
        int dotCount = 0;
        float elapsed = 0f;

        while (elapsed < disconnectDuration)
        {
            dotCount = (dotCount + 1) % (maxDots + 1);
            string dots = new string('.', dotCount);

            // グリッチ適用
            string glitchDots = "";
            foreach (char c in dots)
            {
                if (Random.value < glitchChance)
                    glitchDots += glitchChars[Random.Range(0, glitchChars.Length)];
                else
                    glitchDots += c;
            }

            errorText.text = fullText + baseText + glitchDots;

            yield return new WaitForSeconds(dotInterval);
            elapsed += dotInterval;
        }

        // フェードアウト
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            yield return fadeCanvasGroup.DOFade(1f, fadeDuration).WaitForCompletion();
        }

        SceneManager.LoadScene(titleSceneName);
    }

    private IEnumerator TypewriterGlitchDisplay(TMP_Text textComponent, string message, float delay)
    {
        textComponent.text = "";
        System.Text.StringBuilder displayed = new System.Text.StringBuilder();

        foreach (char c in message)
        {
            char displayChar = c;

            if (Random.value < glitchChance && !char.IsWhiteSpace(c))
            {
                displayChar = glitchChars[Random.Range(0, glitchChars.Length)];
            }

            displayed.Append(displayChar);
            textComponent.text = displayed.ToString();

            yield return new WaitForSeconds(delay);
        }
    }
}








