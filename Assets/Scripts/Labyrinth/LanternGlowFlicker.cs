using UnityEngine;

public class LanternGlowFlicker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer smallGlow;
    [SerializeField] private SpriteRenderer bigGlow;

    [SerializeField] private float speed = 2.5f;

    [SerializeField] private float smallBaseAlpha = 0.75f;
    [SerializeField] private float smallAlphaVariation = 0.12f;

    [SerializeField] private float bigBaseAlpha = 0.20f;
    [SerializeField] private float bigAlphaVariation = 0.05f;

    [SerializeField] private float scaleVariation = 0.04f;

    private Vector3 smallBaseScale;
    private Vector3 bigBaseScale;
    private float seed;

    private void Awake()
    {
        smallBaseScale = smallGlow.transform.localScale;
        bigBaseScale = bigGlow.transform.localScale;
        seed = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(seed, Time.time * speed);
        float flicker = (noise - 0.5f) * 2f;

        Color smallColor = smallGlow.color;
        smallColor.a = Mathf.Clamp01(smallBaseAlpha + flicker * smallAlphaVariation);
        smallGlow.color = smallColor;

        Color bigColor = bigGlow.color;
        bigColor.a = Mathf.Clamp01(bigBaseAlpha + flicker * bigAlphaVariation);
        bigGlow.color = bigColor;

        smallGlow.transform.localScale = smallBaseScale * (1f + flicker * scaleVariation);
        bigGlow.transform.localScale = bigBaseScale * (1f + flicker * scaleVariation * 0.5f);
    }
}