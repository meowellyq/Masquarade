using UnityEngine;

namespace Labyrinth
{
    /// <summary>
    /// Простой контроллер перемещения для лабиринта (top-down)
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Скорость движения")]
        public float moveSpeed = 15f;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0;
            _rb.freezeRotation = true;
        }

        void Update()
        {
            _moveInput.x = Input.GetAxisRaw("Horizontal");
            _moveInput.y = Input.GetAxisRaw("Vertical");
            _moveInput.Normalize();
        }

        void FixedUpdate()
        {
            _rb.velocity = _moveInput * moveSpeed;
        }
    }
}