using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Patrol, Chase, Attack }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("ステート設定")]
    public EnemyState state = EnemyState.Patrol;

    [Header("プレイヤー検知")]
    public float viewRadius = 10f;
    public float viewAngle = 120f;
    public LayerMask obstacleLayer;

    [Header("攻撃")]
    public float attackRange = 5f;       // 攻撃距離
    public float minAttackRange = 2f;    // 近すぎた際
    public float shortAttackRange = 4f; // 近距離攻撃距離
    public float attackAngle = 60f;      // 正面攻撃範囲（度）
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("パトロール")]
    public float patrolRadius = 5f;
    public float patrolWaitTime = 2f;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private Vector3 patrolTarget;
    private float waitTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SetRandomPatrolTarget();
    }

    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);

        switch (state)
        {
            case EnemyState.Patrol:
                Patrol();
                LookForPlayer();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
        }
    }

    #region パトロール
    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                SetRandomPatrolTarget();
                waitTimer = 0f;
            }
        }
    }

    void SetRandomPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            agent.destination = patrolTarget;
        }
    }
    #endregion

    #region プレイヤー検知
    void LookForPlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= viewRadius &&
            !Physics.Raycast(transform.position, dirToPlayer, distance, obstacleLayer))
        {
            state = EnemyState.Chase;
        }
    }
    #endregion

    #region 追跡
    void ChasePlayer()
    {
        agent.destination = player.position;

        if (IsPlayerInAttackRange())
        {
            state = EnemyState.Attack;
        }
        else if (Vector3.Distance(transform.position, player.position) > viewRadius)
        {
            state = EnemyState.Patrol;
            SetRandomPatrolTarget();
        }
    }
    #endregion

    #region 攻撃
    void AttackPlayer()
    {
        agent.destination = transform.position; // 攻撃中は停止

        //HPが10％減ると発動


        //近い場合は後退
        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (distance < minAttackRange)//近すぎたら後退
        {
            anim.SetTrigger("BackJump");
            return;
        }
        if (distance < shortAttackRange && angle <= attackAngle / 1.5f)//近距離攻撃
        {
            anim.SetTrigger("ShortAttack");
            return;
        }


        agent.destination = transform.position;//停止

        if (IsPlayerInAttackRange() && Time.time - lastAttackTime >= attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }

        if (!IsPlayerInAttackRange())
        {
            state = EnemyState.Chase;
        }
    }

    bool IsPlayerInAttackRange()
    {
        // 距離判定
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange) return false;

        // 正面判定
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle <= attackAngle / 1.5f)
            return true;

        return false;
    }
    #endregion
}



