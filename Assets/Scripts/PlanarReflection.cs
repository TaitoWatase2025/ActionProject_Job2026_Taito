using UnityEngine;

[ExecuteAlways]
public class PlanarReflection : MonoBehaviour
{
    public Camera reflectionCamera;
    public RenderTexture reflectionTexture;

    void LateUpdate()
    {
        if (reflectionCamera && reflectionTexture)
        {
            reflectionCamera.Render();
        }
    }
}
