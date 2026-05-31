using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    // 揺れが続く時間（一瞬だけ揺らすので0.15秒）
    public float shakeDuration = 0.15f;
    // 揺れの強さ
    public float shakeMagnitude = 0.3f;

    private Vector3 originalPos;
    private float currentShakeTime = 0f;

    void Start()
    {
        // ゲーム開始時のカメラの元の位置を記憶しておく
        originalPos = transform.localPosition;
    }

    void Update()
    {
        // 揺れタイマーが動いている間は、カメラの位置をランダムにガタガタ動かす
        if (currentShakeTime > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            currentShakeTime -= Time.deltaTime;
        }
        else
        {
            // 揺れが終わったら元の位置に戻す
            transform.localPosition = originalPos;
        }
    }

    // ★他のプログラムから「画面を揺らせ！」と呼び出すための魔法の命令
    public void TriggerShake()
    {
        currentShakeTime = shakeDuration;
    }
}