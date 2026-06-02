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
    [Tooltip("この文字がダメージを受けたときのノックバックの大きさ。0にすれば一切後退しません。")]
    public float knockbackDistance = 4f;

    [Header("--- 【2】耐久度（HP）の設定 ---")]
    public int maxHP = 3;
    private int currentHP;

    [Header("--- 【3】フィニッシュ・スローの設定 ---")]
    public float slowTimeScale = 0.2f;
    public float slowHoldDuration = 0.05f;
    public float slowRecoverDuration = 0.5f;

    [Header("--- 【4】ハサミ斬撃の設定 ---")]
    public float minSlashAngle = -45f;
    public float maxSlashAngle = 45f;

    [Header("--- 【5】ダメージ色（ヒビ表現）の設定 ---")]
    public Color normalColor = Color.white;
    public Color damagedColor1 = new Color(1f, 0.6f, 0.2f);
    public Color damagedColor2 = Color.red;

    [Header("--- 【新規】アニメーション同期用の変数 ---")]
    // ★Animatorからはマテリアルではなく、この「表面の数値」を直接録画してもらいます！
    [Range(0, 1)] public float glitchAmount = 0f;
    public float mosaicSize = 100f;

    // 自分がボスかどうかの判定用
    [HideInInspector] public bool IsBoss = false;

    private TextMeshPro textMeshPro;
    private SpriteRenderer spriteRenderer;
    private Material runtimeMaterial;

    void Start()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        currentHP = maxHP;
        textMeshPro = GetComponent<TextMeshPro>();
        if (textMeshPro != null) textMeshPro.color = normalColor;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.material != null)
        {
            // マテリアルを安全に複製
            runtimeMaterial = new Material(spriteRenderer.material);
            spriteRenderer.material = runtimeMaterial;
        }
    }

    void Update()
    {
        transform.Translate(0, 0, -currentSpeed * Time.deltaTime);

        // ★【超重要】毎フレーム、Animatorが動かしている「表面の数値」をマテリアルの奥底に送り込む！
        if (runtimeMaterial != null)
        {
            runtimeMaterial.SetFloat("_GlitchAmount", glitchAmount);
            runtimeMaterial.SetFloat("_MosaicSize", mosaicSize);
        }

        if (transform.position.z < -2f)
        {
            Destroy(gameObject);
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
        }

        if (textMeshPro != null)
        {
            if (currentHP == 2) textMeshPro.color = damagedColor1;
            else if (currentHP == 1) textMeshPro.color = damagedColor2;
        }

        if (spriteRenderer != null)
        {
            if (currentHP == 2) spriteRenderer.color = damagedColor1;
            else if (currentHP == 1) spriteRenderer.color = damagedColor2;
        }

        if (currentHP > 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + knockbackDistance);
        }
        else
        {
            if (effectPrefab != null) Instantiate(effectPrefab, transform.position, Quaternion.identity);

            if (shaker != null) shaker.StartFinishSlow(slowTimeScale, slowHoldDuration, slowRecoverDuration);

            if (StageManager.Instance != null)
            {
                StageManager.Instance.OnObstacleDestroyed();
            }

            if (runtimeMaterial != null) Destroy(runtimeMaterial);

            Destroy(gameObject);
        }
    }
}