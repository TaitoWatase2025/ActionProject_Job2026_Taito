using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    [Header("UIパーツ")]
    [SerializeField] private Image hpFill;      // 通常HPバー
    [SerializeField] private Image hpDelayed;   // 遅延バー（任意）

    [Header("設定")]
    [SerializeField] private float lerpSpeed = 5f; // 遅延バーの補間速度

    private float targetFill;

    private void Start()
    {
        // PlayerStatusを取得
        PlayerStatus playerStatus = Object.FindFirstObjectByType<PlayerStatus>();
        if (playerStatus != null)
        {
            // HP変化イベントに登録
            playerStatus.OnHealthChanged += UpdateHP;
            // 初期値を設定
            UpdateHP(playerStatus.health / playerStatus.maxHealth);
        }
    }

    private void Update()
    {
        if (hpDelayed != null)
        {
            hpDelayed.fillAmount = Mathf.Lerp(hpDelayed.fillAmount, targetFill, Time.deltaTime * lerpSpeed);
        }
    }

    private void UpdateHP(float fillAmount)
    {
        targetFill = fillAmount;
        if (hpFill != null)
            hpFill.fillAmount = fillAmount;
    }

    private void OnDestroy()
    {
        // 忘れずにイベント解除
        PlayerStatus playerStatus = Object.FindFirstObjectByType<PlayerStatus>();
        if (playerStatus != null)
            playerStatus.OnHealthChanged -= UpdateHP;
    }
}


