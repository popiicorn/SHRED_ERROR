using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    // スピードの最低値と最高値を設定できるようにします
    public float minSpeed = 10f;
    public float maxSpeed = 25f;

    // 実際にこの箱が使うスピード（内部計算用）
    private float currentSpeed;

    public GameObject effectPrefab;

    void Start()
    {
        // ★ここが新しい仕組み！
        // この箱が生まれた瞬間に、minSpeed（10）から maxSpeed（25）の間の数字をランダムで1つ選ぶ
        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        // 毎フレーム、ランダムに決まったスピードで手前に移動させる
        transform.Translate(0, 0, -currentSpeed * Time.deltaTime);

        if (transform.position.z < -2f)
        {
            Destroy(gameObject);
            Debug.Log("箱を切れなかった…（激突）");
        }
    }

    void OnMouseDown()
    {
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }

        // ★ここを追加！画面にいるメインカメラからCameraShakerを見つけて「揺らせ！」と命令する
        Camera.main.GetComponent<CameraShaker>().TriggerShake();

        Destroy(gameObject);
        Debug.Log("チョキリ！ハサミで切り裂いた！エフェクト発生！画面シェイク！");
    }
}