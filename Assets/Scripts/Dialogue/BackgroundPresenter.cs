using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Dialogue
{
    public class BackgroundPresenter : MonoBehaviour
    {
        private static BackgroundPresenter _instance;

        [Header("Фон сцены")]
        [SerializeField] private Image backgroundImage;

        [Header("Спрайты фонов")]
        [SerializeField] private Sprite[] backgrounds;
        [SerializeField] private string[] backgroundNames;

        private void Awake()
        {
            _instance = this;
        }

        [YarnCommand("set_background")]
        public static void SetBackground(string bgName)
        {
            if (_instance == null)
            {
                Debug.LogError("BackgroundPresenter не найден на сцене!");
                return;
            }
            _instance.ShowBackground(bgName);
        }

        private void ShowBackground(string bgName)
        {
            for (int i = 0; i < backgroundNames.Length; i++)
            {
                if (backgroundNames[i] == bgName)
                {
                    backgroundImage.sprite = backgrounds[i];
                    backgroundImage.gameObject.SetActive(true);
                    return;
                }
            }
            Debug.LogWarning($"Фон '{bgName}' не найден! Проверь массив backgroundNames.");
        }

        [YarnCommand("hide_background")]
        public static void HideBackground()
        {
            if (_instance == null) return;
            _instance.backgroundImage.gameObject.SetActive(false);
        }
    }
}