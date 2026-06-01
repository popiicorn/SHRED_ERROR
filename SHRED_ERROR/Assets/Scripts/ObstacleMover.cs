using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float minSpeed = 10f;
    public float maxSpeed = 25f;
    private float currentSpeed;
    public GameObject effectPrefab;

    [Header("--- 【1】ノックバックの設定 ---")]
    public float knockbackDistance = 4f;

    [Header("--- 【2】耐久度（HP）の設定 ---")]
    public int maxHP = 3;
    private int currentHP;

    [Header("--- 【3】フィニッシュ・スローの設定 ---")]
    public float slowTimeScale = 0.2f;
    public float slowHoldDuration = 0.05f;
    public float slowRecoverDuration = 0.5f;

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

            // ★ここを変更！消えないカメラ（shaker）にスローモーションの設定を丸投げする
            if (shaker != null)
            {
                shaker.StartFinishSlow(slowTimeScale, slowHoldDuration, slowRecoverDuration);
            }

            // 自分自身（文字）は安心して消滅する
            Destroy(gameObject);
        }
    }
}