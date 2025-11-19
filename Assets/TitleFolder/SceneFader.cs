using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] public float fadeDuration = 1.0f;

    private void Start()
    {
        // シーン開始時はフェードイン
        fadeCanvasGroup.alpha = 1;
        fadeCanvasGroup.DOFade(0, fadeDuration)
            .OnComplete(() =>
            {
                fadeCanvasGroup.blocksRaycasts = false; // 入力有効化
            });
    }

    public void FadeToScene(string sceneName)
    {
        // フェードアウト後にシーンをロード
        fadeCanvasGroup.DOFade(1, fadeDuration)
            .OnStart(() =>
            {
                fadeCanvasGroup.blocksRaycasts = true;  // 入力無効化
            })
            .OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
    }
}
