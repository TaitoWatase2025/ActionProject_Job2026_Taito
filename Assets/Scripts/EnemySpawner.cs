using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("スポーン範囲")]
    public Vector3 center;           // スポーン範囲の中心
    public Vector3 size;             // 範囲の大きさ

    [Header("Prefab設定")]
    public GameObject enemyPrefab;
    public GameObject groundPortalPrefab;
    public GameObject airPortalPrefab;

    [Header("スポーン設定")]
    public float airHeight = 3f;      // 空中スポーンの高さ
    public float portalLifeTime = 2f; // ポータルが消えるまでの時間
    public float spawnDelay = 1f;     // 次のウェーブまでの時間

    private int spawnCount = 1;       // 最初は 1体
    private int aliveEnemies = 0;
    private bool firstSpawnDone = false;
    public PlayerController player; // プレイヤー参照

    void Update()
    {
        // 最初の1体は「プレイヤーが動いた瞬間」に出す
        if (!firstSpawnDone && PlayerHasMoved())
        {
            firstSpawnDone = true;
            StartCoroutine(SpawnWave());
        }
    }

    bool PlayerHasMoved()
    {
        // プレイヤーが 0.1 以上動いたら「ゲーム開始」
        return player.moveSpeed > 0.1f;
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < spawnCount; i++)
            SpawnEnemy();

        // ウェーブ完了を待つ
        while (aliveEnemies > 0)
            yield return null;

        // 次のウェーブに進む
        spawnCount++; // 敵の数を増やす（1→2→3→4…）
        yield return new WaitForSeconds(spawnDelay);

        StartCoroutine(SpawnWave());
    }

    void SpawnEnemy()
    {
        Vector3 pos = GetRandomPosition();
        bool spawnInAir = Random.value > 0.5f;

        if (spawnInAir)
            pos.y += airHeight;

        // ポータル生成（地面用と空中用を分ける）
        GameObject portal = Instantiate(
            spawnInAir ? airPortalPrefab : groundPortalPrefab,
            pos,
            Quaternion.identity
        );

        Destroy(portal, portalLifeTime);

        // 敵を出現
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        aliveEnemies++;

        // 敵が死んだらカウントを減らす処理
        EnemyDeathNotifier notifier = enemy.AddComponent<EnemyDeathNotifier>();
        notifier.onDeath = () => { aliveEnemies--; };
    }

    // 範囲内のランダムポイントを返す
    Vector3 GetRandomPosition()
    {
        return center + new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2),
            Random.Range(-size.z / 2, size.z / 2)
        );
    }

    // Gizmosで範囲を可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, size);
    }
}

// 敵が死んだことをスポナーへ通知する仕組み
public class EnemyDeathNotifier : MonoBehaviour
{
    public System.Action onDeath;

    void OnDestroy()
    {
        onDeath?.Invoke();
    }
}


