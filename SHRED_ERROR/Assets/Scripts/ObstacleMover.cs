using UnityEngine;
using TMPro;

public class ObstacleMover : MonoBehaviour
{
    public float minSpeed = 10f;
    public float maxSpeed = 25f;
    private float currentSpeed;

    public GameObject effectPrefab;
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
    public Color normalColor = Color.white;
    public Color damagedColor1 = new Color(1f, 0.6f, 0.2f);
    public Color damagedColor2 = Color.red;

    // ★今回のエラーの原因：この1行が足りていませんでした！
    // 自分がボスかどうかの判定用（デフォルトはザコ(false)）
    [HideInInspector] public bool IsBoss = false;

    private TextMeshPro textMeshPro;

    void Start()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        currentHP = maxHP;
        textMeshPro = GetComponent<TextMeshPro>();
        if (textMeshPro != null) textMeshPro.color = normalColor;
    }

    void Update()
    {
        transform.Translate(0, 0, -currentSpeed * Time.deltaTime);
        if (transform.position.z < -2f)
        {
            Destroy(gameObject);
            Debug.Log("言葉を粉砕できなかった…");
        }
    }

    void OnMouseDown()
    {
        currentHP--;

        CameraShaker shaker = Camera.main.GetComponent<CameraShaker>();
        if (shaker != null) shaker.TriggerShake();

        if (slashPrefab != null)
        {
            float randomAngle = Random.Range(minSlashAngle, maxSlashAngle);
            Quaternion slashRotation = Quaternion.Euler(0f, 0f, randomAngle);
            GameObject spawnedSlash = Instantiate(slashPrefab, transform.position, slashRotation);
            spawnedSlash.transform.rotation = slashRotation;
        }

        if (textMeshPro != null)
        {
            if (currentHP == 2) textMeshPro.color = damagedColor1;
            else if (currentHP == 1) textMeshPro.color = damagedColor2;
        }

        if (currentHP > 0)
        {
            // ★ボスだけノックバックを少し控えめにする（巨体なので重くリアクションさせるため）
            float actualKnockback = IsBoss ? knockbackDistance * 0.5f : knockbackDistance;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + actualKnockback);

            transform.localScale *= 0.9f;
        }
        else
        {
            if (effectPrefab != null) Instantiate(effectPrefab, transform.position, Quaternion.identity);

            // プレハブ側に設定したフィニッシュスローがそのまま発動します！
            if (shaker != null) shaker.StartFinishSlow(slowTimeScale, slowHoldDuration, slowRecoverDuration);

            if (StageManager.Instance != null)
            {
                StageManager.Instance.OnObstacleDestroyed();
            }

            Destroy(gameObject);
        }
    }
}