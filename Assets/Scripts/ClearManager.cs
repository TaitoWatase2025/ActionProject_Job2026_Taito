using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearManager : MonoBehaviour
{
    [Header("UI")]
    public Image fadeImage;        // フェード用
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public Button TitleButton;
    public Button ExitButton;

    [Header("Audio")]
    public AudioClip clearBGM;
    public AudioClip hoverSE;
    public AudioClip clickSE;
    public float fadeDuration = 2f;

    private void Awake()
    {
        if (TitleButton != null) TitleButton.onClick.AddListener(() => OnButtonPressed(true));
        if (ExitButton != null) ExitButton.onClick.AddListener(() => OnButtonPressed(false));
        AddHoverEvent(TitleButton.gameObject, () => TitleAudioManager.Instance.PlayUISE(hoverSE));
        AddHoverEvent(ExitButton.gameObject, () => TitleAudioManager.Instance.PlayUISE(hoverSE));
        

    }
    private void Start()
    {
        fadeImage.color = Color.white;      
        text1.color = new Color(1, 1, 1, 0); 
        text2.color = new Color(1, 1, 1, 0);
        TitleButton.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(false);

        TitleAudioManager.Instance.PlayBGM(clearBGM, fadeDuration);

        

        // 演出開始
        StartCoroutine(ClearSequence());
    }

    private IEnumerator ClearSequence()
    {
        yield return StartCoroutine(FadeImage(fadeImage, 1f, 0f, fadeDuration));
        yield return StartCoroutine(FadeText(text1, 0f, 1f, 1f));
        yield return StartCoroutine(FadeText(text2, 0f, 1f, 1f));
        TitleButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInCanvasGroup(ExitButton.GetComponent<CanvasGroup>(), 2f));
        yield return StartCoroutine(FadeInCanvasGroup(TitleButton.GetComponent<CanvasGroup>(), 2f));

    }

    public void OnButtonPressed(bool goToTitle)
    {
        StartCoroutine(FadeOutAndLoad(goToTitle));
        TitleAudioManager.Instance.StopBGM(fadeDuration);
        TitleAudioManager.Instance.PlayUISE(clickSE);
    }

    private IEnumerator FadeOutAndLoad(bool goToTitle)
    {
        // 黒フェードイン
        fadeImage.color = new Color(0, 0, 0, 0);
        yield return StartCoroutine(FadeImage(fadeImage, 0f, 1f, fadeDuration));

        if (goToTitle)SceneManager.LoadScene("TitleScene");
        else Application.Quit();
    }

    private IEnumerator FadeImage(Image img, float start, float end, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, timer / duration);
            Color c = img.color;
            c.a = alpha;
            img.color = c;
            yield return null;
        }
        Color final = img.color;
        final.a = end;
        img.color = final;
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float start, float end, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, timer / duration);
            Color c = text.color;
            c.a = alpha;
            text.color = c;
            yield return null;
        }
        Color final = text.color;
        final.a = end;
        text.color = final;
    }
    private IEnumerator FadeInCanvasGroup(CanvasGroup cg, float duration)
    {
        float timer = 0f;
        cg.interactable = false; // フェード中は押せない
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }
        cg.alpha = 1f;
        cg.interactable = true; // 完全表示後に押せるように
        cg.blocksRaycasts = true;
    }
    private void AddHoverEvent(GameObject target, System.Action onHover)
    {
        // 既存の EventTrigger を取得、なければ追加
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }
        // ホバーイベントを追加
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => { onHover(); });
        trigger.triggers.Add(entry);
    }

}
