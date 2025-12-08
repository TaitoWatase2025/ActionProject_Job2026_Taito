using System.Collections;
using UnityEngine;

public class BreakStage : MonoBehaviour
{
    public void OnBreakStage()
    {
        gameObject.SetActive(false);
        GameAudioManager.Instance.StageBreakSE(transform.position);
    }
}
