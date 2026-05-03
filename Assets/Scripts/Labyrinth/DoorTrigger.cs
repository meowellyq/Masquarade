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

            var gsm = GameStateManager.Instance;

            if (gsm.pondVisited)
            {
                _isTriggering = true;

                // Если флакон получен И Scene17 ещё не была — запускаем Scene17
                if (gsm.wrathEchoDone && !string.IsNullOrEmpty(gsm.flask) && !gsm.scene17Done)
                {
                    Debug.Log("[DoorTrigger] Флакон есть — переход в Scene17.");
                    gsm.scene17Done = true;  // ← блокируем повторный вход сразу
                    gsm.currentYarnNode = "Scene17_Inadequacy";
                }
                else if (!gsm.scene17Done)
                {
                    Debug.Log("[DoorTrigger] Обычный вход — запускаем " + entryYarnNode);
                    gsm.currentYarnNode = entryYarnNode;
                }
                else
                {
                    // Scene17 уже была — дверь больше не реагирует
                    Debug.Log("[DoorTrigger] Scene17 уже пройдена, вход заблокирован.");
                    _isTriggering = false;
                    return;
                }

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