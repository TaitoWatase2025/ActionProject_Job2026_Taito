using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public AudioClip titleBGM;
    public AudioClip hoverSE;
    public AudioClip clickSE;
    public AudioClip sliderSE; // 追加：スライダー音

    public Button startButton;
    public Button optionButton;
    public Button exitButton;
    public Slider volumeSlider; // 追加：音量スライダー

    public float fadeDuration = 1f;

    private void Start()
    {
        // BGM をフェードイン
        TitleAudioManager.Instance.PlayBGM(titleBGM, 1f);

        // Start ボタン
        startButton.onClick.AddListener(() =>
        {
            TitleAudioManager.Instance.PlayUISE(clickSE);
            TitleAudioManager.Instance.StopBGM(fadeDuration);
        });
        AddHoverEvent(startButton.gameObject, () =>
        {
            TitleAudioManager.Instance.PlayUISE(hoverSE);
        });

        // Option ボタン
        optionButton.onClick.AddListener(() => TitleAudioManager.Instance.PlayUISE(clickSE));
        AddHoverEvent(optionButton.gameObject, () =>
        {
            TitleAudioManager.Instance.PlayUISE(hoverSE);
        });

        // Exit ボタン
        exitButton.onClick.AddListener(() =>
        {
            TitleAudioManager.Instance.PlayUISE(clickSE);
            TitleAudioManager.Instance.StopBGM(fadeDuration);
        });
        AddHoverEvent(exitButton.gameObject, () =>
        {
            TitleAudioManager.Instance.PlayUISE(hoverSE);
        });

        // --- スライダー音 ---
        if (volumeSlider != null)
        {
            // 初期値を BGM 音量に設定
            volumeSlider.value = TitleAudioManager.Instance.bgmVolume;

            // 値が変わるたびに音を鳴らす
            volumeSlider.onValueChanged.AddListener((v) =>
            {
                TitleAudioManager.Instance.bgmSource.volume = v;
                TitleAudioManager.Instance.bgmVolume = v;

                // カチカチ音を再生
                TitleAudioManager.Instance.PlayUISE(sliderSE);
            });
        }
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
