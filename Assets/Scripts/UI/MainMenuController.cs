using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

public class MainMenuController : MonoBehaviour
{
    [Header("Start Settings")]
    [SerializeField] private string dialogueSceneName = "DialogueScene";
    [SerializeField] private string firstYarnNode = "Scene01_Start";

    public void StartNewGame()
    {
        if (GameStateManager.Instance != null)
        {
            // Стартуем с первой сюжетной ноды
            GameStateManager.Instance.currentYarnNode = firstYarnNode;

            // На будущее: здесь же можно будет сбрасывать очки осей,
            // ключи, флаги мини-игр и прогресс.
        }
        else
        {
            Debug.LogWarning("[MainMenu] GameStateManager.Instance не найден. DialogueScene загрузится, но стартовая Yarn-нода может не установиться.");
        }

        SceneManager.LoadScene(dialogueSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        Debug.Log("[MainMenu] QuitGame вызван. В редакторе Unity приложение не закрывается.");
#endif
    }
}