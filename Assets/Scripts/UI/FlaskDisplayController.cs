using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlaskDisplayController : MonoBehaviour
{
    public static FlaskDisplayController Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;
    public Image overlay;
    public Image flaskImage;

    [Header("Спрайты флаконов")]
    public Sprite flaskEmpty;
    public Sprite flaskBlack;
    public Sprite flaskPink;
    public Sprite flaskGrey;

    [Header("Настройки")]
    [Range(0f, 0.8f)]
    public float overlayAlpha = 0.6f;

    private bool _inputEnabled = false;

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

        _inputEnabled = false;
        StartCoroutine(EnableInputNextFrame());
    }

    private IEnumerator EnableInputNextFrame()
    {
        yield return null;
        yield return null;
        _inputEnabled = true;
    }

    public void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        _inputEnabled = false;
    }

    private void Update()
    {
        if (!panel.activeSelf) return;
        if (!_inputEnabled) return;
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            Hide();
    }
}