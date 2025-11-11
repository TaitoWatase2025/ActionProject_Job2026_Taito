using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("視界設定")]
    public float viewDistance = 20f;   // プレイヤーを認識する距離
    public float viewAngle = 120f;     // 視界の角度
    public Transform player;           // プレイヤーのTransform

    [Header("移動速度設定")]
    public float walkSpeed = 3f;       // 近距離で歩く速度
    public float runSpeed = 6f;        // 中距離で走る速度
    public float sprintSpeed = 10f;    // 遠距離でスプリント

    private NavMeshAgent agent;
    private Animator anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        anim = GetComponent<Animator>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // NavMeshAgentの基本設定
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;
    }

    private void Update()
    {
        if (PlayerInSight())
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // 距離に応じて速度を変更
            if (distance > 15f)
            {
                // 遠距離ではスプリントアニメーションを再生
                if (anim != null)
                    anim.SetFloat("Speed", 1.5f); // スプリント速度に応じたアニメーション速度
                agent.speed = sprintSpeed;
            }
                
            else if (distance > 5f)
            {
                // 中距離では走るアニメーションを再生
                if (anim != null)
                    anim.SetFloat("Speed", 1.0f); // 走る速度に応じたアニメーション速度
                agent.speed = runSpeed;
            }
                
            else
            {
                // 近距離では歩くアニメーションを再生
                if (anim != null)
                    anim.SetFloat("Speed", 0.5f); // 歩く速度に応じたアニメーション速度
                agent.speed = walkSpeed;
            }
                

            agent.SetDestination(player.position);// プレイヤーに向かって移動
        }
        else
        {
            // プレイヤーを見失った場合は止まる
            agent.SetDestination(transform.position);
        }
    }

    private bool PlayerInSight()
    {
        if (player == null) return false;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        if (distance > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > viewAngle / 2f) return false;

        // 障害物の判定（壁などを考慮）
        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, direction.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, viewDistance))
        {
            if (hit.transform == player)
                return true; // プレイヤーを発見
        }

        return false;
    }
}

