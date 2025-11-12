using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, Patrol, Chase, Attack, Dodge, Stagger }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DarkSoulsEnemyAI : MonoBehaviour
{
    [Header("‹ŠE")]
    public float viewDistance = 20f;
    public float viewAngle = 120f;

    [Header("UŒ‚‹——£")]
    public float attackRange = 2f;

    [Header("ˆÚ“®‘¬“x")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("QÆ")]
    public Transform player;

    private NavMeshAgent agent;
    private Animator anim;
    private EnemyState state = EnemyState.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.updateRotation = false; // è“®‚Å‰ñ“]
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
            case EnemyState.Dodge:
                // Œã‚Å’Ç‰Á
                break;
            case EnemyState.Stagger:
                // Œã‚Å’Ç‰Á
                break;
        }

        RotateTowardsPlayer();
    }

    private void LookForPlayer()
    {
        if (CanSeePlayer())
        {
            state = EnemyState.Chase;
        }
    }

    private void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.speed = distance > 5f ? runSpeed : walkSpeed;
            agent.SetDestination(player.position);
            anim.SetFloat("Speed", agent.speed / runSpeed); // 0~1‚ÅAnimator‚É‘—‚é
        }
        else
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0f);
            state = EnemyState.Attack;
        }
    }

    private void AttackPlayer()
    {
        // UŒ‚ƒAƒjƒ[ƒVƒ‡ƒ“Ä¶
        anim.SetTrigger("Attack");
        state = EnemyState.Chase; // UŒ‚Œã‚ÉÄ‚Ñ’ÇÕ
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

        // áŠQ•¨ƒ`ƒFƒbƒN
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, dir.normalized, out RaycastHit hit, viewDistance))
        {
            if (hit.transform == player) return true;
        }

        return false;
    }
}

