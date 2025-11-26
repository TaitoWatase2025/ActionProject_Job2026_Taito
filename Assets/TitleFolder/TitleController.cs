using UnityEngine;
using System.Collections;

public class TitleMenu : MonoBehaviour
{
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
        sceneFader.FadeToScene(""); // 空文字でもOK、Fade処理だけ使う場合

        // フェードが終わるまで待つ
        // SceneFaderにフェード時間のプロパティがある場合はそちらを利用
        float fadeDuration = sceneFader.fadeDuration; // 例：public float fadeDuration = 1f;
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
        
    }
}
