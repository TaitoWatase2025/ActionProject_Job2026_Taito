using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Attacking
    }
    public PlayerState state;

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
    public Animator anim;
    public Transform playerTransform;
    public Transform cameraTransform;

    private float verticalVelocity;
    private Vector3 previousPosition;

    // PlayerControls を追加
    private PlayerControls controls;

    //コンボ攻撃用の変数
    private int comboStep = 0;
    private float comboTimer = 0f;
    private float comboMaxDelay = 1f; // コンボの最大遅延時間

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerTransform == null) playerTransform = transform;

        controls = new PlayerControls();

        // 入力イベント登録
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();// 移動入力取得
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;// 移動入力リセット
        controls.Player.Jump.performed += ctx => jumpBufferCounter = jumpBufferTime;// ジャンプバッファ設定
        controls.Player.Sprint.performed += ctx => { }; // SprintはIsPressedで取得
        controls.Player.Attack.started += ctx => HandleAttackInput();// 攻撃入力処理
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Start() => previousPosition = playerTransform.position;

    private void Update()
    {
        HandlecomboTimer();
        Move();
        UpdateAnimator();
        Debug.Log("Is Grounded: " + controller.isGrounded);
    }

    private void Move()
    {
        if (state == PlayerState.Attacking)
        {
            // 攻撃中は移動しない
            return;
        }
        if (cameraTransform == null) return;
        // カメラの向きに基づく移動方向計算
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;// 移動方向計算

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 接地猶予
        coyoteTimeCounter = controller.isGrounded ? coyoteTime : coyoteTimeCounter - Time.deltaTime;

        // ジャンプバッファ
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            anim.SetTrigger("Jump");
            verticalVelocity = jumpForce;
            jumpBufferCounter = 0;
        }
        else
            jumpBufferCounter -= Time.deltaTime;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        // ダッシュ処理
        float speed = moveSpeed;
        if (controls.Player.Sprint.IsPressed()) speed *= sprintMultiplier;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void HandleAttackInput()
    {
        if (state != PlayerState.Attacking)
        {
            state = PlayerState.Attacking;
            comboStep++;
            comboTimer = comboMaxDelay; // コンボタイマーリセット
            anim.SetInteger("ComboStep", comboStep);
            anim.SetTrigger("Attack");
        }
        else if (state == PlayerState.Attacking && comboTimer > 0f)
        {
            // コンボ攻撃の入力受付
            comboStep++;
            comboTimer = comboMaxDelay; // コンボタイマーリセット
        }
    }
    private void HandlecomboTimer()
    {
        if (comboTimer > 0f) comboTimer -= Time.deltaTime;
        else comboStep = 0; // タイムアウトでコンボリセット
    }

    private void OnAttackEnd()
    {
        state = PlayerState.Idle;
    }

    private void UpdateAnimator()
    {

        Vector3 delta = playerTransform.position - previousPosition;
        delta.y = 0;
        float speed = delta.magnitude / Time.deltaTime;

        anim.SetFloat("Speed", speed);
        anim.SetFloat("ComboStep", comboStep);
        previousPosition = playerTransform.position;
    }
}
