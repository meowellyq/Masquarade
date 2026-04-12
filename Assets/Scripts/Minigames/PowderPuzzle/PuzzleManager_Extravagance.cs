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
        private bool _isGoldenKey    = false;

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
            _isGoldenKey = _cleanSidePieces > (totalPieces / 2);

            Debug.Log($"[Puzzle] Паззл собран! Ключ: {(_isGoldenKey ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteMiniGame("extravagance", _isGoldenKey);

            if (completionPanel != null)
                completionPanel.SetActive(true);

            Invoke(nameof(ReturnToLabyrinth), 2f);
        }

        void ReturnToLabyrinth()
        {
            // Выбрать ноду-реакцию в зависимости от результата
            string outroNode = _isGoldenKey
                ? "Echo_Extravagance_Outro_Golden"
                : "Echo_Extravagance_Outro_Silver";

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.currentYarnNode = outroNode;

            Debug.Log($"[Puzzle] Переход в DialogueScene, нода: {outroNode}");
            SceneManager.LoadScene("DialogueScene");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnExitButton();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F7)) DebugForceComplete(true);  // было F1
            if (Input.GetKeyDown(KeyCode.F8)) DebugForceComplete(false); // было F2
#endif
        }

        public void OnExitButton()
        {
            // Выход без прохождения — просто обратно в лабиринт без диалога
            Debug.Log("[Puzzle] Игрок вышел без прохождения.");
            SceneManager.LoadScene("LabyrinthScene");
        }

#if UNITY_EDITOR
        void DebugForceComplete(bool golden)
        {
            Debug.Log($"[DEBUG] Форсирую завершение паззла. Ключ: {(golden ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");
            _isGoldenKey = golden;
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteMiniGame("extravagance", golden);
            if (completionPanel != null)
                completionPanel.SetActive(true);
            Invoke(nameof(ReturnToLabyrinth), 2f);
        }
#endif
    }
}