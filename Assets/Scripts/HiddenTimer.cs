using System;
using UnityEngine;
using UnityEngine.Events;

public class HiddenTimer : MonoBehaviour
{
    [Header("設定")]
    public int targetSeconds = 900;
    public bool autoStartOnPlayerInput = true;

    [Header("イベント（15分到達時）")]
    public UnityEvent onHiddenAreaSpawn;

    private bool isSpawned = false;
    private float elapsed = 0f;
    private void Start()
    {
        StartTimer();
    }
    void Update()
    {
        if (isSpawned) return;
        elapsed += Time.unscaledDeltaTime;
        Debug.Log("[HiddenTimer] 経過時間: " + elapsed + "秒");
        if (elapsed >= targetSeconds)
        {
            isSpawned = true;
            onHiddenAreaSpawn?.Invoke();

        }
    }

    public void StartTimer()
    {
        if (isSpawned) return;
        elapsed = 0;
        Debug.Log("[HiddenTimer] カウント開始");

    }
}
