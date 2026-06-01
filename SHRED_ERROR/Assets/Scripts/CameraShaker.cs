using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    // --- 画面シェイクの設定 ---
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.3f;
    private Vector3 originalPos;
    private float currentShakeTime = 0f;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (currentShakeTime > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            currentShakeTime -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }

    public void TriggerShake()
    {
        currentShakeTime = shakeDuration;
    }

    // ★【新機能】文字から呼び出されるスローモーションの命令
    public void StartFinishSlow(float slowTime, float holdTime, float recoverTime)
    {
        // 途中で他のオブジェクトが消されても安全なように、カメラ自身で時間を戻す処理をスタートする
        StartCoroutine(FinishSlowMotion(slowTime, holdTime, recoverTime));
    }

    IEnumerator FinishSlowMotion(float slowTime, float holdTime, float recoverTime)
    {
        // 1. スロー開始
        Time.timeScale = slowTime;

        // 2. 超スローをキープ
        yield return new WaitForSecondsRealtime(holdTime);

        // 3. じわじわ時間を戻す
        float elapsed = 0f;
        while (elapsed < recoverTime)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(slowTime, 1.0f, elapsed / recoverTime);
            yield return null;
        }

        // 4. 通常速度に固定
        Time.timeScale = 1.0f;
    }
}