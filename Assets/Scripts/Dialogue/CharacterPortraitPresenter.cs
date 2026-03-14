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
        [SerializeField] public Sprite extravaganceSprite;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            if (portraitLeft != null)
                portraitLeft.gameObject.SetActive(false);
            if (portraitRight != null)
                portraitRight.gameObject.SetActive(false);
        }

        // ─── Поиск спрайта по имени ────────────────────────────
        private Sprite GetSpriteByName(string characterName)
        {
            switch (characterName)
            {
                case "xenobia":   return xenobiaSprite;
                case "guide":     return guideSprite;
                case "fontaine":  return fontaineSprite;
                case "playfulness": return playfulnessSprite; 
                case "extravagance": return extravaganceSprite;
                default:
                    Debug.LogWarning($"Неизвестный персонаж: '{characterName}'. " +
                                     "Доступные: xenobia, guide, fontaine, playfulness, extravagance.");
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
            portraitImage.gameObject.SetActive(true);
        }
    }
}