using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Core;

namespace Labyrinth
{
    /// <summary>
    /// Триггер Фонтана в лабиринте (проверяет наличие ключей)
    /// </summary>
    public class FountainTrigger : MonoBehaviour
    {
        [Header("UI Notification")]
        [Tooltip("Текстовое поле для сообщения 'Нужны все ключи' (опционально)")]
        public TextMeshProUGUI notificationText;

        void Start()
        {
            if (notificationText != null)
                notificationText.gameObject.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (GameStateManager.Instance == null) return;

            if (GameStateManager.Instance.BothMiniGamesCompleted())
            {
                Debug.Log("[FountainTrigger] Все ключи собраны. Возврат к диалогу.");
                GameStateManager.Instance.currentYarnNode = "Scene09_ReturnToFountain";
                SceneManager.LoadScene("DialogueScene");
            }
            else
            {
                Debug.Log("[FountainTrigger] Недостаточно ключей.");
                ShowNotification();
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                HideNotification();
        }

        void ShowNotification()
        {
            if (notificationText != null)
            {
                notificationText.text = "Вы не собрали все ключи. Найдите Эхо в лабиринте.";
                notificationText.gameObject.SetActive(true);
            }
        }

        void HideNotification()
        {
            if (notificationText != null)
                notificationText.gameObject.SetActive(false);
        }
    }
}