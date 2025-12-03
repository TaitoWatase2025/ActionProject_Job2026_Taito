using UnityEngine;

public class GameBGMStart : MonoBehaviour
{
    public AudioClip gameBGM;
    void Start()
    {
        GameAudioManager.Instance.PlayBGM(gameBGM, 1f); // 1秒フェードイン
    }
}
