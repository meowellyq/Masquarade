using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace Minigames.PowderPuzzle
{
    public class PuzzleManager_Extravagance : MonoBehaviour
    {
        [Header("Puzzle Config")]
        public int totalPieces = 6;
        public GameObject completionPanel;

        private int _placedPieces    = 0;
        private int _cleanSidePieces = 0;

        public void OnPiecePlaced(bool isCleanSide)
        {
            _placedPieces++;
            if (isCleanSide) _cleanSidePieces++;

            Debug.Log($"[Puzzle] Установлен осколок {_placedPieces}/{totalPieces}. Чистых: {_cleanSidePieces}");

            if (_placedPieces >= totalPieces)
                CompletePuzzle();
        }

        void CompletePuzzle()
        {
            bool isGoldenKey = _cleanSidePieces > (totalPieces / 2);

            Debug.Log($"[Puzzle] Паззл собран! " +
                      $"Ключ: {(isGoldenKey ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteMiniGame("extravagance", isGoldenKey);

            if (completionPanel != null)
                completionPanel.SetActive(true);

            Invoke(nameof(ReturnToLabyrinth), 2f);
        }

        void ReturnToLabyrinth()
        {
            SceneManager.LoadScene("LabyrinthScene");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnExitButton();

#if UNITY_EDITOR
            // Отладка: F1 = золотой ключ, F2 = серебряный
            if (Input.GetKeyDown(KeyCode.F1)) DebugForceComplete(true);
            if (Input.GetKeyDown(KeyCode.F2)) DebugForceComplete(false);
#endif
        }

        public void OnExitButton()
        {
            Debug.Log("[Puzzle] Игрок вышел без прохождения.");
            SceneManager.LoadScene("LabyrinthScene");
        }

#if UNITY_EDITOR
        void DebugForceComplete(bool golden)
        {
            Debug.Log($"[DEBUG] Форсирую завершение паззла. Ключ: {(golden ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteMiniGame("extravagance", golden);
            if (completionPanel != null)
                completionPanel.SetActive(true);
            Invoke(nameof(ReturnToLabyrinth), 2f);
        }
#endif
    }
}