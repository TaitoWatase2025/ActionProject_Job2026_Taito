using UnityEngine;
using UnityEngine.InputSystem;

public class CameraContoroller : MonoBehaviour
{
    public Transform target;      // 追従対象（Player）
    public Vector3 offset = new Vector3(0, 1.5f, -5f); // カメラの後方・上方オフセット
    public float lookSpeed = 2f;
    public float minPitch = -60f;
    public float maxPitch = 60f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    // ロックオン用
    public Transform lockOnTarget;   // ロックオン対象の敵
    public bool isLockOn = false;    // ロックオン中フラグ
    public float lockOnRotationSpeed = 5f;

    private float yaw;   // 左右回転
    private float pitch; // 上下回転
    private float zoom;

    private Vector2 lookInput;

    private void Start()
    {
        if (target == null) return;

        yaw = 180f;          // Player の背後
        pitch = 10f;         // 少し見下ろす角度
        zoom = Mathf.Abs(offset.z);
    }

    private void LateUpdate()
    {
        if (!isLockOn)
        {
            HandleInput();
        }
        RotateAndFollow();
    }

    private void HandleInput()
    {
        lookInput.x = Mouse.current.delta.x.ReadValue();
        lookInput.y = Mouse.current.delta.y.ReadValue();

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

        Quaternion rotation;

        if (isLockOn && lockOnTarget != null)
        {
            // ロックオン中は敵方向を向く
            Vector3 direction = lockOnTarget.position - transform.position;
            direction.y = 0; // 上下角度を固定したい場合
            if (direction.sqrMagnitude > 0.01f)
            {
                rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), lockOnRotationSpeed * Time.deltaTime);
            }
            else
            {
                rotation = transform.rotation;
            }
        }
        else
        {
            // 通常カメラ回転
            rotation = Quaternion.Euler(pitch, yaw, 0);
        }

        // カメラ位置を設定
        transform.position = target.position + rotation * offset;

        // カメラの向き = Playerを見る（ロックオン中でも少し上を見る）
        Vector3 lookAt = target.position + Vector3.up * 1.5f;
        transform.LookAt(lookAt);
    }

    // ロックオン切替メソッド（例）
    public void ToggleLockOn(Transform enemy)
    {
        if (enemy == null)
        {
            isLockOn = false;
            lockOnTarget = null;
        }
        else
        {
            isLockOn = true;
            lockOnTarget = enemy;
        }
    }
}
