using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("--- 演出用の素材設定 ---")]
    [Tooltip("画面に表示するルカのカットインUIパネル")]
    public GameObject cutInUI;

    [Tooltip("巨大なボス文字のプレハブ")]
    public GameObject bossPrefab;

    private int destroyedCount = 0;
    private bool bossSpawned = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (cutInUI != null) cutInUI.SetActive(false);
    }

    // 文字が破壊されたら、Moverからこの関数が呼ばれる
    public void OnObstacleDestroyed()
    {
        if (bossSpawned) return; // ボス戦中はカウントしない

        destroyedCount++;
        Debug.Log($"文字を破壊しました！ 現在の破壊数: {destroyedCount}");

        // 2個破壊したらカットイン演出へ！
        if (destroyedCount >= 2)
        {
            StartCoroutine(PlayCutInAndSpawnBoss());
        }
    }

    private IEnumerator PlayCutInAndSpawnBoss()
    {
        bossSpawned = true;

        // 1. スポナーを止める
        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null) spawner.enabled = false;

        yield return new WaitForSeconds(0.5f); // 最後の蝶が散るのを少し待つ

        // 2. ルカのカットイン画像を表示！
        Debug.Log("カットイン表示！深層解放完了！");
        if (cutInUI != null) cutInUI.SetActive(true);

        // 2秒間カットインを見せる
        yield return new WaitForSeconds(2.0f);

        // 3. カットインを消す
        if (cutInUI != null) cutInUI.SetActive(false);

        // 4. 巨大なボス文字をドォン！と生成
        if (bossPrefab != null)
        {
            Debug.Log("巨大なボス言葉が出現！");
            // 画面の奥（Z:20）の真ん中（X:0, Y:1）に生成
            Vector3 bossPosition = new Vector3(0f, 1f, 20f);
            GameObject boss = Instantiate(bossPrefab, bossPosition, Quaternion.identity);

            ObstacleMover bossMover = boss.GetComponent<ObstacleMover>();
            if (bossMover != null)
            {
                // ★ここだけ追加！ボスプレハブに「お前はボスだよ」という目印だけを教える
                // これにより、ボスプレハブに設定したスローモーション値が自動で発動します！
                bossMover.IsBoss = true;
            }
        }
    }
}