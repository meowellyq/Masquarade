using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Dialogue
{
    public class CharacterPortraitPresenter : MonoBehaviour
    {
        private static CharacterPortraitPresenter _instance;

        [Header("Левый портрет")]
        [SerializeField] private Image portraitLeft;
        [SerializeField] private Sprite xenobiaSprite;

        [Header("Правый портрет")]
        [SerializeField] private Image portraitRight;
        [SerializeField] private Sprite guideSprite;
        [SerializeField] private Sprite fontaineSprite;
        [SerializeField] private Sprite playfulnessSprite;
        [SerializeField] public  Sprite extravaganceSprite;
        [SerializeField] private Sprite inadequacySprite;
        [SerializeField] private Sprite guiltSprite;
        [SerializeField] private Sprite xenosSprite;

        [Header("Маски Зала Гнева")]
        [SerializeField] private Sprite vulnerabilitySprite;
        [SerializeField] private Sprite wrathSprite;
        [SerializeField] private Sprite echoWrathSprite;

        private bool _initialized = false;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            // Гарантируем что color.a = 1 (на случай если в Inspector стоит 0)
            if (portraitLeft != null)
            {
                portraitLeft.color = Color.white;
                portraitLeft.gameObject.SetActive(false);
            }
            if (portraitRight != null)
            {
                portraitRight.color = Color.white;
                portraitRight.gameObject.SetActive(false);
            }
            _initialized = true;
        }

        private Sprite GetSpriteByName(string characterName)
        {
            switch (characterName)
            {
                case "xenobia":        return xenobiaSprite;
                case "guide":          return guideSprite;
                case "fontaine":       return fontaineSprite;
                case "playfulness":    return playfulnessSprite;
                case "extravagance":   return extravaganceSprite;
                case "inadequacy":     return inadequacySprite;
                case "guilt":          return guiltSprite;
                case "vulnerability":  return vulnerabilitySprite;
                case "wrath":          return wrathSprite;
                case "echo_wrath":     return echoWrathSprite;
                case "xenos":          return xenosSprite;
                default:
                    Debug.LogWarning($"Неизвестный персонаж: '{characterName}'.");
                    return null;
            }
        }

        [YarnCommand("show_portrait_left")]
        public static void ShowPortraitLeft(string characterName)
        {
            if (_instance == null)
            {
                Debug.LogError("CharacterPortraitPresenter не найден!");
                return;
            }
            Sprite sprite = _instance.GetSpriteByName(characterName);
            _instance.SetPortrait(_instance.portraitLeft, characterName, sprite);
        }

        [YarnCommand("show_portrait_right")]
        public static void ShowPortraitRight(string characterName)
        {
            if (_instance == null)
            {
                Debug.LogError("CharacterPortraitPresenter не найден!");
                return;
            }
            Sprite sprite = _instance.GetSpriteByName(characterName);
            _instance.SetPortrait(_instance.portraitRight, characterName, sprite);
        }

        [YarnCommand("hide_portrait_left")]
        public static void HidePortraitLeft()
        {
            if (_instance == null) return;
            if (_instance.portraitLeft != null)
                _instance.portraitLeft.gameObject.SetActive(false);
        }

        [YarnCommand("hide_portrait_right")]
        public static void HidePortraitRight()
        {
            if (_instance == null) return;
            if (_instance.portraitRight != null)
                _instance.portraitRight.gameObject.SetActive(false);
        }

        [YarnCommand("hide_all_portraits")]
        public static void HideAllPortraits()
        {
            HidePortraitLeft();
            HidePortraitRight();
        }

        private void SetPortrait(Image portraitImage, string characterName, Sprite sprite)
        {
            if (sprite == null)
            {
                Debug.LogWarning($"Спрайт для '{characterName}' не назначен в инспекторе!");
                return;
            }

            portraitImage.sprite = sprite;
            
            // ═══ FIX 1: Гарантируем что Image.color видимый ═══
            portraitImage.color = Color.white;
            
            // ═══ FIX 2: Гарантируем что Image компонент включён ═══
            portraitImage.enabled = true;
            
            // ═══ FIX 3: Принудительный toggle — сброс Canvas dirty flag ═══
            // Без этого Unity может не перестроить rendering после toggle родителя
            portraitImage.gameObject.SetActive(false);
            portraitImage.gameObject.SetActive(true);
            
            // ═══ FIX 4: Гарантируем что родительский контейнер активен ═══
            Transform parent = portraitImage.transform.parent;
            while (parent != null)
            {
                if (!parent.gameObject.activeSelf)
                    parent.gameObject.SetActive(true);
                parent = parent.parent;
            }

            // Диагностика
            Debug.Log($"[Portrait] SET '{characterName}' | " +
                      $"activeSelf={portraitImage.gameObject.activeSelf} | " +
                      $"activeInHierarchy={portraitImage.gameObject.activeInHierarchy} | " +
                      $"enabled={portraitImage.enabled} | " +
                      $"color.a={portraitImage.color.a} | " +
                      $"size={portraitImage.rectTransform.sizeDelta} | " +
                      $"sprite={(portraitImage.sprite != null ? portraitImage.sprite.name : "NULL")} | " +
                      $"canvas={portraitImage.canvas?.name ?? "NULL"}");
        }
    }
}