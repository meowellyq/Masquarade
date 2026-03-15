using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace Minigames.PowderPuzzle
{
    /// <summary>
    /// Управление паззлом "Разбитая пудреница"
    /// </summary>
    public class PuzzleManager_Extravagance : MonoBehaviour
    {
        [Header("Puzzle Config")]
        public int totalPieces = 6;
        public GameObject completionPanel;

        private int _placedPieces  = 0;
        private int _cleanSidePieces = 0;

        /// <summary>
        /// Вызывается каждым осколком при правильной установке
        /// </summary>
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
                      $"Ключ: {(isGoldenKey ? "ЗОЛОТОЙ (чистое зеркало)" : "СЕРЕБРЯНЫЙ (наклейки)")}");

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
        }

        public void OnExitButton()
        {
            Debug.Log("[Puzzle] Игрок вышел без прохождения.");
            SceneManager.LoadScene("LabyrinthScene");
        }
    }
}