using UnityEngine;

public class UIAudioController : MonoBehaviour
{

    [Header("汎用AudioSource")]
    [SerializeField] private AudioSource audioSource;

    [Header("オーディオクリップ")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;


    public void PlayClickSE()
    {
        if (audioSource != null && clickClip != null)
        {
            audioSource.PlayOneShot(clickClip);
        }
    }

    public void PlayHoverSE()
    {
        if (audioSource != null && hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
    }
}