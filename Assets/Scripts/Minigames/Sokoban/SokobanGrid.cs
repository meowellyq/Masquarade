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

        [Header("Стены (статичные объекты с тегом 'Wall')")]
        [Tooltip("Заполняется автоматически в Start, или назначь вручную")]
        public Transform[] walls;

        [Header("Слоты-каркасы")]
        public SokobanSlot[] slots;

        void Start()
        {
            // Автосбор стен по тегу если не назначены вручную
            if (walls == null || walls.Length == 0)
            {
                var wallObjects = GameObject.FindGameObjectsWithTag("Wall");
                walls = new Transform[wallObjects.Length];
                for (int i = 0; i < wallObjects.Length; i++)
                    walls[i] = wallObjects[i].transform;
            }

            // Автосбор слотов если не назначены вручную
            if (slots == null || slots.Length == 0)
                slots = FindObjectsOfType<SokobanSlot>();
        }

        public Vector2Int WorldToCell(Vector3 worldPos)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPos.x / cellSize),
                Mathf.RoundToInt(worldPos.y / cellSize)
            );
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(cell.x * cellSize, cell.y * cellSize, 0);
        }

        /// <summary>
        /// Проходима ли клетка?
        /// Скелет НЕ может войти на клетку со стеной.
        /// Манекен тоже не может (гнилой пол — это отдельный тег "RottenFloor", опционально).
        /// </summary>
        public bool IsCellWalkable(Vector2Int cell, bool isSkeleton)
        {
            Vector3 worldPos = CellToWorld(cell);

            foreach (var wall in walls)
            {
                if (wall == null) continue;
                Vector2Int wallCell = WorldToCell(wall.position);
                if (wallCell == cell) return false;
            }

            // Проверка других PushableObject (нельзя толкать два сразу)
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