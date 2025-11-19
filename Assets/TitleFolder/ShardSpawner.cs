using UnityEngine;
using System.Collections.Generic;

public class ShardSpawner : MonoBehaviour
{
    public GameObject shardPrefab;
    public int shardCount = 50;
    public Vector3 spawnArea = new Vector3(10, 3, 10); // 配置範囲
    public float minDistance = 0.7f; // 破片同士の最小距離

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        for (int i = 0; i < shardCount; i++)
        {
            Vector3 pos = GetRandomPosition();
            GameObject shard = Instantiate(shardPrefab, pos, Random.rotation, transform);

            // スケールのランダム化
            float scale = Random.Range(0.01f, 0.1f);
            shard.transform.localScale = Vector3.one * scale;
        }
    }

    Vector3 GetRandomPosition()
    {
        int attempts = 0;
        while (attempts < 100)
        {
            float x = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
            float y = Random.Range(0, spawnArea.y);
            float z = Random.Range(-spawnArea.z / 2, spawnArea.z / 2);
            Vector3 candidate = new Vector3(x, y, z);

            bool valid = true;
            foreach (Vector3 pos in spawnedPositions)
            {
                if (Vector3.Distance(pos, candidate) < minDistance)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                spawnedPositions.Add(candidate);
                return candidate;
            }

            attempts++;
        }

        // 最終的に見つからなければ適当な場所に置く
        return new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            Random.Range(0, spawnArea.y),
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );
    }
}

