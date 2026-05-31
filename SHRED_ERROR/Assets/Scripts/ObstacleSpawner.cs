using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnInterval = 1.5f;
    private float timer;

    // ★新しい変数：左右にどれくらいバラつかせるかの幅（真ん中から左右に3メーターずつ）
    public float xRange = 3f;

    void Start()
    {
        SpawnObstacle();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab != null)
        {
            // ★ここを改造！
            // X軸（左右）の横幅を、-3 から +3 の間でランダムに決定する
            float randomX = Random.Range(-xRange, xRange);

            // 高さは0、奥行きは20のまま、左右の位置だけをランダムにした座標を作る
            Vector3 spawnPosition = new Vector3(randomX, 0f, 20f);

            // ランダムな位置に箱を生み出す！
            Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        }
    }
}