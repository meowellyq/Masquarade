using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Core;

namespace Labyrinth
{
    public class DoorTrigger : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI notificationText;

        [Header("Yarn нода после входа в дверь")]
        public string entryYarnNode = "Scene10_Start";

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

            Debug.Log($"[DoorTrigger] Вошёл: {other.name}, Tag: {other.tag}");

            if (GameStateManager.Instance.pondVisited)
            {
                _isTriggering = true;
                Debug.Log("[DoorTrigger] Дверь открыта. Запускаем Yarn-ноду.");
                GameStateManager.Instance.currentYarnNode = entryYarnNode;
                SceneManager.LoadScene("DialogueScene");
            }
            else
            {
                Debug.Log("[DoorTrigger] Дверь закрыта — пруд не посещён.");
                ShowHint("Дверь не поддаётся.\nМожет, стоит осмотреть пруд?");
            }
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