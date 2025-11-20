// MirrorCameraController.cs
using UnityEngine;

public class MirrorCameraController : MonoBehaviour
{
    public Transform mirrorPlane;
    public Transform mainCamera;

    void LateUpdate()
    {
        Vector3 camPos = mainCamera.position;
        Vector3 mirrorNormal = mirrorPlane.forward;
        Vector3 mirrorPos = mirrorPlane.position;

        // ‹¾–Ê‚É‘Î‚µ‚ÄƒJƒƒ‰ˆÊ’u‚ğ”½“]
        Vector3 reflectedPos = camPos - 2 * Vector3.Dot(camPos - mirrorPos, mirrorNormal) * mirrorNormal;
        transform.position = reflectedPos;

        // ‹ü‚ğ”½Ë
        Vector3 lookDir = mainCamera.forward;
        Vector3 reflectedDir = lookDir - 2 * Vector3.Dot(lookDir, mirrorNormal) * mirrorNormal;
        transform.rotation = Quaternion.LookRotation(reflectedDir, Vector3.up);
    }
}

