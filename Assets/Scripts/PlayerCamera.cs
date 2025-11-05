using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;      // 追従対象（Player）
    public Vector3 offset = new Vector3(0, 1.5f, -5f); // カメラの後方・上方オフセット
    public float lookSpeed = 2f;
    public float minPitch = -60f;
    public float maxPitch = 60f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private float yaw;   // 左右回転
    private float pitch; // 上下回転
    private float zoom;

    private Vector2 lookInput;
    private void Start()
    {
        if (target == null) return;

        // 初期カメラ位置を背後に設定
        yaw = 180f;          // Player の背後
        pitch = 10f;         // 少し見下ろす角度
        zoom = -offset.z;    // offset.z に合わせてズーム初期化
    }

    private void Update()
    {
        HandleInput();
        RotateAndFollow();
    }

    private void HandleInput()
    {
        // マウスまたはスティックの入力を取得
        lookInput.x = Mouse.current.delta.x.ReadValue();
        lookInput.y = Mouse.current.delta.y.ReadValue();
        // ズーム入力（マウスホイール）
        float scrollInput = Mouse.current.scroll.y.ReadValue();
        zoom -= scrollInput * zoomSpeed * Time.deltaTime;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        offset.z = -zoom;

        yaw += lookInput.x * lookSpeed;
        pitch -= lookInput.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void RotateAndFollow()
    {
        if (target == null) return;

        // カメラの回転を作る
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // カメラの位置 = Player位置 + 回転後のオフセット
        transform.position = target.position + rotation * offset;

        // カメラの向き = Playerを見る
        transform.LookAt(target.position + Vector3.up * 1.5f); // キャラクターの頭方向を見る

    }
}


