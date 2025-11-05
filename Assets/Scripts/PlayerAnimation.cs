using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator anim;               // Animator
    public Transform playerTransform;   // Player本体
    public CharacterController controller; // CharacterController（速度取得用）

    private Vector3 previousPosition;

    private void Start()
    {
        previousPosition = playerTransform.position;
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        // XZ 平面の速度計算
        Vector3 delta = playerTransform.position - previousPosition;
        delta.y = 0;
        float speed = delta.magnitude / Time.deltaTime;

        // Animator に反映
        anim.SetFloat("Speed", speed);
        anim.SetBool("IsGrounded", controller.isGrounded);
        anim.SetFloat("VerticalVelocity", controller.velocity.y);

        previousPosition = playerTransform.position;

        


    }
}

