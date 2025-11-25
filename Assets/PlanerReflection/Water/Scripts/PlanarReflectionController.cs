using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlanarReflectionController : MonoBehaviour
{
    [Header("Targets")]
    public Camera mainCamera;
    public Camera reflectionCamera;
    public RenderTexture reflectionTexture;

    [Header("Water Plane")]
    public Vector3 planeNormal = Vector3.up;
    public float planeHeight = 0f; // 水面のY座標

    Material _material;
    static readonly int PlanarTexId = Shader.PropertyToID("_PlanarReflectionTex");

    void Awake()
    {
        _material = GetComponent<Renderer>().sharedMaterial;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null || reflectionCamera == null || reflectionTexture == null)
            return;

        // 1. 水面の平面（法線と一点）を定義
        var n = planeNormal.normalized;
        var planePoint = new Vector3(0f, planeHeight, 0f);

        // 2. メインカメラの位置と向きを反転
        Vector3 camPos = mainCamera.transform.position;
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camUp = mainCamera.transform.up;

        // 平面までの距離
        float d = Vector3.Dot(n, camPos - planePoint);
        Vector3 reflPos = camPos - 2f * d * n;
        Vector3 reflForward = Vector3.Reflect(camForward, n);
        Vector3 reflUp = Vector3.Reflect(camUp, n);

        reflectionCamera.transform.position = reflPos;
        reflectionCamera.transform.rotation = Quaternion.LookRotation(reflForward, reflUp);

        // 3. クリッププレーンを調整（簡易版：省略しても動くが、水面上の映り込みが安定しやすい）
        Vector4 plane = new Vector4(n.x, n.y, n.z, -Vector3.Dot(n, planePoint));
        var reflectionMat = CalculateReflectionMatrix(plane);
        reflectionCamera.worldToCameraMatrix = mainCamera.worldToCameraMatrix * reflectionMat;

        // 投影行列はメインカメラと揃える
        reflectionCamera.projectionMatrix = mainCamera.projectionMatrix;

        // 4. 描画
        reflectionCamera.targetTexture = reflectionTexture;
        reflectionCamera.enabled = false;  // 明示的に
        reflectionCamera.Render();

        // 5. マテリアルにテクスチャを渡す
        _material.SetTexture(PlanarTexId, reflectionTexture);
    }

    // 平面に対する反射行列（Unity標準の実装と同等の簡易版）
    Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
    {
        Matrix4x4 m = Matrix4x4.identity;

        m.m00 = 1f - 2f * plane[0] * plane[0];
        m.m01 = -2f * plane[0] * plane[1];
        m.m02 = -2f * plane[0] * plane[2];
        m.m03 = -2f * plane[3] * plane[0];

        m.m10 = -2f * plane[1] * plane[0];
        m.m11 = 1f - 2f * plane[1] * plane[1];
        m.m12 = -2f * plane[1] * plane[2];
        m.m13 = -2f * plane[3] * plane[1];

        m.m20 = -2f * plane[2] * plane[0];
        m.m21 = -2f * plane[2] * plane[1];
        m.m22 = 1f - 2f * plane[2] * plane[2];
        m.m23 = -2f * plane[3] * plane[2];

        m.m30 = 0f;
        m.m31 = 0f;
        m.m32 = 0f;
        m.m33 = 1f;
        return m;
    }
}