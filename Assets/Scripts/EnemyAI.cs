using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum EnemyState {  Chase, Attack, Falling }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("ステート設定")]
    public EnemyState state = EnemyState.Chase;

    [Header("プレイヤー検知")]
    public float viewRadius = 1000f;
    public float viewAngle = 360f;
    public LayerMask obstacleLayer;

    [Header("攻撃")]
    public float attackRange = 5f;
    public float minAttackRange = 2f;
    public float shortAttackRange = 4f;
    public float attackAngle = 60f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    //[Header("パトロール")]
    //public float patrolRadius = 5f;
    //public float patrolWaitTime = 2f;

    [Header("落下")]
    public float gravity = 9f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float LandingDelay = 0.5f;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    //private Vector3 patrolTarget;
    //private float waitTimer = 0f;

    private bool isGrounded;
    private float verticalVelocity = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if(!agent.isOnNavMesh)
        {
            agent.enabled = false;
        }
    }

    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);

        if (!agent.enabled)
        {
            // 落下と地面判定
            CheckGround();
            ApplyGravity();
            return;
        }
        // ステートごとの処理
        switch (state)
        {
            case EnemyState.Chase: if (agent.isOnNavMesh) ChasePlayer(); break;
            case EnemyState.Attack: AttackPlayer(); break;
            case EnemyState.Falling: break;
        }
    }

    #region 落下・地面判定
    void CheckGround()
    {
        // 足元からRayを飛ばして地面判定
        isGrounded = Physics.Raycast
            (
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            groundCheckDistance + 0.1f,
            groundLayer
            );

        if (isGrounded)
        {
            if (state == EnemyState.Falling)
            {
                verticalVelocity = 0f;
                anim.SetBool("IsFalling", false);
                anim.SetTrigger("Land");

                if(NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;

                    if(!agent.enabled) agent.enabled = true;
                    agent.isStopped = true;
                    StartCoroutine(ReturnToChaseAfterLanding());
                }
                else
                {
                    StartCoroutine(WaitForNavMesh());
                }

            }
        }
        else
        {
            if (state != EnemyState.Falling)
            {
                state = EnemyState.Falling;
                if (agent.isOnNavMesh) agent.isStopped = true;
                anim.SetBool("IsFalling", true);
            }
        }
    }
    private IEnumerator ReturnToChaseAfterLanding()
    {
        yield return new WaitForSeconds(LandingDelay);

        agent.isStopped = false;
        state = EnemyState.Chase;
    }

    private IEnumerator WaitForNavMesh()
    {
        while(!agent.isOnNavMesh)
            yield return null;

        agent.isStopped = false;
        state=EnemyState.Chase;

    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

            if(agent.isOnNavMesh)
                agent.isStopped = true; 
        }
    }
    #endregion

    //#region パトロール
    //void Patrol()
    //{
    //    if (!agent.isOnNavMesh) return;
    //    if (!agent.pathPending && agent.remainingDistance < 0.5f)
    //    {
    //        waitTimer += Time.deltaTime;
    //        if (waitTimer >= patrolWaitTime)
    //        {
    //            SetRandomPatrolTarget();
    //            waitTimer = 0f;
    //        }
    //    }
    //}

    //#endregion

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
        if(!agent.enabled) return;

        agent.destination = player.position;
        if(IsPlayerInAttackRange())
        {
            state = EnemyState.Attack;
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




