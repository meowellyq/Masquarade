using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Masquerade.Dialogue
{
    public class CharacterPortraitPresenter : MonoBehaviour
    {
        // Статическая ссылка на себя — надёжнее чем FindFirstObjectByType
        private static CharacterPortraitPresenter _instance;

        [Header("UI")]
        [SerializeField] private Image portraitImage;

        [Header("Спрайты персонажей")]
        [SerializeField] private Sprite xenobiaSprite;
        [SerializeField] private Sprite guideSprite;

        private void Awake()
        {
            // Запоминаем себя при старте
            _instance = this;
        }

        private void Start()
        {
            if (portraitImage != null)
                portraitImage.gameObject.SetActive(false);
        }

        [YarnCommand("show_portrait")]
        public static void ShowPortrait(string characterName)
        {
            if (_instance == null)
            {
                Debug.LogError("CharacterPortraitPresenter не найден на сцене!");
                return;
            }
            _instance.SetPortrait(characterName);
        }

        [YarnCommand("hide_portrait")]
        public static void HidePortrait()
        {
            if (_instance == null) return;

            if (_instance.portraitImage != null)
                _instance.portraitImage.gameObject.SetActive(false);
        }

        private void SetPortrait(string characterName)
        {
            Sprite targetSprite = characterName.ToLower() switch
            {
                "xenobia" => xenobiaSprite,
                "guide"   => guideSprite,
                _         => null
            };

            if (targetSprite == null)
            {
                Debug.LogWarning($"Спрайт для '{characterName}' не найден! " +
                                 $"Проверь поля Xenobia Sprite и Guide Sprite в Inspector.");
                return;
            }

            portraitImage.sprite = targetSprite;
            portraitImage.gameObject.SetActive(true);
        }
    }
}