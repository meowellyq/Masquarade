using UnityEngine;
using TMPro;
using Core;

namespace UI
{
    /// <summary>
    /// Отображает количество собранных ключей в лабиринте
    /// </summary>
    public class KeysUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI keysText;

        void Update()
        {
            if (GameStateManager.Instance == null || keysText == null) return;

            int totalKeys = 0;
            if (GameStateManager.Instance.hasExtravaganceKey) totalKeys++;
            if (GameStateManager.Instance.hasInadequacyKey)   totalKeys++;

            keysText.text = $"Ключи: {totalKeys}/2";
        }
    }
}