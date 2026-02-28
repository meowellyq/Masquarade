using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Dialogue
{
    public class CharacterPortraitPresenter : MonoBehaviour
    {
        private static CharacterPortraitPresenter _instance;

        [Header("Левый портрет (Ксенобия)")]
        [SerializeField] private Image portraitLeft;
        [SerializeField] private Sprite xenobiaSprite;

        [Header("Правый портрет (Проводник)")]
        [SerializeField] private Image portraitRight;
        [SerializeField] private Sprite guideSprite;

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

        [YarnCommand("show_portrait_left")]
        public static void ShowPortraitLeft(string characterName)
        {
            if (_instance == null)
            {
                Debug.LogError("CharacterPortraitPresenter не найден!");
                return;
            }
            _instance.SetPortrait(_instance.portraitLeft,
                                  characterName,
                                  _instance.xenobiaSprite);
        }

        [YarnCommand("show_portrait_right")]
        public static void ShowPortraitRight(string characterName)
        {
            if (_instance == null)
            {
                Debug.LogError("CharacterPortraitPresenter не найден!");
                return;
            }
            _instance.SetPortrait(_instance.portraitRight,
                                  characterName,
                                  _instance.guideSprite);
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
                Debug.LogWarning($"Спрайт для '{characterName}' не найден!");
                return;
            }

            portraitImage.sprite = sprite;
            portraitImage.gameObject.SetActive(true);
        }
    }
}