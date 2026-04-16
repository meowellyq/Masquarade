using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace Labyrinth
{
    // Триггер возврата в беседку с флаконом (после Scene17)
    // Активен только когда flask != "" и wrathEchoDone == true
    public class GazeboReturnTrigger : MonoBehaviour
    {
        private bool _isTriggering = false;

        void Start()
        {
            var gsm = GameStateManager.Instance;
            if (gsm == null) { gameObject.SetActive(false); return; }

            // Показываем только если флакон заполнен и ещё не отдан
            bool shouldShow = gsm.wrathEchoDone
                              && !string.IsNullOrEmpty(gsm.flask)
                              && gsm.flask != "empty"
                              && !gsm.gazeboReturnDone;

            if (!shouldShow)
                gameObject.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTriggering || !other.CompareTag("Player")) return;
            if (GameStateManager.Instance == null) return;

            _isTriggering = true;
            GameStateManager.Instance.currentYarnNode = "Scene18_GazeboReturn";
            SceneManager.LoadScene("DialogueScene");
        }
    }
}