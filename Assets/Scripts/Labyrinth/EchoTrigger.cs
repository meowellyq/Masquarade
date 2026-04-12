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
        [Tooltip("Тип Эха: extravagance, inadequacy, wrath")]
        public string echoType = "extravagance";

        [Tooltip("Имя Unity-сцены мини-игры (запасной вариант)")]
        public string miniGameSceneName = "Minigame_Mirror";

        [Tooltip("Если true — запускает диалог сразу без панели подтверждения")]
        public bool skipConfirmation = false;

        [Header("Yarn нода — вступление")]
        public string introYarnNode = "Echo_Extravagance_Intro";

        [Header("Visual (опционально)")]
        public SpriteRenderer echoSprite;

        [Header("Confirmation UI")]
        public GameObject confirmationPanel;
        public TextMeshProUGUI confirmationText;
        public Button confirmButton;
        public Button cancelButton;

        private bool _isCompleted = false;
        private bool _isTriggering = false; // ← защита от повторного срабатывания
        private Vector3 _playerPositionOnEnter;

        void Start()
        {
            if (GameStateManager.Instance != null)
            {
                var gsm = GameStateManager.Instance;

                // Эхо Ярости недоступно пока Зал Печали не посещён
                if (echoType == "wrath" && !gsm.hallOfSorrowEntered)
                {
                    Debug.Log("[EchoTrigger] Эхо Ярости заблокировано — Зал Печали ещё не посещён.");
                    gameObject.SetActive(false);
                    return;
                }

                // Проверяем завершённость по типу
                _isCompleted = echoType switch
                {
                    "extravagance" => gsm.hasExtravaganceKey,
                    "inadequacy"   => gsm.hasInadequacyKey,
                    "wrath"        => gsm.wrathEchoDone,
                    _              => false
                };

                if (_isCompleted)
                {
                    Debug.Log($"[EchoTrigger] {echoType} уже пройдена. Триггер скрыт.");
                    gameObject.SetActive(false);
                }
            }

            HidePanel();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_isCompleted || _isTriggering || !other.CompareTag("Player")) return;

            _playerPositionOnEnter = other.transform.position;
            Debug.Log($"[EchoTrigger] Позиция входа сохранена: {_playerPositionOnEnter}");

            if (skipConfirmation)
            {
                _isTriggering = true;
                OnConfirm();
            }
            else
            {
                ShowPanel();
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isTriggering = false;
                HidePanel();
            }
        }

        void ShowPanel()
        {
            if (confirmationPanel == null) return;

            if (confirmationText != null)
            {
                string echoName = echoType switch
                {
                    "extravagance" => "Эхо Экстравагантности",
                    "inadequacy"   => "Эхо Неполноценности",
                    "wrath"        => "Эхо Ярости",
                    _              => "Эхо"
                };
                confirmationText.text = $"Перед вами {echoName}.\nВы готовы войти?";
            }

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
            _isCompleted = true; // ← сразу блокируем повторный вход

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.labyrinthReturnPosition = _playerPositionOnEnter;
                GameStateManager.Instance.spawnPointId = "return_position";

                if (!string.IsNullOrEmpty(introYarnNode))
                {
                    GameStateManager.Instance.currentYarnNode = introYarnNode;
                    Debug.Log($"[EchoTrigger] Переход в DialogueScene, нода: {introYarnNode}");
                    SceneManager.LoadScene("DialogueScene");
                }
            }
            else
            {
                Debug.LogWarning("[EchoTrigger] GameStateManager не найден.");
                SceneManager.LoadScene(miniGameSceneName);
            }
        }

        void OnCancel()
        {
            _isTriggering = false;
            Debug.Log($"[EchoTrigger] Отменено игроком.");
            HidePanel();
        }

        void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}