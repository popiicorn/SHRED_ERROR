using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float minSpeed = 10f;
    public float maxSpeed = 25f;
    private float currentSpeed;

    [Tooltip("破壊したときに飛び散る『蝶』のエフェクトプレハブ")]
    public GameObject effectPrefab;

    [Tooltip("クリックしたときに画面に走る『斬撃（閃光）』のエフェクトプレハブ")]
    public GameObject slashPrefab;

    [Header("--- 【1】ノックバックの設定 ---")]
    public float knockbackDistance = 4f;

    [Header("--- 【2】耐久度（HP）の設定 ---")]
    public int maxHP = 3;
    private int currentHP;

    [Header("--- 【3】フィニッシュ・スローの設定 ---")]
    public float slowTimeScale = 0.2f;
    public float slowHoldDuration = 0.05f;
    public float slowRecoverDuration = 0.5f;

    [Header("--- 【4】ハサミ斬撃の角度設定 ---")]
    // ★ここがインスペクターでいじれるようになった新しい設定です！
    [Tooltip("斬撃の最小角度（例: -45）")]
    public float minSlashAngle = -45f;

    [Tooltip("斬撃の最大角度（例: 45）")]
    public float maxSlashAngle = 45f;

    void Start()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        currentHP = maxHP;
    }

    void Update()
    {
        transform.Translate(0, 0, -currentSpeed * Time.deltaTime);

        if (transform.position.z < -2f)
        {
            Destroy(gameObject);
            Debug.Log("言葉を粉砕できなかった…（激突）");
        }
    }

    void OnMouseDown()
    {
        currentHP--;

        // カメラのコンポーネントを取得
        CameraShaker shaker = Camera.main.GetComponent<CameraShaker>();

        if (shaker != null)
        {
            shaker.TriggerShake(); // 画面を揺らす
        }

        // クリックした文字の場所（transform.position）に斬撃を出す
        if (slashPrefab != null)
        {
            // ★インスペクターで指定した「最小〜最大」の間でランダムな角度（Z軸）を決める！
            float randomAngle = Random.Range(minSlashAngle, maxSlashAngle);

            // 3D空間の正面（Z軸が手前）を向いたまま、ランダムな角度にパキッと回転させる魔法の命令
            Quaternion slashRotation = Quaternion.Euler(0f, 0f, randomAngle);

            // 文字の場所に、計算した角度で実体化！
            GameObject spawnedSlash = Instantiate(slashPrefab, transform.position, slashRotation);

            // ★念押し対策：たまにパーティクルシステム側の設定で回転が効かないことがあるので、
            // 生成されたエフェクト自体のTransformの角度も強制的に上書きして固定します！
            spawnedSlash.transform.rotation = slashRotation;
        }

        if (currentHP > 0)
        {
            Debug.Log($"言葉を攻撃！ 残り耐久度: {currentHP}");
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + knockbackDistance);
            transform.localScale *= 0.8f;
        }
        else
        {
            Debug.Log("言葉を完全に粉砕した！");

            if (effectPrefab != null)
            {
                Instantiate(effectPrefab, transform.position, Quaternion.identity);
            }

            if (shaker != null)
            {
                shaker.StartFinishSlow(slowTimeScale, slowHoldDuration, slowRecoverDuration);
            }

            Destroy(gameObject);
        }
    }
}