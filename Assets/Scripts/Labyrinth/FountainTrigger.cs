using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Core;

namespace Labyrinth
{
    // Триггер Фонтана в лабиринте
    // Первый визит (до ключей): подсказка собрать ключи
    // Второй визит (оба ключа): запускает Scene09_ReturnToFountain
    // После сдачи ключей: показывает короткое сообщение, больше не реагирует
    public class FountainTrigger : MonoBehaviour
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

            // Фаза 3 — монета получена (после Scene18.5) → Scene19
            if (gsm.gazeboReturnDone)
            {
                _isTriggering = true;
                Debug.Log("[FountainTrigger] Монета получена. Запускаем Scene19.");
                gsm.currentYarnNode = "Scene19_Start";
                SceneManager.LoadScene("DialogueScene");
                return;
            }

            // Фаза 2 — фонтан пройден, ждём пруда
            if (gsm.fountainDone)
            {
                if (!gsm.pondVisited)
                    ShowHint("Иди к пруду в правом углу лабиринта.");
                return;
            }

            // Фаза 1 — оба ключа есть → Scene09
            if (gsm.BothMiniGamesCompleted())
            {
                _isTriggering = true;
                Debug.Log("[FountainTrigger] Оба ключа собраны. Запускаем Scene09.");
                gsm.currentYarnNode = "Scene09_ReturnToFountain";
                SceneManager.LoadScene("DialogueScene");
                return;
            }

            // Ключей не хватает — подсказка
            int missing = 0;
            if (!gsm.hasExtravaganceKey) missing++;
            if (!gsm.hasInadequacyKey)   missing++;

            string hint = missing == 2
                ? "Мне нужно два ключа. Найди Эхо в лабиринте."
                : "Осталось найти ещё один ключ.";

            ShowHint(hint);
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