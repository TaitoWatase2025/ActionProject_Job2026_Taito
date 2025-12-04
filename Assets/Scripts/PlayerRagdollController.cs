using System.Xml.Serialization;
using UnityEngine;

public class PlayerRagdollController : MonoBehaviour
{
    [Header("人形化")]
    public Rigidbody[] ragdollRigidbodies;
    public Collider[] ragdollColliders;
    public Collider mainCollider;

    public Animator animator;
    private bool isRagdoll = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetRagdollActive(false);
    }
    void Update()
    {
        if (!isRagdoll) return;
    }

    void SetRagdollActive(bool active)
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
        }
        foreach (var col in ragdollColliders)
        {
            col.enabled = active;
        }
    }

    //アニメーションイベントから呼び出し
    public void OnEnableRagdoll()
    {
        isRagdoll = true;
        ExecuteRagdoll();
    }

    void ExecuteRagdoll()
    {
        animator.enabled = false;
        mainCollider.enabled = false;
        SetRagdollActive(true);
    }
}
