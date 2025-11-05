using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float coyoteTime = 0.1f;     // 地面離れてもジャンプできる猶予
    public float jumpBufferTime = 0.1f; // ジャンプボタン押し猶予
    [Header("回転速度")]
    public float rotationSpeed = 10f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private Animation animations;

    public Transform cameraTransform;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    

    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animations = GetComponent<Animation>();

    }

    private void OnEnable()
    {
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
        playerInput.actions["Jump"].performed += OnJump;
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
        playerInput.actions["Jump"].performed -= OnJump;
    }

    private void Update()
    {
        Move();
        //地面に接地しているかどうかのデバッグ表示
        Debug.Log("Is Grounded: " + controller.isGrounded);
    }

    private void Move()
    {
        if(cameraTransform == null) return;
        // カメラの向きを基準に移動方向を計算
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        //水平方向の成分のみを使用
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move= forward * moveInput.y + right * moveInput.x;

        // 移動方向があるときにキャラクターを回転
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        // --- 接地猶予（Coyote Time） ---
        if (controller.isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // --- ジャンプ入力猶予（Jump Buffer） ---
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            verticalVelocity = jumpForce;
            jumpBufferCounter = 0; // ジャンプ消化
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }


        // --- 重力 ---
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        // ダッシュ処理
        bool sprinting = playerInput.actions["Sprint"].IsPressed();
        float speed = sprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        // 移動
        controller.Move(move * speed * Time.deltaTime);
    }


    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }
    
}
