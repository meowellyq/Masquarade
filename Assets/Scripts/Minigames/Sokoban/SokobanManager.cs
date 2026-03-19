using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace Minigames.Sokoban
{
    public class SokobanManager : MonoBehaviour
    {
        [Header("Ссылки")]
        public SokobanGrid grid;
        public Transform   playerTransform;

        [Header("Всего слотов для заполнения")]
        public int totalSlots = 3;

        [Header("UI")]
        public GameObject completionPanel;

        private bool _isComplete = false;

        void Start() { }

        void Update()
        {
            if (_isComplete) return;

            Vector2Int dir = Vector2Int.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    dir = Vector2Int.up;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  dir = Vector2Int.down;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  dir = Vector2Int.left;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2Int.right;

            if (dir == Vector2Int.zero) return;
            TryMove(dir);
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("[Sokoban] Игрок вышел без прохождения.");
                SceneManager.LoadScene("LabyrinthScene");
            }

#if UNITY_EDITOR
            // Отладка: F1 = золотой ключ, F2 = серебряный
            if (Input.GetKeyDown(KeyCode.F1)) DebugForceComplete(true);
            if (Input.GetKeyDown(KeyCode.F2)) DebugForceComplete(false);
#endif
        }

        void TryMove(Vector2Int dir)
        {
            Vector2Int playerCell = grid.WorldToCell(playerTransform.position);
            Vector2Int targetCell = playerCell + dir;

            PushableObject pushable = GetPushableAt(targetCell);

            if (pushable != null)
            {
                bool pushed = pushable.TryPush(dir, grid);
                if (!pushed) return;
            }
            else
            {
                if (!grid.IsCellWalkable(targetCell, true)) return;
            }

            playerTransform.position = grid.CellToWorld(targetCell);
            CheckCompletion();
        }

        PushableObject GetPushableAt(Vector2Int cell)
        {
            var all = FindObjectsOfType<PushableObject>();
            foreach (var obj in all)
            {
                if (obj.IsInSlot) continue;
                if (grid.WorldToCell(obj.transform.position) == cell)
                    return obj;
            }
            return null;
        }

        void CheckCompletion()
        {
            var slots = FindObjectsOfType<SokobanSlot>();
            int filled = 0;
            int skeletonCount = 0;

            foreach (var slot in slots)
            {
                if (slot.IsFilled)
                {
                    filled++;
                    if (slot.FilledBySkeleton) skeletonCount++;
                }
            }

            if (filled < totalSlots) return;

            _isComplete = true;
            int mannequinCount = filled - skeletonCount;
            bool isGoldenKey = skeletonCount > mannequinCount;

            Debug.Log($"[Sokoban] Завершено! Скелетов: {skeletonCount}, Манекенов: {mannequinCount}. " +
                      $"Ключ: {(isGoldenKey ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteMiniGame("inadequacy", isGoldenKey);

            if (completionPanel != null)
                completionPanel.SetActive(true);

            Invoke(nameof(ReturnToLabyrinth), 2f);
        }

        void ReturnToLabyrinth() => SceneManager.LoadScene("LabyrinthScene");

#if UNITY_EDITOR
        void DebugForceComplete(bool golden)
        {
            if (_isComplete) return;
            _isComplete = true;
            Debug.Log($"[DEBUG] Форсирую завершение Sokoban. Ключ: {(golden ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteMiniGame("inadequacy", golden);
            if (completionPanel != null)
                completionPanel.SetActive(true);
            Invoke(nameof(ReturnToLabyrinth), 2f);
        }
#endif
    }
}