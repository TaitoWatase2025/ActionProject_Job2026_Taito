using UnityEngine;
using UnityEngine.UI;

public class EnemyHPManager : MonoBehaviour
{
    [Header("UIパーツ")]
    [SerializeField] private Image hpFill;
    [SerializeField] private Image hpDelayed;

    [Header("設定")]
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    private float targetFill;
    private Transform enemyTransform;
    private EnemyStatus enemyStatus;

    public void Initialize(EnemyStatus enemy)
    {
        enemyStatus = enemy;
        enemyTransform = enemy.transform;

        // HP変化時にUI更新
        enemy.OnHealthChanged += UpdateHP;

        // 初期HP反映
        UpdateHP(enemy.health / enemy.maxHealth);

        // 敵が死んだらHPバー削除
        enemy.OnDeath += DestroySelf;
    }

    private void Update()
    {
        // 遅延バー補間
        if (hpDelayed != null)
            hpDelayed.fillAmount = Mathf.Lerp(hpDelayed.fillAmount, targetFill, Time.deltaTime * lerpSpeed);

        // 敵の頭上に追従（Overlay用）
        if (enemyTransform != null)
            transform.position = Camera.main.WorldToScreenPoint(enemyTransform.position + offset);
    }

    private void UpdateHP(float fillAmount)
    {
        targetFill = fillAmount;
        if (hpFill != null)
            hpFill.fillAmount = fillAmount;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (enemyStatus != null)
        {
            enemyStatus.OnHealthChanged -= UpdateHP;
            enemyStatus.OnDeath -= DestroySelf;
        }
    }
}


