using UnityEngine;
using UnityEngine.UI;

public class MaterialBridge : MonoBehaviour
{
    // Animatorから動かしたい数値
    [Range(0, 1)] public float glitchAmount;
    public float mosaicSize = 30;

    private Material targetMaterial;
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage != null)
        {
            // マテリアルを複製して「このオブジェクト専用」にする（元のファイルを汚さないため）
            targetMaterial = Instantiate(rawImage.material);
            rawImage.material = targetMaterial;
        }
    }

    void Update()
    {
        if (targetMaterial != null)
        {
            // 変数の数値をシェーダーに送り込む
            // ※Shader Graphで作った「Reference」の名前と一致させる必要があります
            targetMaterial.SetFloat("_GlitchAmount", glitchAmount);
            targetMaterial.SetFloat("_MosaicSize", mosaicSize);
        }
    }
}