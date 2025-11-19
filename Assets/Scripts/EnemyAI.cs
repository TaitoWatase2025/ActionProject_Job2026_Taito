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
    public float attackRange = 5f;
    public float minAttackRange = 2f;
    public float shortAttackRange = 4f;
    public float attackAngle = 60f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("パトロール")]
    public float patrolRadius = 5f;
    public float patrolWaitTime = 2f;

    [Header("落下")]
    public float gravity = 9.8f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private Vector3 patrolTarget;
    private float waitTimer = 0f;

    private bool isGrounded;
    private float verticalVelocity = 0f;

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

        // 落下と地面判定
        CheckGround();
        ApplyGravity();

        // ステートごとの処理
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

    #region 落下・地面判定
    void CheckGround()
    {
        // 足元からRayを飛ばして地面判定
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f, groundLayer);

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
            anim.SetBool("IsFalling", false);
            anim.SetTrigger("Land");
        }
        else
        {
            anim.SetBool("IsFalling", true);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
        }
    }
    #endregion

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

        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (distance < minAttackRange)
        {
            anim.SetTrigger("BackJump");
            return;
        }
        if (distance < shortAttackRange && angle <= attackAngle / 1.5f)
        {
            anim.SetTrigger("ShortAttack");
            return;
        }

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
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        return angle <= attackAngle / 1.5f;
    }
    #endregion
}




