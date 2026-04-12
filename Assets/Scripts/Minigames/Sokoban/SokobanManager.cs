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

        private bool _isComplete  = false;
        private bool _isGoldenKey = false;

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
            if (Input.GetKeyDown(KeyCode.F7)) DebugForceComplete(true);  // было F1
            if (Input.GetKeyDown(KeyCode.F8)) DebugForceComplete(false); // было F2
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
            FinishGame(skeletonCount > (filled - skeletonCount));
        }

        // ─── Единая точка завершения ────────────────────────────
        void FinishGame(bool golden)
        {
            if (_isComplete) return; // защита от двойного вызова
            _isComplete  = true;
            _isGoldenKey = golden;

            Debug.Log($"[Sokoban] Завершено! Ключ: {(_isGoldenKey ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteMiniGame("inadequacy", _isGoldenKey);

            // Скрываем панель на случай если она уже открыта, потом показываем чисто
            if (completionPanel != null)
            {
                completionPanel.SetActive(false);
                completionPanel.SetActive(true);
            }

            CancelInvoke(nameof(ReturnToLabyrinth)); // отменяем предыдущий invoke если был
            Invoke(nameof(ReturnToLabyrinth), 2f);
        }

        void ReturnToLabyrinth()
        {
            string outroNode = _isGoldenKey
                ? "Echo_Inadequacy_Outro_Golden"
                : "Echo_Inadequacy_Outro_Silver";

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.currentYarnNode = outroNode;

            Debug.Log($"[Sokoban] Переход в DialogueScene, нода: {outroNode}");
            SceneManager.LoadScene("DialogueScene");
        }

#if UNITY_EDITOR
        void DebugForceComplete(bool golden)
        {
            Debug.Log($"[DEBUG] Форсирую завершение Sokoban. Ключ: {(golden ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");
            FinishGame(golden); // теперь через единую точку — дублей не будет
        }
#endif
    }
}