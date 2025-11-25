using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("スポーン範囲")]
    public Vector3 center;
    public Vector3 size;

    [Header("Prefab設定")]
    public GameObject enemyPrefab;
    public GameObject groundPortalPrefab;
    public GameObject airPortalPrefab;
    public GameObject enemyHPUIPrefab;  // Canvas用HPバーPrefab
    public Transform canvasTransform;    // CanvasのTransform

    [Header("スポーン設定")]
    public float airHeight = 3f;
    public float portalLifeTime = 2f;
    public float spawnDelay = 1f;

    private int spawnCount = 1;
    private int aliveEnemies = 0;
    private bool firstSpawnDone = false;
    public PlayerController player;

    void Update()
    {
        if (!firstSpawnDone && PlayerHasMoved())
        {
            firstSpawnDone = true;
            StartCoroutine(SpawnWave());
        }
    }

    bool PlayerHasMoved()
    {
        return player.moveSpeed > 0.1f;
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < spawnCount; i++)
            SpawnEnemy();

        while (aliveEnemies > 0)
            yield return null;

        spawnCount++;
        yield return new WaitForSeconds(spawnDelay);
        StartCoroutine(SpawnWave());
    }

    void SpawnEnemy()
    {
        Vector3 pos = GetRandomPosition();
        bool spawnInAir = Random.value > 0.5f;
        if (spawnInAir)
            pos.y += airHeight;

        GameObject portal = Instantiate(spawnInAir ? airPortalPrefab : groundPortalPrefab, pos, Quaternion.identity);
        Destroy(portal, portalLifeTime);

        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        aliveEnemies++;

        // HPバー生成
        if (enemyHPUIPrefab != null && canvasTransform != null)
        {
            GameObject hpBar = Instantiate(enemyHPUIPrefab, canvasTransform);

            // HPバー追従用
            HPBarFollow follower = hpBar.GetComponent<HPBarFollow>();
            follower.target = enemy.transform.Find("HeadPoint");

            // HP同期
            EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
            hpBar.GetComponent<EnemyHPManager>().Initialize(enemyStatus);
        }

        // 敵が死んだらカウント減
        EnemyDeathNotifier notifier = enemy.AddComponent<EnemyDeathNotifier>();
        notifier.onDeath = () => { aliveEnemies--; };
    }

    Vector3 GetRandomPosition()
    {
        return center + new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2),
            Random.Range(-size.z / 2, size.z / 2)
        );
    }
}

// 敵死亡通知
public class EnemyDeathNotifier : MonoBehaviour
{
    public System.Action onDeath;
    void OnDestroy() => onDeath?.Invoke();
}


