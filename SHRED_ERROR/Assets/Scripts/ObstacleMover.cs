using UnityEngine;
using TMPro; // ★TextMesh Proをプログラムでいじれるようにする魔法の呪文

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
    public float minSlashAngle = -45f;
    public float maxSlashAngle = 45f;

    [Header("--- 【5】ダメージ色（ヒビ表現）の設定 ---")]
    // ★インスペクターで段階ごとの文字の色を決められるようにします！
    [Tooltip("無傷（HP3）のときの文字の色")]
    public Color normalColor = Color.white;

    [Tooltip("1回殴られた（HP2）ときの文字の色")]
    public Color damagedColor1 = new Color(1f, 0.6f, 0.2f); // オレンジっぽいの

    [Tooltip("2回殴られてボロボロ（HP1）のときの文字の色")]
    public Color damagedColor2 = Color.red; // 真っ赤！

    // TextMeshProのコンポーネントを記憶しておく変数
    private TextMeshPro textMeshPro;

    void Start()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        currentHP = maxHP;

        // 自分のオブジェクトにくっついているTextMeshProを自動で見つけて持ってくる
        textMeshPro = GetComponent<TextMeshPro>();

        // 最初は通常の色にしておく
        if (textMeshPro != null)
        {
            textMeshPro.color = normalColor;
        }
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

        // 斬撃エフェクトを出す
        if (slashPrefab != null)
        {
            float randomAngle = Random.Range(minSlashAngle, maxSlashAngle);
            Quaternion slashRotation = Quaternion.Euler(0f, 0f, randomAngle);

            GameObject spawnedSlash = Instantiate(slashPrefab, transform.position, slashRotation);
            spawnedSlash.transform.rotation = slashRotation;
        }

        // ★新機能：残りHPに合わせて文字の色をリアルタイムに切り替える！
        if (textMeshPro != null)
        {
            if (currentHP == 2)
            {
                textMeshPro.color = damagedColor1; // 1ダメージ目の色に
            }
            else if (currentHP == 1)
            {
                textMeshPro.color = damagedColor2; // 2ダメージ目（瀕死）の色に
            }
        }

        if (currentHP > 0)
        {
            Debug.Log($"言葉を攻撃！ 残り耐久度: {currentHP}");
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + knockbackDistance);
            transform.localScale *= 0.9f; // 文字がちょっと縮むリアクション
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