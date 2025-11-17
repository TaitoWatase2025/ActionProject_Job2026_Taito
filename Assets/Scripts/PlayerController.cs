using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Walking, Running, Jumping, Attacking, Dodging }
    public PlayerState state = PlayerState.Idle;

    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("回避設定")]
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.5f;

    [Header("回転速度")]
    public float rotationSpeed = 10f;

    private CharacterController controller;
    private Vector2 moveInput;
    private float verticalVelocity;
    public Transform cameraTransform;
    public Transform playerTransform;
    public Animator anim;

    private PlayerControls controls;
    private Vector3 previousPosition;

    // コンボ攻撃用
    private int comboStep = 0;
    private int comboMax = 3;
    private bool comboInput = false;

    // 回避用
    private bool isDodging = false;
    private Vector3 dodgeDirection;
    private float dodgeTimer;

    // ジャンプ用
    private float coyoteTime = 0.1f;
    private float jumpBufferTime = 0.1f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerTransform == null) playerTransform = transform;// デフォルトで自分自身を設定

        controls = new PlayerControls();

        // 移動
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // ジャンプ
        controls.Player.Jump.performed += ctx => jumpBufferCounter = jumpBufferTime;

        // 攻撃
        controls.Player.Attack.started += ctx => HandleAttackInput();

        // 回避
        controls.Player.Dodge.started += ctx => HandleDodgeInput();

        // スプリント（IsPressedで判定）
        controls.Player.Sprint.performed += ctx => { };
    }


    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        if (GetComponent<PlayerStatus>().IsStunned()) return; // スタン中は操作不可
        HandleJump();      // 先にジャンプ判定
        HandleGravity();
        HandleDodge();
        HandleMove();
        anim.SetBool("IsGrounded", controller.isGrounded);// 接地情報をアニメーターに渡す
        anim.SetFloat("Speed", controller.velocity.magnitude);// 速度情報をアニメーターに渡す
        UpdateAnimator();
    }

    #region 移動
    private void HandleMove()
    {
        if (state == PlayerState.Attacking || isDodging || state == PlayerState.Dodging) return;
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;// カメラの向きから前方向を取得
        Vector3 right = cameraTransform.right;// カメラの向きから右方向を取得
        forward.y = right.y = 0;// 水平方向のみに制限
        forward.Normalize(); right.Normalize();// 正規化
        // 移動方向の計算
        Vector3 move = forward * moveInput.y + right * moveInput.x;

        if (move.sqrMagnitude > 0.01f)// 回転処理
        {
            Quaternion targetRot = Quaternion.LookRotation(move.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        // 移動処理
        float speed = moveSpeed;
        if (controls.Player.Sprint.IsPressed()) speed *= sprintMultiplier;

        controller.Move(move * speed * Time.deltaTime);

        // ジャンプ中や攻撃中は上書きしない
        if (state != PlayerState.Jumping && state != PlayerState.Dodging && state != PlayerState.Attacking)
            state = move.sqrMagnitude > 0.01f ? PlayerState.Walking : PlayerState.Idle;
    }
    #endregion

    #region ジャンプ
    private void HandleJump()
    {
        // 接地猶予
        if (controller.isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            coyoteTimeCounter = Mathf.Max(coyoteTimeCounter, 0f);
        }

        // ジャンプバッファ
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            verticalVelocity = jumpForce;
            jumpBufferCounter = 0;
            anim.SetTrigger("Jump");
            state = PlayerState.Jumping;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
            jumpBufferCounter = Mathf.Max(jumpBufferCounter, 0f);
        }
    }
    #endregion

    #region 攻撃
    private void HandleAttackInput()
    {
        if (state == PlayerState.Dodging) return;

        if (state != PlayerState.Attacking)
        {
            if (comboInput && comboStep < comboMax)
            {
                comboInput = true;
            }
        }

        state = PlayerState.Attacking;
        comboStep = 1;
        comboInput = false;
        anim.SetInteger("ComboStep", comboStep);
        anim.SetTrigger("Attack");

    }
    public void Combowindowstart()
    {
        // コンボウィンドウ開始（アニメーションイベントから呼び出し）
        comboInput = true;
    }
    public void Combowindowend()
    {
        // コンボウィンドウ終了（アニメーションイベントから呼び出し）
        comboInput = false;
    }

    public void OnAttackEnd()
    {

        if (comboInput && comboStep < comboMax)
        {
            comboStep++;
            anim.SetInteger("ComboStep", comboStep);
            anim.SetTrigger("Attack");
            comboInput = false;
        }
        else
        {
            comboStep = 0;
            state = PlayerState.Idle;
            anim.SetInteger("ComboStep", comboStep);
        }
    }
    #endregion

    #region 回避
    private void HandleDodgeInput()
    {
        if (state == PlayerState.Attacking || isDodging||GetComponent<PlayerStatus>().IsStunned()) return;

        state = PlayerState.Dodging;
        isDodging = true;
        dodgeTimer = dodgeDuration;

        Vector3 dir = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        dir.y = 0;
        dodgeDirection = dir.sqrMagnitude > 0.01f ? dir.normalized : playerTransform.forward;

        anim.SetTrigger("Dodge");
    }

    private void HandleDodge()
    {
        if (!isDodging) return;

        controller.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
        dodgeTimer -= Time.deltaTime;

        if (dodgeTimer <= 0f)
        {
            isDodging = false;
            state = PlayerState.Idle;
        }
    }
    #endregion

    #region 重力
    private void HandleGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
    #endregion

    #region 被弾時
    public void OnHit()
    {
        // 被弾時の処理（アニメーションイベントから呼び出し）
        // アニメーション時間操作できない

        state = PlayerState.Idle;
        comboStep = 0;
        anim.SetInteger("ComboStep", comboStep);
        anim.SetTrigger("AttackHit");
        GetComponent<PlayerStatus>().StunTime = 1.5f; // スタン時間を設定
    }
    #endregion

    private void UpdateAnimator()
    {
        Vector3 delta = playerTransform.position - previousPosition;
        delta.y = 0;
        float speed = delta.magnitude / Time.deltaTime;
        anim.SetFloat("Speed", speed);
        anim.SetInteger("ComboStep", comboStep);
        previousPosition = playerTransform.position;
    }
}