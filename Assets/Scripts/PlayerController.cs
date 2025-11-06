using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Walking, Running, Jumping, Attacking }
    public PlayerState state = PlayerState.Idle;

    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    [Header("回転速度")]
    public float rotationSpeed = 10f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private CharacterController controller;
    private Vector2 moveInput;
    public Transform playerTransform;
    public Transform cameraTransform;
    public Animator anim;

    private float verticalVelocity;
    private Vector3 previousPosition;

    // Input System
    private PlayerControls controls;

    // コンボ攻撃用
    private int comboStep = 0;
    private int comboMax = 3;
    private bool comboInput = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerTransform == null) playerTransform = transform;

        controls = new PlayerControls();

        // 移動入力
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // ジャンプ入力
        controls.Player.Jump.performed += ctx => jumpBufferCounter = jumpBufferTime;

        // 攻撃入力
        controls.Player.Attack.started += ctx => HandleAttackInput();

        // スプリントはIsPressedで取得
        controls.Player.Sprint.performed += ctx => { };
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Start() => previousPosition = playerTransform.position;

    private void Update()
    {
        Move();
        UpdateAnimator();
    }

    private void Move()
    {
        if (state == PlayerState.Attacking) return; // 攻撃中は移動不可
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = right.y = 0;
        forward.Normalize(); right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 接地猶予
        coyoteTimeCounter = controller.isGrounded ? coyoteTime : coyoteTimeCounter - Time.deltaTime;

        // ジャンプバッファ
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            verticalVelocity = jumpForce;
            jumpBufferCounter = 0;
            anim.SetTrigger("Jump");
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        float speed = moveSpeed;
        if (controls.Player.Sprint.IsPressed()) speed *= sprintMultiplier;

        controller.Move(move * speed * Time.deltaTime);

        // 状態設定
        state = move.sqrMagnitude > 0.01f ? PlayerState.Walking : PlayerState.Idle;
    }

    private void HandleAttackInput()
    {
        if (state != PlayerState.Attacking)
        {
            state = PlayerState.Attacking;
            comboStep = 1;
            comboInput = false;
            anim.SetInteger("ComboStep", comboStep);
            anim.SetTrigger("Attack");
        }
        else
        {
            comboInput = true; // 攻撃中の入力を記録
        }
    }

    // アニメーションイベントから呼ぶ
    public void OnAttackEnd()
    {
        if (comboInput && comboStep < comboMax)
        {
            comboStep++;
            comboInput = false;
            anim.SetInteger("ComboStep", comboStep);
            anim.SetTrigger("Attack"); // 次のコンボ攻撃
        }
        else
        {
            comboStep = 0;
            state = PlayerState.Idle;
        }
    }

    private void UpdateAnimator()
    {
        Vector3 delta = playerTransform.position - previousPosition;
        delta.y = 0;
        float speed = (state == PlayerState.Attacking) ? 0f : delta.magnitude / Time.deltaTime;

        anim.SetFloat("Speed", speed);
        anim.SetInteger("ComboStep", comboStep);

        previousPosition = playerTransform.position;
    }
}

