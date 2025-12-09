using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;

    public Image whitePanel;

    public float fadeSpeed = 1f;

    void Awake()
    {
        Instance = this;
        whitePanel.color = new Color(1, 1, 1, 0);
    }

    public void StartHiddenClearFade()
    {
        StartCoroutine(FadeSequence());
        GameAudioManager.Instance.StopBGM(2f);
    }

    IEnumerator FadeSequence()
    {
        // 白フェードイン
        yield return StartCoroutine(FadeIn(whitePanel));
        SceneManager.LoadScene("ClearScene");
    }

    IEnumerator FadeIn(Image img)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            img.color = new Color(img.color.r, img.color.g, img.color.b, t);
            yield return null;
        }
    }
}

