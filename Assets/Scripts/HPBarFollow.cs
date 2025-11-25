using UnityEngine;

public class HPBarFollow : MonoBehaviour
{
    public Transform target;                 // 敵の頭位置
    public Vector3 offset = new Vector3(0, 2.0f, 0); // 頭上の高さ

    void LateUpdate()
    {
        if (target == null) return;

        // Screen Space - Overlay 用: Camera.main.WorldToScreenPoint をそのまま使用
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);
        transform.position = screenPos;
    }
}


