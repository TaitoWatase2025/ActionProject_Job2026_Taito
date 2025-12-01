using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public Collider attackCollider; // 攻撃判定用
    private EnemyStatus enemyStatus;
    private PlayerStatus playerStatus;
    public ParticleSystem areaEffectPrefab1; // エリア攻撃エフェクト用
    public ParticleSystem areaEffectPrefab2; // エリア攻撃エフェクト用
    public Transform footPosition; // エフェクト生成位置

    void Start()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;

        // 攻撃者のステータスを取得（親にある想定）
        playerStatus = GetComponentInParent<PlayerStatus>();
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
            PlayerController playerController = targetPlayer.GetComponent<PlayerController>();
            if (enemyStatus != null && targetPlayer != null)
            {
                if (playerController != null && playerController.isGuarding)
                {
                    targetPlayer.TakeDamage(enemyStatus.AttackPower * 0.5f);
                    playerController?.OnAreaAttackGuardHit();
                    return;
                }
                targetPlayer.TakeDamage(enemyStatus.AttackPower);
                playerController?.OnAreaAttackHit();
                Debug.Log("areaAttack Hit Player!");
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

        // パーティクル1を生成して再生
        if (areaEffectPrefab1 != null)
        {
            ParticleSystem effect1 = Instantiate(areaEffectPrefab1, footPosition.position, Quaternion.identity);
            effect1.Play();
        }

        // パーティクル2を生成して再生
        if (areaEffectPrefab2 != null)
        {
            ParticleSystem effect2 = Instantiate(areaEffectPrefab2, footPosition.position, Quaternion.identity);
            effect2.Play();
        }
    }
}
