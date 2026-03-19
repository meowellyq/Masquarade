using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Core;

namespace Labyrinth
{
    public class EchoTrigger : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("Тип Эха: extravagance или inadequacy")]
        public string echoType = "extravagance";

        [Tooltip("Имя Unity-сцены мини-игры (используется только как запасной вариант)")]
        public string miniGameSceneName = "Minigame_Mirror";

        [Header("Yarn нода — вступление")]
        [Tooltip("Нода которая запустится перед мини-игрой. Пример: Echo_Extravagance_Intro")]
        public string introYarnNode = "Echo_Extravagance_Intro";

        [Header("Visual (опционально)")]
        public SpriteRenderer echoSprite;

        [Header("Confirmation UI")]
        public GameObject confirmationPanel;
        public TextMeshProUGUI confirmationText;
        public Button confirmButton;
        public Button cancelButton;

        private bool _isCompleted = false;

        void Start()
        {
            if (GameStateManager.Instance != null)
            {
                _isCompleted = echoType == "extravagance"
                    ? GameStateManager.Instance.hasExtravaganceKey
                    : GameStateManager.Instance.hasInadequacyKey;

                if (_isCompleted)
                {
                    Debug.Log($"[EchoTrigger] {echoType} уже пройдена. Триггер скрыт.");
                    gameObject.SetActive(false);
                }
            }

            // Только скрываем панель — без подписок
            HidePanel();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_isCompleted || !other.CompareTag("Player")) return;
            ShowPanel();
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                HidePanel();
        }

        void ShowPanel()
        {
            if (confirmationPanel == null) return;

            if (confirmationText != null)
            {
                string echoName = echoType == "extravagance"
                    ? "Эхо Экстравагантности"
                    : "Эхо Неполноценности";
                confirmationText.text = $"Перед вами {echoName}.\nВы готовы войти?";
            }

            // Подписываемся только когда панель показывается
            // RemoveAllListeners гарантирует что второй триггер не накопится
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(OnConfirm);
            }
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(OnCancel);
            }

            confirmationPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        void HidePanel()
        {
            if (confirmationPanel != null)
            {
                // Отписываемся когда панель скрывается
                if (confirmButton != null)
                    confirmButton.onClick.RemoveAllListeners();
                if (cancelButton != null)
                    cancelButton.onClick.RemoveAllListeners();

                confirmationPanel.SetActive(false);
            }
            Time.timeScale = 1f;
        }

        void OnConfirm()
        {
            Time.timeScale = 1f;

            if (GameStateManager.Instance != null &&
                !string.IsNullOrEmpty(introYarnNode))
            {
                GameStateManager.Instance.currentYarnNode = introYarnNode;
                Debug.Log($"[EchoTrigger] Переход в DialogueScene, нода: {introYarnNode}");
                SceneManager.LoadScene("DialogueScene");
            }
            else
            {
                Debug.LogWarning("[EchoTrigger] GameStateManager не найден, прямой переход.");
                SceneManager.LoadScene(miniGameSceneName);
            }
        }

        void OnCancel()
        {
            Debug.Log($"[EchoTrigger] Отменено игроком.");
            HidePanel();
        }

        void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}