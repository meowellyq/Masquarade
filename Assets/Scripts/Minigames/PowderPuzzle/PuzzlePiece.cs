using UnityEngine;

namespace Minigames.PowderPuzzle
{
    /// <summary>
    /// Осколок зеркала: drag & drop + переворот ПКМ + настраиваемая стартовая сторона.
    /// </summary>
    public class PuzzlePiece : MonoBehaviour
    {
        public enum InitialSideMode
        {
            Clean,
            Decorated,
            Random
        }

        [Header("Config")]
        public int pieceID = 0;
        public Transform correctSlot;
        public float snapDistance = 0.5f;

        [Header("Initial State")]
        [Tooltip("Какая сторона будет видна при старте сцены")]
        public InitialSideMode initialSide = InitialSideMode.Random;

        [Header("Sides")]
        [Tooltip("Правдивая / чистая сторона")]
        public Sprite cleanSide;

        [Tooltip("Ложная / украшенная сторона")]
        public Sprite decoratedSide;

        private bool _isCleanSideUp = true;
        private bool _isPlaced = false;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _dragOffset;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (cleanSide == null)
                cleanSide = _spriteRenderer.sprite;

            if (decoratedSide == null)
                decoratedSide = _spriteRenderer.sprite;

            ApplyInitialSide();
        }

        void ApplyInitialSide()
        {
            switch (initialSide)
            {
                case InitialSideMode.Clean:
                    SetSide(true);
                    break;

                case InitialSideMode.Decorated:
                    SetSide(false);
                    break;

                case InitialSideMode.Random:
                    SetSide(Random.value >= 0.5f);
                    break;
            }
        }

        void Update()
        {
            if (!_isPlaced && IsMouseOver() && Input.GetMouseButtonDown(1))
                FlipPiece();
        }

        void FlipPiece()
        {
            SetSide(!_isCleanSideUp);

            Debug.Log($"[PuzzlePiece] Осколок {pieceID} перевёрнут. " +
                      $"Сторона: {(_isCleanSideUp ? "ПРАВДА" : "ЛОЖЬ")}");
        }

        void SetSide(bool cleanSideUp)
        {
            _isCleanSideUp = cleanSideUp;

            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            _spriteRenderer.sprite = _isCleanSideUp ? cleanSide : decoratedSide;

            // Важно: не красим арт розовым или зелёным, чтобы не портить изображение.
            _spriteRenderer.color = Color.white;
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

                // Оставляем реальный арт без цветной заливки.
                _spriteRenderer.color = Color.white;

                FindObjectOfType<PuzzleManager_Extravagance>()?.OnPiecePlaced(_isCleanSideUp);

                Debug.Log($"[PuzzlePiece] Осколок {pieceID} установлен в слот. " +
                          $"Сторона: {(_isCleanSideUp ? "ПРАВДА" : "ЛОЖЬ")}");
            }
        }
    }
}