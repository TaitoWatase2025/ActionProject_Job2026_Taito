using UnityEngine;

public class DynamicTitleCamera : MonoBehaviour
{
    public Transform target;

    [Header("‰ñ“]")]
    public float rotateSpeed = 5f;

    [Header("ã‰ºˆÚ“®")]
    public float floatAmplitudeY = 1.5f;
    public float floatSpeedY = 0.5f;
    public float minY = 2f;
    public float maxY = 5f;

    [Header("‘OŒãˆÚ“®")]
    public float floatAmplitudeZ = 3f;
    public float floatSpeedZ = 0.3f;
    public float minZ = -5f;
    public float maxZ = 5f;

    [Header("¶‰EˆÚ“®")]
    public float floatAmplitudeX = 2f;
    public float floatSpeedX = 0.4f;
    public float minX = -3f;
    public float maxX = 3f;

    [Header("‹ü—h‚ê")]
    public float lookAmplitudeX = 0.5f;
    public float lookAmplitudeY = 0.3f;
    public float lookAmplitudeZ = 0.5f;
    public float lookSpeedX = 0.3f;
    public float lookSpeedY = 0.5f;
    public float lookSpeedZ = 0.4f;

    private Vector3 startPos;
    private float phaseY, phaseZ, phaseX;

    void Start()
    {
        startPos = transform.position;

        // ƒtƒF[ƒY‚ğƒ‰ƒ“ƒ_ƒ€‰»‚µ‚Ä—h‚ê‚ğƒoƒ‰ƒoƒ‰‚É
        phaseY = Random.Range(0f, Mathf.PI * 2f);
        phaseZ = Random.Range(0f, Mathf.PI * 2f);
        phaseX = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // …–Ê’†S‚ğ‰ñ“]
        transform.RotateAround(target.position, Vector3.up, rotateSpeed * Time.deltaTime);

        // --- ˆÊ’u—h‚ê ---
        float yOffset = Mathf.Sin(Time.time * floatSpeedY + phaseY) * floatAmplitudeY;
        float clampedY = Mathf.Clamp(startPos.y + yOffset, minY, maxY);

        float zOffset = Mathf.Sin(Time.time * floatSpeedZ + phaseZ) * floatAmplitudeZ;
        float clampedZ = Mathf.Clamp(startPos.z + zOffset, minZ, maxZ);

        float xOffset = Mathf.Sin(Time.time * floatSpeedX + phaseX) * floatAmplitudeX;
        float clampedX = Mathf.Clamp(startPos.x + xOffset, minX, maxX);

        transform.position = new Vector3(clampedX, clampedY, clampedZ);

        // --- ‹ü—h‚ê ---
        Vector3 lookTarget = target.position;
        lookTarget += new Vector3(
            Mathf.Sin(Time.time * lookSpeedX + phaseX) * lookAmplitudeX,
            Mathf.Sin(Time.time * lookSpeedY + phaseY) * lookAmplitudeY,
            Mathf.Sin(Time.time * lookSpeedZ + phaseZ) * lookAmplitudeZ
        );

        // ŠŠ‚ç‚©‚É‰ñ“]
        Vector3 direction = (lookTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
    }
}












