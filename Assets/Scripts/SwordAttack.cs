using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Collider attackCollider; // 攻撃判定用
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

        if (playerStatus != null && targetEnemy != null)
        {
            HitStop attackerHitStop = playerStatus.GetComponent<HitStop>();
            HitStop enemyHitStop = targetEnemy.GetComponent<HitStop>();
            // プレイヤーの攻撃で敵にダメージ
            targetEnemy.TakeDamage(playerStatus.attackPower);
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
            if (playerController != null && playerController.isGuarding)
            {
                // ガード中はダメージを軽減
                targetPlayer.TakeDamage(enemyStatus.AttackPower * 0.2f); // 20%のダメージ
                playerController?.OnGuardHit();
                Animator enemyAnim = enemyStatus.GetComponent<Animator>();
                if (enemyAnim != null)
                {
                    enemyAnim.SetTrigger("AttackBlocked");
                }
                attackCollider.enabled = false; // 攻撃判定を無効化
                Debug.Log("Player Guarded the Attack!");
                return;
            }
            // 敵の攻撃でプレイヤーにダメージ
            targetPlayer.TakeDamage(enemyStatus.AttackPower);
            playerController?.OnHit();
            Debug.Log("Player Hit!");
        }
    }
}
