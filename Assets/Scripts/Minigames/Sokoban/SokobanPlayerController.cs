using System.Collections;
using UnityEngine;

namespace Minigames.Sokoban
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SokobanPlayerController : MonoBehaviour
    {
        [Tooltip("Сколько секунд играет анимация после шага (подбери под скорость игры)")]
        public float moveAnimDuration = 0.5f;

        private Animator _animator;
        private SpriteRenderer _sr;
        private Coroutine _idleCoroutine;
        


        void Awake()
        {
            _animator = GetComponent<Animator>();
            _sr = GetComponent<SpriteRenderer>();
        }

        public void OnMove(Vector2Int dir, bool isPushing, bool isPushingStrong)
        {
            // Сначала сбрасываем оси чтобы переходы сработали чисто
            _animator.SetFloat("MoveX", 0f);
            _animator.SetFloat("MoveY", 0f);

            // Устанавливаем новое направление
            _animator.SetFloat("MoveX", dir.x);
            _animator.SetFloat("MoveY", dir.y);
            _animator.SetBool("IsMoving", true);
            _animator.SetBool("IsPushing", isPushing);
            _animator.SetBool("IsPushingStrong", isPushingStrong);

            // Флип влево
            if (Mathf.Abs(dir.x) > 0)
                _sr.flipX = dir.x < 0;

            // Перезапускаем таймер возврата в Idle
            if (_idleCoroutine != null) StopCoroutine(_idleCoroutine);
            _idleCoroutine = StartCoroutine(ReturnToIdleAfter(moveAnimDuration));
        }

        public void OnIdle()
        {
            if (_idleCoroutine != null) StopCoroutine(_idleCoroutine);
            SetIdle();
        }

        private IEnumerator ReturnToIdleAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetIdle();
        }

        private void SetIdle()
        {
            _animator.SetBool("IsMoving", false);
            _animator.SetBool("IsPushing", false);
            _animator.SetBool("IsPushingStrong", false);
        }
    }
}