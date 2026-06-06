using System.Collections;
using UnityEngine;
using TMPro;

public class JustSliceObstacle3D : MonoBehaviour
{
    [Header("--- 【基本移動設定】 ---")]
    public float minSpeed = 10f;
    public float maxSpeed = 25f;
    private float currentSpeed;

    public GameObject effectPrefab;
    public GameObject slashPrefab;

    [Header("--- 【1】ノックバックの設定 ---")]
    [Tooltip("道中でダメージを受けたときに奥へ押し戻される距離")]
    public float knockbackDistance = 4f;

    [Header("--- 【2】耐久度（HP）の設定 ---")]
    public int maxHP = 3;
    private int currentHP;

    [Header("--- 【3】フィニッシュ・スローの設定 ---")]
    public float slowTimeScale = 0.2f;
    public float slowHoldDuration = 0.05f;
    public float slowRecoverDuration = 0.5f;

    [Header("--- 【★ジャスト・スライス設定】 ---")]
    [SerializeField] private float noiseDurationMin = 1.0f; // ノイズ時間（最小値）
    [SerializeField] private float noiseDurationMax = 3.0f; // ノイズ時間（最大値）
    [SerializeField] private float chanceDuration = 0.5f;   // チャンス（晴れる）時間
    [SerializeField] private float noiseUpdateSpeed = 0.02f; // 超高速チカチカ速度

    [Header("--- 【★シェーダーのランダム範囲設定】 ---")]
    [SerializeField] private float glitchMin = 0.1f;
    [SerializeField] private float glitchMax = 0.8f;
    [SerializeField] private float mosaicMin = 10f;
    [SerializeField] private float mosaicMax = 100f;

    [Header("--- 【★真っ二つ ＆ 弾け飛び ＆ 消滅設定】 ---")]
    [Tooltip("子要素にある Text_Upper をセットしてください")]
    [SerializeField] private TextMeshPro textMeshUpper;
    [Tooltip("子要素にある Text_Lower をセットしてください")]
    [SerializeField] private TextMeshPro textMeshLower;

    [Tooltip("ハサミで斬る角度の最小値（例：-35）")]
    [SerializeField] private float minSlashAngle = -35f;
    [Tooltip("ハサミで斬る角度の最大値（例：35）")]
    [SerializeField] private float maxSlashAngle = 35f;

    [Tooltip("★【n秒】真っ二つに弾け飛んでから、モザイク消滅が始まるまでの待機時間")]
    [SerializeField] private float delayBeforeMosaic = 0.5f;

    [Tooltip("真っ過つになったパーツが外側にパカッと弾け飛ぶ距離")]
    [SerializeField] private float popDistance = 0.6f;

    [Tooltip("断面からシュワシュワ消え去るまでの時間（秒）")]
    [SerializeField] private float destroyDuration = 0.8f;

    private Material matUpper;
    private Material matLower;
    private bool isJustTiming = false;
    private bool isDestroying = false;
    private int playerHP = 100;

    void Start()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        currentHP = maxHP;

        // インスペクターで設定した文字の色だけを適用します。
        if (textMeshUpper != null) textMeshUpper.color = Color.white;
        if (textMeshLower != null) textMeshLower.color = Color.white;

        // TextMeshProの一番下にある自作フォントマテリアルを取得
        if (textMeshUpper != null) matUpper = textMeshUpper.fontMaterial;
        if (textMeshLower != null) matLower = textMeshLower.fontMaterial;

        // 最初は切断モードをOFF、消滅進行度を通常(0)にしておく
        if (matUpper != null) { matUpper.SetFloat("_IsDestroyed", 0f); matUpper.SetFloat("_DestroyProgress", 0f); }
        if (matLower != null) { matLower.SetFloat("_IsDestroyed", 0f); matLower.SetFloat("_DestroyProgress", 0f); }

