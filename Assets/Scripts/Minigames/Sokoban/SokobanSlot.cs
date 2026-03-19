using UnityEngine;

namespace Minigames.Sokoban
{
    public class SokobanSlot : MonoBehaviour
    {
        [Header("Визуал")]
        // Было: белый полупрозрачный — сливался со скелетом
        // Стало: жёлтый контурный — сразу видно "сюда нужно поставить"
        public Color emptyColor     = new Color(1f, 0.85f, 0f, 0.5f);   // жёлтый — "цель"
        public Color skeletonColor  = new Color(0.35f, 0.35f, 0.35f);   // тёмно-серый
        public Color mannequinColor = new Color(1f, 0.75f, 0.55f);      // оранжево-бежевый

        private SpriteRenderer _sr;
        private bool _isFilled = false;
        private bool _filledBySkeleton = false;

        public bool IsFilled         => _isFilled;
        public bool FilledBySkeleton => _filledBySkeleton;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _sr.color = emptyColor;
        }

        public void FillSlot(bool bySkeleton)
        {
            if (_isFilled) return;
            _isFilled = true;
            _filledBySkeleton = bySkeleton;
            if (_sr != null)
                _sr.color = bySkeleton ? skeletonColor : mannequinColor;
        }
    }
}