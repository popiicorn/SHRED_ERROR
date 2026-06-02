using UnityEngine;
using System.Collections;
using UnityEngine.UI; // UI（RawImage）を動かすためのお守り

public class StageManager : MonoBehaviour
{
    // 他のスクリプトから通信できるようにするシングルトンのお約束
    public static StageManager Instance { get; private set; }

    [Header("--- 既存の設定 ---")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject cutInUI;

    [Header("--- ボス登場の演出調整 ---")]
    [Tooltip("カットインが消えた後、ボスが出現するまでの『静寂（タメ）』の秒数")]
    [SerializeField] private float bossSpawnDelay = 0.8f;

    [Tooltip("ボスが動き始めてから定位置に到着するまでの秒数")]
    [SerializeField] private float entranceDuration = 1.0f;

    [Tooltip("ボスの登場「開始」位置")]
    [SerializeField] private Vector3 bossStartPos = new Vector3(0f, 1f, 35f);

    [Tooltip("ボスの登場「終了（戦闘）」位置")]
    [SerializeField] private Vector3 bossEndPos = new Vector3(0f, 1f, 15f);

    [Header("--- 【新規】ボス出現の条件設定 ---")]
    [Tooltip("ボスを出すために、事前に破壊する必要があるザコ文字の数")]
    [SerializeField] private int requiredEnemyDefeatCount = 2; // ★インスペクターで自由に変えられます！

    [Header("--- 現在のステータス（確認用） ---")]
    [SerializeField] private int currentEnemyDefeatCount = 0; // 今何個壊したか

    private bool bossSpawned = false;
    private bool isInitialized = false;

    // 最初に呼び出される初期化処理
    private void Awake()
    {
        Instance = this;

        // ゲーム開始直後はカットインのパネルを強制的に非表示にしておく安全対策
        if (cutInUI != null) cutInUI.SetActive(false);

        StartCoroutine(EnableManagerAfterFrame());
    }

    private IEnumerator EnableManagerAfterFrame()
    {
        yield return null; // 最初の1フレームだけ待つ
        isInitialized = true;
    }

    // ObstacleMoverから呼び出される、ザコ文字が壊れた時の関数
    public void OnObstacleDestroyed()
    {
        // 起動直後の誤作動防止
        if (!isInitialized) return;

        // すでにボスが出現しているなら、カウントはしない
        if (bossSpawned) return;

        // 破壊数を1個カウントアップ！
        currentEnemyDefeatCount++;

        // ★設定した「必要撃破数」に達したら、満を持してボス演出をスタート！
        if (currentEnemyDefeatCount >= requiredEnemyDefeatCount)
        {
            StartCoroutine(PlayCutInAndSpawnBoss());
        }
    }

    private IEnumerator PlayCutInAndSpawnBoss()
    {
        bossSpawned = true;

        // ザコ敵のスポナーを止める
        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null) spawner.enabled = false;

        yield return new WaitForSeconds(0.5f);

        // 1. カットイン再生開始
        if (cutInUI != null)
        {
            cutInUI.SetActive(true);
            Animator anim = cutInUI.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("Luka_Mosaic_Anim", 0, 0f);
            }
        }

        // カットインが完全に表示され、モザイクが晴れるまでの時間（2秒）待つ
        yield return new WaitForSeconds(2.0f);

        // 使い終わったらマテリアルを初期値（通常）に戻す
        RawImage rawImg = cutInUI.GetComponent<RawImage>();
        if (rawImg != null && rawImg.material != null)
        {
            rawImg.material.SetFloat("_GlitchAmount", 0f);
            rawImg.material.SetFloat("_MosaicSize", 100f);
        }

        // 2. カットインを非表示にする
        if (cutInUI != null) cutInUI.SetActive(false);

        // インスペクターで設定した「タメの時間」だけ待つ
        yield return new WaitForSeconds(bossSpawnDelay);

        // 3. ボスを満を持して生成！
        if (bossPrefab != null)
        {
            GameObject boss = Instantiate(bossPrefab, bossStartPos, Quaternion.identity);

            ObstacleMover bossMover = boss.GetComponent<ObstacleMover>();
            if (bossMover != null)
            {
                bossMover.IsBoss = true;
            }

            // 4. ボスの専用登場アクション
            StartCoroutine(BossEntranceAnimation(boss.transform));
        }
    }

    private IEnumerator BossEntranceAnimation(Transform bossTransform)
    {
        float elapsed = 0f;

        while (elapsed < entranceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / entranceDuration;
            t = t * t; // イージング

            if (bossTransform != null)
            {
                bossTransform.position = Vector3.Lerp(bossStartPos, bossEndPos, t);
            }
            yield return null;
        }

        if (bossTransform != null) bossTransform.position = bossEndPos;
    }
}