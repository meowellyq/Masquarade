using UnityEngine;

namespace Minigames.PowderPuzzle
{
    /// <summary>
    /// Осколок зеркала (Drag & Drop + переворот ПКМ)
    /// </summary>
    public class PuzzlePiece : MonoBehaviour
    {
        [Header("Config")]
        public int pieceID = 0;
        public Transform correctSlot;
        public float snapDistance = 0.5f;

        [Header("Sides")]
        [Tooltip("Спрайт чистой стороны (можно оставить пустым — будет использован текущий)")]
        public Sprite cleanSide;
        [Tooltip("Спрайт стороны с наклейками (можно оставить пустым — будет использован текущий)")]
        public Sprite decoratedSide;

        private bool _isCleanSideUp = true;
        private bool _isPlaced = false;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _dragOffset;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            // Если спрайты не назначены — запомнить текущий как заглушку для обеих сторон
            if (cleanSide == null)    cleanSide    = _spriteRenderer.sprite;
            if (decoratedSide == null) decoratedSide = _spriteRenderer.sprite;
        }

        void Update()
        {
            if (!_isPlaced && IsMouseOver() && Input.GetMouseButtonDown(1))
                FlipPiece();
        }

        void FlipPiece()
        {
            _isCleanSideUp = !_isCleanSideUp;
            // Меняем цвет как визуальную подсказку если спрайты одинаковые
            _spriteRenderer.sprite = _isCleanSideUp ? cleanSide : decoratedSide;
            _spriteRenderer.color  = _isCleanSideUp
                ? Color.white
                : new Color(1f, 0.8f, 0.9f); // розоватый = "украшенная" сторона
            Debug.Log($"[PuzzlePiece] Осколок {pieceID} перевёрнут. " +
                      $"Сторона: {(_isCleanSideUp ? "ЧИСТАЯ" : "НАКЛЕЙКИ")}");
        }

        bool IsMouseOver()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var col = GetComponent<Collider2D>();
            return col != null && col.OverlapPoint(mousePos);
        }

        void OnMouseDown()
        {
            if (_isPlaced) return;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            _dragOffset = transform.position - mousePos;
        }

        void OnMouseDrag()
        {
            if (_isPlaced) return;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos + _dragOffset;
        }

        void OnMouseUp()
        {
            if (_isPlaced || correctSlot == null) return;

            if (Vector2.Distance(transform.position, correctSlot.position) <= snapDistance)
            {
                transform.position = correctSlot.position;
                _isPlaced = true;
                // Зафиксировать цвет при укладке
                _spriteRenderer.color = _isCleanSideUp
                    ? new Color(0.8f, 1f, 0.8f)  // зеленоватый = встал на место чистой стороной
                    : new Color(1f, 0.7f, 0.85f); // розовый = встал украшенной
                FindObjectOfType<PuzzleManager_Extravagance>()?.OnPiecePlaced(_isCleanSideUp);
                Debug.Log($"[PuzzlePiece] Осколок {pieceID} установлен в слот.");
            }
        }
    }
}