using UnityEngine;

namespace Minigames.Sokoban
{
    /// <summary>
    /// Каркас-слот. Меняет цвет когда заполнен.
    /// Скелет → тёмный, Манекен → светлый.
    /// </summary>
    public class SokobanSlot : MonoBehaviour
    {
        [Header("Визуал")]
        public Color emptyColor    = new Color(1f, 1f, 1f, 0.3f); // полупрозрачный белый
        public Color skeletonColor = new Color(0.4f, 0.4f, 0.4f); // тёмно-серый
        public Color mannequinColor= new Color(1f, 0.95f, 0.8f);  // кремовый

        private SpriteRenderer _sr;
        private bool _isFilled = false;
        private bool _filledBySkeleton = false;

        public bool IsFilled => _isFilled;
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