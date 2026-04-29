using UnityEngine;

namespace Labyrinth
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 15f;

        private Rigidbody2D _rb;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _moveInput;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0;
            _rb.freezeRotation = true;
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            _moveInput.x = Input.GetAxisRaw("Horizontal");
            _moveInput.y = Input.GetAxisRaw("Vertical");
            _moveInput.Normalize();

            bool isMoving = _moveInput != Vector2.zero;
            _animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                _animator.SetFloat("MoveX", _moveInput.x);
                _animator.SetFloat("MoveY", _moveInput.y);

                // Флип влево для Walk_Side
                if (Mathf.Abs(_moveInput.x) > 0.1f)
                    _spriteRenderer.flipX = _moveInput.x < 0;
            }
        }

        void FixedUpdate()
        {
            _rb.velocity = _moveInput * moveSpeed;
        }
    }
}