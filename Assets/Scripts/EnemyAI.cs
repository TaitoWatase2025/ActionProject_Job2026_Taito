using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum EnemyState { Chase, Attack, Falling, Die }

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
    public float minRange = 1.5f;
    public float shortRange = 3f;
    public float attackAngle = 30f;
    public float attackCooldown = 3f;
    private float lastAttackTime = 0f;

    private enum LastAction { Attack, ShortAttack, BackJump, AreaAttack }
    private LastAction lastAction;
    private bool isAttacking = false;

    [Header("落下")]
    public float gravity = 9f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float LandingDelay = 0.5f;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private EnemyStatus Status;

    private bool isGrounded;
    private float verticalVelocity = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Status = GetComponent<EnemyStatus>();
        agent.stoppingDistance = 1.5f;
        lastAction = LastAction.AreaAttack;

        if (!agent.isOnNavMesh)
            agent.enabled = false;

        if (Status != null)
            Status.OnDeath += HandleDeath;
    }

    void Update()
    {
        if (state == EnemyState.Die) return;
        if (!agent.enabled || state == EnemyState.Falling)
        {
            CheckGround();
            ApplyGravity();
        }
        anim.SetFloat("Speed", agent.velocity.magnitude);

        switch (state)
        {
            case EnemyState.Chase:
                if (agent.isOnNavMesh) ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
            case EnemyState.Falling:
                break;
            case EnemyState.Die:
                return;
        }
    }

    #region 落下・地面判定
    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f,
                                     Vector3.down,
                                     groundCheckDistance + 0.1f,
                                     groundLayer);

        if (isGrounded && state == EnemyState.Falling)
        {
            // 着地処理は ApplyGravity 内で行う
            verticalVelocity = 0f;
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
        if (IsPlayerInAttackRange()) state = EnemyState.Attack;
    }

    private IEnumerator WaitForNavMesh()
    {
        while (!agent.isOnNavMesh)
            yield return null;

        agent.isStopped = false;
        state = EnemyState.Chase;
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                Vector3.down,
                out RaycastHit hitinfo,
                1.2f,
                groundLayer))
            {
                transform.position = hitinfo.point + Vector3.up * 0.05f;
                verticalVelocity = 0f;
                isGrounded = true;
                if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                }
                if (state == EnemyState.Falling)
                {
                    anim.SetBool("IsFalling", false);
                    anim.SetTrigger("Land");

                    if (!agent.enabled) agent.enabled = true;
                    agent.isStopped = true;
                    StartCoroutine(ReturnToChaseAfterLanding());
                }
                return;
            }
            if (agent.isOnNavMesh)
                agent.isStopped = true;
        }
    }
    #endregion

    #region 追跡
    void ChasePlayer()
    {
        if (!agent.enabled) return;

        agent.destination = player.position;
        if (IsPlayerInAttackRange())
            state = EnemyState.Attack;
    }
    #endregion

    #region 攻撃
    void AttackPlayer()
    {
        if (state == EnemyState.Die) return;

        agent.isStopped = isAttacking;

        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);


        if ((float)Status.health / Status.maxHealth <= 0.8f &&
            distance < shortRange &&
            //Random.value < 0.5f &&
            lastAction != LastAction.AreaAttack)
        {
            anim.SetTrigger("AreaAttack");
            lastAction = LastAction.AreaAttack;
            isAttacking = true;
            return;
        }
        if (distance < minRange)
        {
            if (Random.value < 0.5f && lastAction != LastAction.BackJump && Time.time - lastAttackTime >= attackCooldown)
            {
                anim.SetTrigger("BackJump");
                lastAction = LastAction.BackJump;
                lastAttackTime = Time.time;
                isAttacking = true;
                return;
            }
            else if (lastAction != LastAction.ShortAttack && angle < attackAngle && Time.time - lastAttackTime >= attackCooldown)
            {
                anim.SetTrigger("ShortAttack");
                lastAction = LastAction.ShortAttack;
                lastAttackTime = Time.time;
                isAttacking = true;
                return;
            }
        }

        if (distance > attackRange &&
            angle < attackAngle &&
            Time.time - lastAttackTime >= attackCooldown &&
            lastAction != LastAction.Attack)
        {
            anim.SetTrigger("Attack");
            lastAction = LastAction.Attack;
            lastAttackTime = Time.time;
            isAttacking = true;

        }

        if (!IsPlayerInAttackRange() && !isAttacking)
        {
            state = EnemyState.Chase;
            agent.isStopped = false;
        }
    }

    bool IsPlayerInAttackRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Attack を発動する距離範囲
        if (distance > attackRange) return false;

        // 正面判定
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= attackAngle;
    }

    public void OnAttackEnd()
    {
        if (lastAction == LastAction.BackJump) return;
        isAttacking = false;
        if (state == EnemyState.Attack)
        {
            state = EnemyState.Chase;
            agent.isStopped = false;
        }
    }

    
    #endregion

    #region 死亡判定
    private void HandleDeath()
    {
        state = EnemyState.Die;
        agent.enabled = false;
        anim.SetTrigger("Die");
        StartCoroutine(RemoveAfterDelay(3f));
    }

    private IEnumerator RemoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    #endregion
}

