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
    public Vector3 offset = new Vector3(0, 2, -6); // Playerîwå„
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

    // ç∂âEêÿë÷óp
    public void SwitchTarget(int dir)
    {
        Collider[] hits = Physics.OverlapSphere(player.position, searchRadius, enemyLayer);
        List<Transform> enemies = hits.Select(h => h.transform).ToList();
        if (enemies.Count <= 1) return;

        Transform best = null;
        float bestDot = -Mathf.Infinity;
        Vector3 playerForward = (currentTarget.position - player.position).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, playerForward) * dir;

        foreach (var e in enemies)
        {
            if (e == currentTarget) continue;
            float dot = Vector3.Dot(right, (e.position - player.position).normalized);
            if (dot > bestDot)
            {
                bestDot = dot;
                best = e;
            }
        }

        if (best != null) currentTarget = best;
    }

    // ç≈Ç‡ãﬂÇ¢ìGÇíçéã
    void UpdateLockOnTarget()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, searchRadius, enemyLayer);
        List<Transform> enemies = hits.Select(h => h.transform).ToList();
        if (enemies.Count == 0) { isLockOn = false; currentTarget = null; return; }

        if (currentTarget == null || !enemies.Contains(currentTarget))
            currentTarget = enemies.OrderBy(e => Vector3.Distance(player.position, e.position)).First();
    }

    void MoveCamera()
    {
        Vector3 desiredPos = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, moveSmoothTime);
    }

    void LookAtTarget()
    {
        Vector3 targetPos = currentTarget != null ? currentTarget.position : player.position;
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }
}


