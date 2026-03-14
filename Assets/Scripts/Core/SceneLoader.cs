using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace Core
{
    /// <summary>
    /// Управляет переходами между Unity-сценами и автозапуском Yarn-нод
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
                Debug.Log($"Автозапуск Yarn-ноды: {nodeToStart}");
                
                // Сбросить точку возврата (чтобы не запускалась повторно)
                GameStateManager.Instance.currentYarnNode = "";
                
                // Запустить диалог
                if (dialogueRunner != null)
                {
                    dialogueRunner.StartDialogue(nodeToStart);
                }
            }
            else
            {
                // Дефолтный старт (первая сцена)
                if (dialogueRunner != null)
                {
                    dialogueRunner.StartDialogue("Scene01_Start");
                }
            }
        }

        /// <summary>
        /// Загрузить сцену из кода C#
        /// </summary>
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}