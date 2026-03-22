using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace Core
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("References")]
        public DialogueRunner dialogueRunner;

        [Header("Тест (только для разработки)")]
        [Tooltip("Если заполнено — запустит эту ноду вместо сохранённой. Очисти перед финальной сборкой!")]
        public string debugStartNode = "";

        void Start()
        {
            Debug.Log($"[SceneLoader] Instance существует: {GameStateManager.Instance != null}");
            Debug.Log($"[SceneLoader] currentYarnNode = '{GameStateManager.Instance?.currentYarnNode}'");

            // Тестовый режим — приоритет над всем
            if (!string.IsNullOrEmpty(debugStartNode))
            {
                Debug.Log($"[SceneLoader] ТЕСТ: запускаем ноду '{debugStartNode}'");
                dialogueRunner?.StartDialogue(debugStartNode);
                return;
            }

            if (GameStateManager.Instance != null &&
                !string.IsNullOrEmpty(GameStateManager.Instance.currentYarnNode))
            {
                string nodeToStart = GameStateManager.Instance.currentYarnNode;
                Debug.Log($"[SceneLoader DEBUG] currentYarnNode = '{nodeToStart}' / пустая: {string.IsNullOrEmpty(nodeToStart)}");
                Debug.Log($"[SceneLoader] Автозапуск Yarn-ноды: {nodeToStart}");

                GameStateManager.Instance.currentYarnNode = "";

                if (dialogueRunner != null)
                    dialogueRunner.StartDialogue(nodeToStart);
                else
                    Debug.LogError("[SceneLoader] DialogueRunner не назначен в Inspector!");
            }
            else
            {
                if (dialogueRunner != null)
                {
                    Debug.Log("[SceneLoader] Запуск с начала игры");
                    dialogueRunner.StartDialogue("Scene01_Start");
                }
                else
                    Debug.LogError("[SceneLoader] DialogueRunner не назначен в Inspector!");
            }
        }

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}