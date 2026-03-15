using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Core;

namespace Labyrinth
{
    /// <summary>
    /// Триггер для запуска мини-игры при столкновении с Эхо маски
    /// </summary>
    public class EchoTrigger : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("Тип Эха: extravagance или inadequacy")]
        public string echoType = "extravagance";

        [Tooltip("Имя Unity-сцены мини-игры")]
        public string miniGameSceneName = "MiniGame_Extravagance";

        [Header("Visual (опционально)")]
        public SpriteRenderer echoSprite;

        [Header("Confirmation UI")]
        [Tooltip("Панель подтверждения (Canvas → Panel)")]
        public GameObject confirmationPanel;

        [Tooltip("Текст вопроса внутри панели")]
        public TextMeshProUGUI confirmationText;

        [Tooltip("Кнопка 'Да'")]
        public Button confirmButton;

        [Tooltip("Кнопка 'Нет'")]
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

            // Скрыть панель при старте и подвязать кнопки
            HidePanel();

            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirm);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancel);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_isCompleted || !other.CompareTag("Player")) return;

            Debug.Log($"[EchoTrigger] Игрок вошёл в зону {echoType}. Показываем подтверждение.");
            ShowPanel();
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // Игрок ушёл — скрыть панель без перехода
                HidePanel();
                Debug.Log($"[EchoTrigger] Игрок вышел из зоны {echoType}. Панель скрыта.");
            }
        }

        void ShowPanel()
        {
            if (confirmationPanel == null) return;

            // Обновить текст под конкретное Эхо
            if (confirmationText != null)
            {
                string echoName = echoType == "extravagance" ? "Эхо Экстравагантности" : "Эхо Неполноценности";
                confirmationText.text = $"Перед вами {echoName}.\nВы готовы войти?";
            }

            confirmationPanel.SetActive(true);
            Time.timeScale = 0f; // Пауза движения игрока
        }

        void HidePanel()
        {
            if (confirmationPanel != null)
                confirmationPanel.SetActive(false);

            Time.timeScale = 1f; // Снять паузу
        }

        void OnConfirm()
        {
            Debug.Log($"[EchoTrigger] Подтверждено. Загрузка: {miniGameSceneName}");
            Time.timeScale = 1f; // Сбросить до загрузки сцены
            SceneManager.LoadScene(miniGameSceneName);
        }

        void OnCancel()
        {
            Debug.Log($"[EchoTrigger] Отменено игроком.");
            HidePanel();
        }

        void OnDestroy()
        {
            // Гарантированно снять паузу если объект уничтожен
            Time.timeScale = 1f;
        }
    }
}
