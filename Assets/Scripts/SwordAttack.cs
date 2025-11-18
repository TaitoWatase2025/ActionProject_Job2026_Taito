using Unity.Cinemachine;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Collider attackCollider; // 攻撃判定用
    public GameObject bloodParticlePrefab; // 血しぶきエフェクト用
    public GameObject sparkParticlePrefab; // 火花エフェクト用
    private PlayerStatus playerStatus;
    private EnemyStatus enemyStatus;

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
        // 攻撃対象のステータスを取得
        PlayerStatus targetPlayer = other.GetComponent<PlayerStatus>();
        EnemyStatus targetEnemy = other.GetComponent<EnemyStatus>();
        void SpawnBlood(Vector3 hitpoint)
        {
            if (bloodParticlePrefab != null)
            {
                Vector3 attackDirection = (other.transform.position - transform.position).normalized;// 攻撃方向を計算
                GameObject blood = Instantiate(bloodParticlePrefab, hitpoint, Quaternion.LookRotation(attackDirection));// 血しぶきエフェクトを生成
                Destroy(blood, 2f); // 2秒後にエフェクトを破棄
            }
        }
        void SpawnSpark(Vector3 hitPoint)
        {
            if (sparkParticlePrefab != null)
            {
                Vector3 attackDirection = (other.transform.position - transform.position).normalized;
                GameObject spark = Instantiate(sparkParticlePrefab, hitPoint, Quaternion.LookRotation(attackDirection));
                Destroy(spark, 2f);
            }
        }
        if (playerStatus != null && targetEnemy != null)
        {
            Vector3 hitpoint = other.ClosestPoint(transform.position);
            SpawnBlood(hitpoint);

            HitStop attackerHitStop = playerStatus.GetComponent<HitStop>();
            HitStop enemyHitStop = targetEnemy.GetComponent<HitStop>();
            // プレイヤーの攻撃で敵にダメージ
            targetEnemy.TakeDamage(playerStatus.attackPower, transform.position);
            if (attackerHitStop != null)
            {
                attackerHitStop.StartCoroutine(attackerHitStop.Stop(0.08f));
            }
            if (enemyHitStop != null)
            {
                enemyHitStop.StartCoroutine(enemyHitStop.Stop(0.1f));
            }
            Debug.Log("Enemy Hit!");
            return;
        }
        else if (enemyStatus != null && targetPlayer != null)
        {
            PlayerController playerController = targetPlayer.GetComponent<PlayerController>();
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            if (playerController != null && playerController.isGuarding)
            {
                // ガード中はダメージを軽減
                targetPlayer.TakeDamage(enemyStatus.AttackPower * 0.2f); // 20%のダメージ
                playerController?.OnGuardHit();

                SpawnSpark(hitPoint);
                Animator enemyAnim = enemyStatus.GetComponent<Animator>();
                if (enemyAnim != null)
                {
                    enemyAnim.SetTrigger("AttackBlocked");
                }
                attackCollider.enabled = false; // 攻撃判定を無効化
                Debug.Log("Player Guarded the Attack!");
                return;
            }
            Vector3 hitpoint = other.ClosestPoint(transform.position);
            SpawnBlood(hitpoint);

            // 敵の攻撃でプレイヤーにダメージ
            targetPlayer.TakeDamage(enemyStatus.AttackPower);
            playerController?.OnHit();
            Debug.Log("Player Hit!");
        }
    }
}
