using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Core;

namespace Labyrinth
{
    // Триггер у пруда — запускает Scene08_5_Pond
    // Срабатывает только если Фонтан уже направил сюда (fountainDone)
    // и пруд ещё не был посещён
    public class PondTrigger : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI notificationText;

        private bool _isTriggering = false;

        void Start()
        {
            if (notificationText != null)
                notificationText.gameObject.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (GameStateManager.Instance == null) return;
            if (_isTriggering) return;

            var gsm = GameStateManager.Instance;

            // Пруд уже посещён — тихо пропускаем
            if (gsm.pondVisited)
            {
                Debug.Log("[PondTrigger] Пруд уже посещён.");
                return;
            }

            // Фонтан ещё не пройден — подс��азка
            if (!gsm.fountainDone)
            {
                ShowHint("Здесь тихо. Но пока тебе нечего здесь искать.");
                return;
            }

            // Сохраняем позицию игрока перед уходом на сцену пруда
            gsm.labyrinthReturnPosition = other.transform.position;
            gsm.spawnPointId = "return_position";
            Debug.Log($"[PondTrigger] Позиция возврата сохранена: {other.transform.position}");

            // Запускаем сцену
            _isTriggering = true;
            Debug.Log("[PondTrigger] Запускаем Scene08_5_Pond.");
            gsm.currentYarnNode = "Scene08_5_Pond";
            SceneManager.LoadScene("DialogueScene");
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                HideHint();
        }

        void ShowHint(string message)
        {
            if (notificationText != null)
            {
                notificationText.text = message;
                notificationText.gameObject.SetActive(true);
            }
        }

        void HideHint()
        {
            if (notificationText != null)
                notificationText.gameObject.SetActive(false);
        }
    }
}