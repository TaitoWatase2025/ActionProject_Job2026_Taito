using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class DeathManager : MonoBehaviour
{
    public Image fadeImage;         // ブラックアウト用
    public TMP_Text errorText;      // エラー文字表示用
    public string titleSceneName = "Title"; // タイトル画面のScene名
    public float fadeDuration = 1f; // ブラックアウト時間
    public float textDisplayDuration = 2f; // エラー文字表示時間

    public void HandlePlayerDeath(string errorMessage)
    {
        StartCoroutine(DeathSequence(errorMessage));
    }

    private IEnumerator DeathSequence(string errorMessage)
    {
        // 1. エラー文字を表示
        if (errorText != null)
        {
            errorText.text = errorMessage;
            errorText.alpha = 1f;
        }

        yield return new WaitForSeconds(textDisplayDuration);

        // 2. フェードアウト
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }
        }

        // 3. タイトル画面に戻る
        SceneManager.LoadScene(titleSceneName);
    }
}


