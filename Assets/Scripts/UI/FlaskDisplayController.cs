using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlaskDisplayController : MonoBehaviour
{
    public static FlaskDisplayController Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;          // весь оверлей
    public Image overlay;             // полупрозрачный чёрный фон
    public Image flaskImage;          // PNG флакона по центру

    [Header("Спрайты флаконов")]
    public Sprite flaskEmpty;
    public Sprite flaskBlack;
    public Sprite flaskPink;
    public Sprite flaskGrey;

    [Header("Настройки")]
    [Range(0f, 0.8f)]
    public float overlayAlpha = 0.6f;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string flaskType)
    {
        Sprite sprite = flaskType switch
        {
            "empty" => flaskEmpty,
            "black" => flaskBlack,
            "pink"  => flaskPink,
            "grey"  => flaskGrey,
            _       => flaskEmpty
        };

        flaskImage.sprite = sprite;
        var c = overlay.color;
        c.a = overlayAlpha;
        overlay.color = c;

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    // клик в любом месте экрана
    private void Update()
    {
        if (!panel.activeSelf) return;
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            Hide();
    }
}