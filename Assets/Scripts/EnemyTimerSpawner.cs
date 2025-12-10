using UnityEngine;
using System.Collections;

public class EnemyTimerSpawner : MonoBehaviour
{
    [Header("スポーン範囲")]
    public Vector3 center;
    public Vector3 size;

    [Header("Prefab設定")]
    public GameObject enemyPrefab;
    public GameObject enemyHPUIPrefab;  // World Space Canvas HPバー Prefab

    [Header("スポーン設定")]
    public float airHeight = 3f;
    public float spawnDelay = 1f;

    [Header("時間スポーン設定")]
    public float timeInterval = 60f; // 1分 = 60秒
    private Coroutine spawnCoroutine;


    void OnEnable()
    {
        spawnCoroutine = StartCoroutine(TimeSpawnLoop());
    }
    void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }
    IEnumerator TimeSpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeInterval);
            SpawnEnemy();
        }
    }
    void SpawnEnemy()
    {
        // 空中生成
        Vector3 pos = GetRandomPosition();
        pos.y += airHeight;

        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        // HPバー生成（World Space Canvas）- Enemyの子として生成
        if (enemyHPUIPrefab != null)
        {
            GameObject hpBar = Instantiate(enemyHPUIPrefab, enemy.transform);
            hpBar.transform.localPosition = Vector3.zero;
            hpBar.transform.localScale = Vector3.one;
            hpBar.SetActive(true);

            HPBarFollow follower = hpBar.GetComponent<HPBarFollow>();
            Transform head = enemy.transform.Find("HeadPoint");
            if (head != null)
                follower.target = head;

            // カメラ設定
            if (follower.cam == null)
                follower.cam = Camera.main;

            EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
            if (enemyStatus != null)
            {
                // 初期HP反映
                follower.SetHP(enemyStatus.health, enemyStatus.maxHealth);

                // HP変化イベント登録
                enemyStatus.OnHealthChanged += (value) =>
                {
                    follower.SetHP(value, enemyStatus.maxHealth);
                };
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        return center + new Vector3(
            Random.Range(-size.x / 2f, size.x / 2f),
            Random.Range(-size.y / 2f, size.y / 2f),
            Random.Range(-size.z / 2f, size.z / 2f)
        );
    }
}

// 敵死亡通知用
//public class EnemyDeathNotifier : MonoBehaviour
//{
//    public System.Action onDeath;
//    void OnDestroy() => onDeath?.Invoke();
//}



