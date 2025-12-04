using UnityEngine;

public class PushAttack : MonoBehaviour
{
    public Collider attackCollider; // 攻撃判定用
    private EnemyStatus enemyStatus;
    public ParticleSystem areaEffectPrefab; // エリア攻撃エフェクト用
    public Transform footPosition; // エフェクト生成位置

    void Start()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;

        // 攻撃者のステータスを取得（親にある想定）
        enemyStatus = GetComponentInParent<EnemyStatus>();
    }

    public void EnableCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    public void DisableCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            PlayerStatus targetPlayer = other.GetComponent<PlayerStatus>();
            if (targetPlayer != null)
            {
                PlayerController playerController = targetPlayer.GetComponent<PlayerController>();
                if (enemyStatus != null)
                {
                    targetPlayer.TakeDamage(enemyStatus.AttackPower * 0.5f);
                    if (playerController != null)
                    {
                        playerController.OnPushAttackHit(transform); // 引数をTransformに変更
                    }
                    Debug.Log("PushAttack Hit Player!");
                }
            }
        }
    }
    public void OnAreaEfect()
    {
        if (footPosition == null)
        {
            Debug.LogWarning("FootPositionが設定されていません");
            return;
        }
        // パーティクルを生成して再生
        if (areaEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(areaEffectPrefab, footPosition.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }
}
