using UnityEngine;

public class FloatingShard : MonoBehaviour
{
    [Header("“®‚«‚Ì‘å‚«‚³")]
    public float moveAmplitude = 0.5f;
    public float moveSpeed = 1.0f;

    [Header("‰ñ“]‘¬“x")]
    public Vector3 rotationSpeed = new Vector3(10f, 20f, 5f);

    private Vector3 startPos;
    private float offset;

    void Start()
    {
        startPos = transform.position;
        offset = Random.Range(0f, 999f);

        // Še”j•Ð‚²‚Æ‚É“®‚«•û‚ðƒ‰ƒ“ƒ_ƒ€‰»
        moveAmplitude *= Random.Range(0.7f, 1.4f);
        moveSpeed *= Random.Range(0.7f, 1.3f);
        rotationSpeed *= Random.Range(0.7f, 1.3f);
    }

    void Update()
    {
        // ‚Ó‚í‚Ó‚íˆÚ“®
        float t = Time.time * moveSpeed + offset;

        Vector3 floatOffset = new Vector3(
            Mathf.Sin(t * 0.8f) * moveAmplitude,
            Mathf.Sin(t * 1.1f) * moveAmplitude * 0.8f,
            Mathf.Sin(t * 1.3f) * moveAmplitude
        );

        transform.position = startPos + floatOffset;

        // ‚ä‚Á‚­‚è‰ñ“]
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}

