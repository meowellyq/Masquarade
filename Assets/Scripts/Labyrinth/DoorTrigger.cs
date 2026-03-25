using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Core;

namespace Labyrinth
{
    // Дверь в Зал Печали — открывается только после сцены у пруда
    public class DoorTrigger : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI notificationText;

        [Header("Следующая сцена")]
        public string nextScene = "HallOfSorrow";

        void Start()
        {
            if (notificationText != null)
                notificationText.gameObject.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (GameStateManager.Instance == null) return;
            Debug.Log($"[DoorTrigger] Вошёл объект: {other.name}, Tag: {other.tag}");
            if (GameStateManager.Instance.pondVisited)
            {
                Debug.Log("[DoorTrigger] Дверь открыта. Переход в Зал Печали.");
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                Debug.Log("[DoorTrigger] Дверь закрыта — пруд не посещён.");
                ShowHint();
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                HideHint();
        }

        void ShowHint()
        {
            if (notificationText != null)
            {
                notificationText.text = "Дверь не поддаётся.\nМожет, стоит осмотреть пруд?";
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