using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace Core
{
    /// <summary>
    /// Управляет автозапуском Yarn-нод при возврате из геймплейных сцен
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [Header("References")]
        public DialogueRunner dialogueRunner;

        void Start()
        {
            // Проверить, есть ли сохранённая точка возврата
            if (GameStateManager.Instance != null &&
                !string.IsNullOrEmpty(GameStateManager.Instance.currentYarnNode))
            {
                string nodeToStart = GameStateManager.Instance.currentYarnNode;
                Debug.Log($"[SceneLoader] Автозапуск Yarn-ноды: {nodeToStart}");

                // Очистить точку возврата (чтобы не запускалась повторно)
                GameStateManager.Instance.currentYarnNode = "";

                if (dialogueRunner != null)
                {
                    dialogueRunner.StartDialogue(nodeToStart);
                }
                else
                {
                    Debug.LogError("[SceneLoader] DialogueRunner не назначен в Inspector!");
                }
            }
            else
            {
                // Дефолтный старт (первая сцена)
                if (dialogueRunner != null)
                {
                    Debug.Log("[SceneLoader] Запуск с начала игры");
                    dialogueRunner.StartDialogue("Scene01_Start");
                }
                else
                {
                    Debug.LogError("[SceneLoader] DialogueRunner не назначен в Inspector!");
                }
            }
        }

        /// <summary>
        /// Загрузить сцену из кода C# (например, из MiniGame-контроллеров)
        /// </summary>
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}