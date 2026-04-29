using UnityEngine;

namespace Minigames.Sokoban
{
    /// <summary>
    /// Отвечает за перевод координат клетки ↔ мир,
    /// и за проверку проходимости клеток.
    /// </summary>
    public class SokobanGrid : MonoBehaviour
    {
        [Header("Размер сетки")]
        public float cellSize = 1f;

        [Header("Смещение сетки (подбери по позициям стен)")]
        public Vector2 gridOffset = new Vector2(0.42f, -0.42f);

        [Header("Стены (статичные объекты с тегом 'Wall')")]
        [Tooltip("Заполняется автоматически в Start, или назначь вручную")]
        public Transform[] walls;

        [Header("Слоты-каркасы")]
        public SokobanSlot[] slots;

        void Start()
        {
            if (walls == null || walls.Length == 0)
            {
                var wallObjects = GameObject.FindGameObjectsWithTag("Wall");
                walls = new Transform[wallObjects.Length];
                for (int i = 0; i < wallObjects.Length; i++)
                    walls[i] = wallObjects[i].transform;
            }

            if (slots == null || slots.Length == 0)
                slots = FindObjectsOfType<SokobanSlot>();

            // Снап игрока
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector2Int cell = WorldToCell(player.transform.position);
                player.transform.position = CellToWorld(cell);
            }

            // Снап ящиков
            var allPushables = FindObjectsOfType<PushableObject>();
            foreach (var obj in allPushables)
            {
                Vector2Int cell = WorldToCell(obj.transform.position);
                obj.transform.position = CellToWorld(cell);
            }

            // Снап слотов
            foreach (var slot in slots)
            {
                if (slot == null) continue;
                Vector2Int cell = WorldToCell(slot.transform.position);
                slot.transform.position = CellToWorld(cell);
            }
        }

        public Vector2Int WorldToCell(Vector3 worldPos)
        {
            return new Vector2Int(
                Mathf.RoundToInt((worldPos.x - gridOffset.x) / cellSize),
                Mathf.RoundToInt((worldPos.y - gridOffset.y) / cellSize)
            );
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(
                cell.x * cellSize + gridOffset.x,
                cell.y * cellSize + gridOffset.y,
                0
            );
        }

        public bool IsCellWalkable(Vector2Int cell, bool isSkeleton)
        {
            foreach (var wall in walls)
            {
                if (wall == null) continue;
                Vector2Int wallCell = WorldToCell(wall.position);
                if (wallCell == cell) return false;
            }

            var allObjects = FindObjectsOfType<PushableObject>();
            foreach (var obj in allObjects)
            {
                if (obj.IsInSlot) continue;
                Vector2Int objCell = WorldToCell(obj.transform.position);
                if (objCell == cell) return false;
            }

            return true;
        }

        public SokobanSlot GetSlotAt(Vector2Int cell)
        {
            foreach (var slot in slots)
            {
                if (slot == null) continue;
                if (WorldToCell(slot.transform.position) == cell)
                    return slot;
            }
            return null;
        }
    }
}