        StartCoroutine(WordStateLoop());
    }

    void Update()
    {
        if (!isDestroying)
        {
            transform.Translate(0, 0, -currentSpeed * Time.deltaTime);
        }

        if (transform.position.z < -2f)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator WordStateLoop()
    {
        while (currentHP > 0)
        {
            isJustTiming = false;
            float elapsed = 0f;
            float currentNoiseDuration = Random.Range(noiseDurationMin, noiseDurationMax);

            while (elapsed < currentNoiseDuration)
            {
                float randomGlitch = Random.Range(glitchMin, glitchMax);
                float randomMosaic = Random.Range(mosaicMin, mosaicMax);

                if (matUpper != null) { matUpper.SetFloat("_GlitchAmount", randomGlitch); matUpper.SetFloat("_MosaicSize", randomMosaic); }
                if (matLower != null) { matLower.SetFloat("_GlitchAmount", randomGlitch); matLower.SetFloat("_MosaicSize", randomMosaic); }

                yield return new WaitForSeconds(noiseUpdateSpeed);
                elapsed += noiseUpdateSpeed;
            }

            isJustTiming = true;
            if (matUpper != null) { matUpper.SetFloat("_GlitchAmount", 0f); matUpper.SetFloat("_MosaicSize", 200f); }
            if (matLower != null) { matLower.SetFloat("_GlitchAmount", 0f); matLower.SetFloat("_MosaicSize", 200f); }

            yield return new WaitForSeconds(chanceDuration);
        }
    }

    void OnMouseDown()
    {
        if (currentHP <= 0 || isDestroying) return;

        if (isJustTiming)
        {
            ProcessDamage();
        }
        else
        {
            playerHP -= 10;
            Debug.Log($"<color=red>【BLOCK!!】 ノイズに弾かれた！</color>");
            CameraShaker shaker = Camera.main.GetComponent<CameraShaker>();
            if (shaker != null) shaker.TriggerShake();
        }
    }

    private void ProcessDamage()
    {
        currentHP--;

        CameraShaker shaker = Camera.main.GetComponent<CameraShaker>();
        if (shaker != null) shaker.TriggerShake();

        // ★【限定対応】インスペクターで指定された min〜max の範囲で角度をランダム決定
        float randomSlashAngle = Random.Range(minSlashAngle, maxSlashAngle);

        if (slashPrefab != null)
        {
            Quaternion slashRotation = Quaternion.Euler(0f, 0f, randomSlashAngle);
            Instantiate(slashPrefab, transform.position, slashRotation);
        }

        if (currentHP > 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + knockbackDistance);
        }
        else
        {
            isDestroying = true;
            StopAllCoroutines();

            if (effectPrefab != null) Instantiate(effectPrefab, transform.position, Quaternion.identity);
            if (shaker != null) shaker.StartFinishSlow(slowTimeScale, slowHoldDuration, slowRecoverDuration);

            StartCoroutine(AnimateSlashAndBlockDestroy(randomSlashAngle));
        }
    }

    IEnumerator AnimateSlashAndBlockDestroy(float slashAngle)
    {
        if (matUpper != null) { matUpper.SetFloat("_IsDestroyed", 1f); matUpper.SetFloat("_CutAngle", slashAngle); matUpper.SetFloat("_CutSide", 1f); }
        if (matLower != null) { matLower.SetFloat("_IsDestroyed", 1f); matLower.SetFloat("_CutAngle", slashAngle); matLower.SetFloat("_CutSide", -1f); }

        Vector3 slashDirection = Quaternion.Euler(0, 0, slashAngle) * Vector3.up;

        Vector3 startUpperPos = textMeshUpper.transform.localPosition;
        Vector3 startLowerPos = textMeshLower.transform.localPosition;
        Vector3 targetUpperPos = startUpperPos + (slashDirection * popDistance);
        Vector3 targetLowerPos = startLowerPos - (slashDirection * popDistance);

        float popElapsed = 0f;
        float popDuration = 0.15f;

        while (popElapsed < popDuration)
        {
            popElapsed += Time.deltaTime;
            float t = Mathf.Sin((popElapsed / popDuration) * Mathf.PI * 0.5f);

            textMeshUpper.transform.localPosition = Vector3.Lerp(startUpperPos, targetUpperPos, t);
            textMeshLower.transform.localPosition = Vector3.Lerp(startLowerPos, targetLowerPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeMosaic);

        float elapsed = 0f;
        while (elapsed < destroyDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / destroyDuration);

            if (matUpper != null) matUpper.SetFloat("_DestroyProgress", progress);
            if (matLower != null) matLower.SetFloat("_DestroyProgress", progress);

            yield return null;
        }

        if (StageManager.Instance != null) StageManager.Instance.OnObstacleDestroyed();
        Destroy(gameObject);
    }
}