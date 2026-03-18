using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace Minigames.Sokoban
{
    /// <summary>
    /// Главный менеджер мини-игры "Каркас Истины".
    /// Обрабатывает ввод игрока, толкание объектов, проверку победы.
    /// </summary>
    public class SokobanManager : MonoBehaviour
    {
        [Header("Ссылки")]
        public SokobanGrid grid;
        public Transform   playerTransform;

        [Header("Всего слотов для заполнения")]
        [Tooltip("Должно совпадать с количеством SokobanSlot в сцене")]
        public int totalSlots = 3;

        [Header("UI")]
        public GameObject completionPanel;

        private bool _isComplete = false;

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

        void TryMove(Vector2Int dir)
        {
            Vector2Int playerCell  = grid.WorldToCell(playerTransform.position);
            Vector2Int targetCell  = playerCell + dir;

            // Ищем PushableObject в целевой клетке
            PushableObject pushable = GetPushableAt(targetCell);

            if (pushable != null)
            {
                // Попытка толкнуть
                bool pushed = pushable.TryPush(dir, grid);
                if (!pushed) return; // заблокировано — игрок тоже не двигается
            }
            else
            {
                // Просто стена?
                if (!grid.IsCellWalkable(targetCell, true)) return;
            }

            // Двигаем игрока
            playerTransform.position = grid.CellToWorld(targetCell);

            // Проверка победы
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

            // Победа — определяем ключ
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

        void ReturnToLabyrinth()
        {
            SceneManager.LoadScene("LabyrinthScene");
        }

        void Start()
        {
            // Выход по Escape
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("[Sokoban] Игрок вышел без прохождения.");
                SceneManager.LoadScene("LabyrinthScene");
            }
        }
    }
}