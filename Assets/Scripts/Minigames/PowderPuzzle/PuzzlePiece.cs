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
        public Sprite cleanSide;
        public Sprite decoratedSide;

        private bool _isCleanSideUp = true;
        private bool _isPlaced = false;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _dragOffset;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (!_isPlaced && IsMouseOver() && Input.GetMouseButtonDown(1))
                FlipPiece();
        }

        void FlipPiece()
        {
            _isCleanSideUp = !_isCleanSideUp;
            _spriteRenderer.sprite = _isCleanSideUp ? cleanSide : decoratedSide;
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
                FindObjectOfType<PuzzleManager_Extravagance>()?.OnPiecePlaced(_isCleanSideUp);
                Debug.Log($"[PuzzlePiece] Осколок {pieceID} установлен в слот.");
            }
        }
    }
}
