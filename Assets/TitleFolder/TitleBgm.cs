using UnityEngine;

public class TitleBGM : MonoBehaviour
{
    public AudioClip titleBGM;

    private void Start()
    {
        TitleAudioManager.Instance.PlayBGM(titleBGM, 1f); // 1秒フェードイン
    }
}
