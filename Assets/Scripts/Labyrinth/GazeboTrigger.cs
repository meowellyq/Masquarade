using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace Labyrinth
{
    /// <summary>
    /// Триггер беседки — активируется только после того как
    /// Эхо Ярости пройдено (wrathEchoDone == true) и флакон заполнен ($flask != "").
    /// Запускает Scene18 (возврат с флаконом к Эху Ярости).
    /// </summary>
    public class GazeboTrigger : MonoBehaviour
    {
        [Header("Yarn нода")]
        [Tooltip("Нода которая запустится. Пример: Scene18_GazeboReturn")]
        public string yarnNode = "Scene18_GazeboReturn";

        private bool _triggered = false;

        void Start()
        {
            // Если эхо ещё не пройдено — сразу выключаемся,
            // EchoTrigger сам разбудит нас через флаг (см. ниже).
            // Проще — просто проверяем каждый раз при входе.
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered || !other.CompareTag("Player")) return;

            var gsm = GameStateManager.Instance;
            if (gsm == null) return;

            // Условие: Эхо пройдено И флакон заполнен
            if (!gsm.wrathEchoDone) return;
            if (string.IsNullOrEmpty(gsm.flask) || gsm.flask == "empty") return;

            _triggered = true;
            gsm.currentYarnNode = yarnNode;
            Debug.Log($"[GazeboTrigger] Условия выполнены, переход в: {yarnNode}");
            SceneManager.LoadScene("DialogueScene");
        }
    }
}