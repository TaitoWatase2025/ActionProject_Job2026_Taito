using UnityEngine;

public class DynamicTitleCamera : MonoBehaviour
{
    public Transform target;

    public float rotateSpeed = 5f;

    // 上下
    public float floatAmplitudeY = 1.5f;
    public float floatSpeedY = 0.5f;
    public float minY = 2f;
    public float maxY = 5f;

    // 前後
    public float floatAmplitudeZ = 3f;
    public float floatSpeedZ = 0.3f;
    public float minZ = -5f;
    public float maxZ = 5f;

    // 左右（カメラX軸移動）
    public float floatAmplitudeX = 2f;
    public float floatSpeedX = 0.4f;
    public float minX = -3f;
    public float maxX = 3f;

    private Vector3 startPos;
    private float phaseY;
    private float phaseZ;
    private float phaseX;

    void Start()
    {
        startPos = transform.position;

        // フェーズをランダム化して複雑な動きに
        phaseY = Random.Range(0f, Mathf.PI * 2f);
        phaseZ = Random.Range(0f, Mathf.PI * 2f);
        phaseX = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // 水面中心を回転
        transform.RotateAround(target.position, Vector3.up, rotateSpeed * Time.deltaTime);

        // 上下
        float yOffset = Mathf.Sin(Time.time * floatSpeedY + phaseY) * floatAmplitudeY;
        float clampedY = Mathf.Clamp(startPos.y + yOffset, minY, maxY);

        // 前後
        float zOffset = Mathf.Sin(Time.time * floatSpeedZ + phaseZ) * floatAmplitudeZ;
        float clampedZ = Mathf.Clamp(startPos.z + zOffset, minZ, maxZ);

        // 左右
        float xOffset = Mathf.Sin(Time.time * floatSpeedX + phaseX) * floatAmplitudeX;
        float clampedX = Mathf.Clamp(startPos.x + xOffset, minX, maxX);

        transform.position = new Vector3(clampedX, clampedY, clampedZ);
        transform.LookAt(target.position);
    }
}











