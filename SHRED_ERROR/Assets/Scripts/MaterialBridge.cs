using UnityEngine;
using UnityEngine.UI; // UI（RawImage）を触るためのお守り

public class MaterialBridge : MonoBehaviour
{
    [Range(0, 1)] public float glitchAmount = 0f;
    public float mosaicSize = 100f;

    // UI用と3D用の両方のパーツの入れ物を用意しておく
    private RawImage rawImage;
    private SpriteRenderer spriteRenderer;
    private Material runtimeMaterial;

    void OnEnable()
    {
        // 1. まずは「UI（RawImage）」がついているかチェック
        rawImage = GetComponent<RawImage>();
        if (rawImage != null && rawImage.material != null)
        {
            if (runtimeMaterial != null) Destroy(runtimeMaterial);
            runtimeMaterial = new Material(rawImage.material);
            rawImage.material = runtimeMaterial;
            return; // UIが見つかったらここで処理を抜ける
        }

        // 2. UIがなければ「3D（SpriteRenderer）」がついているかチェック
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.material != null)
        {
            if (runtimeMaterial != null) Destroy(runtimeMaterial);
            runtimeMaterial = new Material(spriteRenderer.material);
            spriteRenderer.material = runtimeMaterial;
        }
    }

    void Update()
    {
        // 動いている数値をマテリアルに流し込む（UI・3D共通）
        if (runtimeMaterial != null)
        {
            runtimeMaterial.SetFloat("_GlitchAmount", glitchAmount);
            runtimeMaterial.SetFloat("_MosaicSize", mosaicSize);
        }
    }

    void OnDisable()
    {
        if (runtimeMaterial != null) Destroy(runtimeMaterial);
    }
}