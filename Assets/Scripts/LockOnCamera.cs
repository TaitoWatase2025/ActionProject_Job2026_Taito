using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LockOnCamera : MonoBehaviour
{
    [Header("Targets & Player")]
    public Transform player;
    public LayerMask enemyLayer;
    public float searchRadius = 20f;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 2, -6); // Player背後
    public float moveSmoothTime = 0.2f;
    public float rotateSpeed = 5f;

    [HideInInspector] public bool isLockOn = false;

    private Transform currentTarget;
    private Vector3 velocity;

    void LateUpdate()
    {
        if (isLockOn) UpdateLockOnTarget();
        else currentTarget = null;

        MoveCamera();
        LookAtTarget();
    }

    // 左右切替用
    public void SwitchTarget(int dir)
    {
        if (!isLockOn || currentTarget == null) return;

        Collider[] hits = Physics.OverlapSphere(player.position, searchRadius, enemyLayer);
        List<Transform> enemies = hits.Select(h => h.transform).Where(e => e != currentTarget).ToList();
        if (enemies.Count == 0) return;

        Transform best = null;
        float bestDot = -Mathf.Infinity;
        Vector3 right = (currentTarget.position - player.position).normalized;
        right = Vector3.Cross(Vector3.up, right) * dir;

        foreach (var e in enemies)
        {
            Vector3 dirToEnemy = (e.position - player.position).normalized;
            float dot = Vector3.Dot(right, dirToEnemy);
            if (dot > bestDot)
            {
                bestDot = dot;
                best = e;
            }
        }

        if (best != null) currentTarget = best;
    }

    // 最も近い敵を注視
    void UpdateLockOnTarget()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, searchRadius, enemyLayer);
        List<Transform> enemies = hits.Select(h => h.transform).ToList();

        if (enemies.Count == 0)
        {
            isLockOn = false;
            currentTarget = null;
            return;
        }

        if (currentTarget == null || !enemies.Contains(currentTarget))
        {
            currentTarget = enemies
                .OrderBy(e => Vector3.Distance(player.position, e.position))
                .First();
        }
    }

    void MoveCamera()
    {
        if (!isLockOn)
        {
            // 通常カメラ位置
            transform.position = player.position + offset;
            return;
        }

        if (currentTarget == null) return;

        // Player の背後に固定
        Vector3 behindPos = player.position - player.forward * Mathf.Abs(offset.z) + Vector3.up * offset.y;
        transform.position = Vector3.SmoothDamp(transform.position, behindPos, ref velocity, moveSmoothTime);
    }

    void LookAtTarget()
    {
        if (!isLockOn || currentTarget == null)
        {
            // 通常は Player を向く
            Vector3 lookPos = player.position + Vector3.up * 1.5f;
            transform.LookAt(lookPos);
            return;
        }

        // ロックオン時は Enemy を画面中央に
        Vector3 lookPosEnemy = currentTarget.position + Vector3.up * 1.5f;
        Vector3 dir = (lookPosEnemy - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }
}


