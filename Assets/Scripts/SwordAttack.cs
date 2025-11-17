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
            // プレイヤーの攻撃で敵にダメージ
            targetEnemy.TakeDamage(playerStatus.attackPower);
            Debug.Log("Enemy Hit!");
        }
        else if (enemyStatus != null && targetPlayer != null)
        {
            // 敵の攻撃でプレイヤーにダメージ
            targetPlayer.TakeDamage(enemyStatus.AttackPower);
            targetPlayer.GetComponent<PlayerController>().OnHit();
            Debug.Log("Player Hit!");

            
        }
    }
}
