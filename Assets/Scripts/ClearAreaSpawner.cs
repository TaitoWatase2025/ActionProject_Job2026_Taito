using UnityEngine;

public class ClearAreaSpawner : MonoBehaviour
{
    public GameObject clearAreaPrefab; // Trigger付きプレハブ
    public Transform spawnPoint;
    public AudioClip spawnSE;


    public void SpawnArea()
    {

        // SE再生
        if (spawnSE != null)
        {
            AudioSource.PlayClipAtPoint(spawnSE, spawnPoint.position);
        }

        // クリアエリア生成
        Instantiate(clearAreaPrefab, spawnPoint.position, Quaternion.identity);

        Debug.Log("[HiddenClear] クリアエリア生成");
    }
}

