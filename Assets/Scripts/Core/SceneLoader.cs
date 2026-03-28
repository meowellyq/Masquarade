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
                RestoreVariables();
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

                RestoreVariables(); // ← восстанавливаем все переменные из GSM

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

        // ─── Восстановление переменных из GSM в Yarn Storage ───
        private void RestoreVariables()
        {
            if (GameStateManager.Instance == null) return;

            var storage = FindObjectOfType<InMemoryVariableStorage>();
            if (storage == null)
            {
                Debug.LogWarning("[SceneLoader] InMemoryVariableStorage не найден!");
                return;
            }

            var gsm = GameStateManager.Instance;

            // Оси
            storage.SetValue("$control", gsm.control);
            storage.SetValue("$world",   gsm.world);
            storage.SetValue("$truth",   gsm.truth);

            // Ключи
            storage.SetValue("$golden_keys",         gsm.goldenKeys);
            storage.SetValue("$silver_keys",         gsm.silverKeys);
            storage.SetValue("$both_keys_collected", gsm.BothKeysCollected());

            // Выборы сцен
            storage.SetValue("$scene10_choice", (float)gsm.scene10Choice);

            Debug.Log($"[SceneLoader] Переменные восстановлены: " +
                      $"C={gsm.control} W={gsm.world} T={gsm.truth} | " +
                      $"scene10_choice={gsm.scene10Choice}");
        }

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}