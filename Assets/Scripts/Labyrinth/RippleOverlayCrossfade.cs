using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RippleOverlayCrossfade : MonoBehaviour
{
    [Header("UI Images")]
    public Image imageA;
    public Image imageB;

    [Header("Ripple Frames")]
    public Sprite[] sequence;

    [Header("Visual Settings")]
    [Range(0f, 1f)]
    public float maxAlpha = 0.35f;

    public float fadeDuration = 1.2f;
    public float holdDuration = 0.1f;

    private bool showingA = true;

    private IEnumerator Start()
    {
        if (imageA == null || imageB == null)
        {
            Debug.LogWarning("RippleOverlayCrossfade: Image A or Image B is missing.");
            yield break;
        }

        if (sequence == null || sequence.Length < 2)
        {
            Debug.LogWarning("RippleOverlayCrossfade: Sequence needs at least 2 sprites.");
            yield break;
        }

        imageA.raycastTarget = false;
        imageB.raycastTarget = false;

        imageA.sprite = sequence[0];
        imageB.sprite = sequence[1];

        SetAlpha(imageA, maxAlpha);
        SetAlpha(imageB, 0f);

        int currentIndex = 0;

        while (true)
        {
            int nextIndex = (currentIndex + 1) % sequence.Length;

            Image visibleImage = showingA ? imageA : imageB;
            Image hiddenImage = showingA ? imageB : imageA;

            hiddenImage.sprite = sequence[nextIndex];
            SetAlpha(hiddenImage, 0f);

            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / fadeDuration);

                SetAlpha(visibleImage, Mathf.Lerp(maxAlpha, 0f, t));
                SetAlpha(hiddenImage, Mathf.Lerp(0f, maxAlpha, t));

                yield return null;
            }

            SetAlpha(visibleImage, 0f);
            SetAlpha(hiddenImage, maxAlpha);

            showingA = !showingA;
            currentIndex = nextIndex;

            if (holdDuration > 0f)
                yield return new WaitForSeconds(holdDuration);
        }
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}