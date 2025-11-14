using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, Patrol, Chase, Attack }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DarkSoulsEnemyAI : MonoBehaviour
{
    [Header("視界")]
    public float viewDistance = 20f;
    public float viewAngle = 120f;

    [Header("攻撃距離")]
    public float attackRange = 2f;

    [Header("移動速度")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("参照")]
    public Transform player;

    private NavMeshAgent agent;
    private Animator anim;
    private EnemyState state = EnemyState.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.updateRotation = false; // 手動で回転
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Idle:
            case EnemyState.Patrol:
                LookForPlayer();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
        }

        RotateTowardsPlayer();
    }

    private void LookForPlayer()
    {
        if (CanSeePlayer())
            state = EnemyState.Chase;
    }

    private void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.speed = distance > 5f ? runSpeed : walkSpeed;
            agent.SetDestination(player.position);

            // 移動アニメーション
            anim.SetFloat("Speed", agent.speed / runSpeed);
        }
        else
        {
            agent.isStopped = true;
            state = EnemyState.Attack;
        }
    }

    private void AttackPlayer()
    {
        // 攻撃アニメーション
        anim.SetTrigger("Attack");

        // 攻撃後はIdleに戻る
        state = EnemyState.Idle;
    }

    // 攻撃終了時にAnimation Eventで呼ぶ（必要に応じて）
    public void OnAttackEnd()
    {
        anim.SetTrigger("Idle");
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction.magnitude > 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;
        Vector3 dir = player.position - transform.position;
        if (dir.magnitude > viewDistance) return false;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > viewAngle / 2f) return false;

        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, dir.normalized, out RaycastHit hit, viewDistance))
        {
            if (hit.transform == player) return true;
        }

        return false;
    }
}
