using UnityEngine;

namespace Minigames.Sokoban
{
    /// <summary>
    /// Объект, который игрок толкает (Скелет или Манекен).
    /// Скелет (isSkeleton=true)  — тяжёлый, ломает гнилой пол → Золотой ключ
    /// Манекен (isSkeleton=false) — лёгкий, пролетает сквозь пол → Серебряный ключ
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PushableObject : MonoBehaviour
    {
        [Header("Тип объекта")]
        [Tooltip("true = Скелет (тяжёлый), false = Манекен (лёгкий)")]
        public bool isSkeleton = true;

        [Header("Визуал")]
        [Tooltip("Цвет заглушки: серый = Скелет, бежевый = Манекен")]
        public Color skeletonColor  = new Color(0.7f, 0.7f, 0.7f); // серый
        public Color mannequinColor = new Color(1.0f, 0.9f, 0.7f); // бежевый

        private Rigidbody2D _rb;
        private bool _isInSlot = false;

        public bool IsInSlot => _isInSlot;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale   = 0;
            _rb.freezeRotation = true;
            _rb.isKinematic    = true; // двигаем сами, не физикой

            // Установить цвет-заглушку
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = isSkeleton ? skeletonColor : mannequinColor;
        }

        /// <summary>
        /// Вызывается из SokobanManager когда игрок толкает этот объект.
        /// Возвращает true если объект сдвинулся (не заблокирован).
        /// </summary>
        public bool TryPush(Vector2Int direction, SokobanGrid grid)
        {
            if (_isInSlot) return false; // уже стоит в каркасе — не толкать

            Vector2Int currentCell = grid.WorldToCell(transform.position);
            Vector2Int targetCell  = currentCell + direction;

            if (!grid.IsCellWalkable(targetCell, isSkeleton))
            {
                Debug.Log($"[PushableObject] {name} заблокирован в {targetCell}.");
                return false;
            }

            transform.position = grid.CellToWorld(targetCell);
            Debug.Log($"[PushableObject] {name} сдвинут в {targetCell}.");

            // Проверить — попал ли в слот-каркас
            SokobanSlot slot = grid.GetSlotAt(targetCell);
            if (slot != null)
            {
                _isInSlot = true;
                slot.FillSlot(isSkeleton);
                Debug.Log($"[PushableObject] {name} встал в каркас {slot.name}!");
            }

            return true;
        }

        public void SetInSlot(bool value) => _isInSlot = value;
    }
}