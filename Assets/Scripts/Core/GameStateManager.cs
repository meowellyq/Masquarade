using UnityEngine;

namespace Core
{
    public class GameStateManager : MonoBehaviour
    {
        // Singleton — единственный экземпляр на всю игру
        // Обращаться из любого скрипта: GameStateManager.Instance
        public static GameStateManager Instance { get; private set; }

        // ─── Три оси ───────────────────────────────────────────
        // Каждая ось от -100 до +100
        // Отрицательное значение = левый полюс
        // Положительное значение = правый полюс

        [Header("Ось 1: Сопротивление(-) / Принятие(+)")]
        [Range(-100, 100)]
        public float acceptance = 0f;

        [Header("Ось 2: Самообман(-) / Честность(+)")]
        [Range(-100, 100)]
        public float honesty = 0f;

        [Header("Ось 3: Зависимость(-) / Автономия(+)")]
        [Range(-100, 100)]
        public float autonomy = 0f;

        // ─── Ключи ─────────────────────────────────────────────
        [Header("Ключи")]
        public int goldenKeys = 0;
        public int silverKeys = 0;

        // ─── Инициализация ─────────────────────────────────────
        private void Awake()
        {
            // Если экземпляр уже существует — уничтожаем дубликат
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            // Запоминаем себя как единственный экземпляр
            Instance = this;

            // Не уничтожать при переходе между сценами
            DontDestroyOnLoad(gameObject);
        }

        // ─── Изменение осей ────────────────────────────────────
        // Вызывается из YarnCommands когда срабатывает <<shift_axis>>
        public void ShiftAxis(string axisName, float delta)
        {
            switch (axisName)
            {
                case "acceptance":
                    acceptance = Mathf.Clamp(acceptance + delta, -100f, 100f);
                    Debug.Log($"Принятие/Сопротивление: {acceptance}");
                    break;

                case "honesty":
                    honesty = Mathf.Clamp(honesty + delta, -100f, 100f);
                    Debug.Log($"Честность/Самообман: {honesty}");
                    break;

                case "autonomy":
                    autonomy = Mathf.Clamp(autonomy + delta, -100f, 100f);
                    Debug.Log($"Автономия/Зависимость: {autonomy}");
                    break;

                default:
                    Debug.LogWarning($"Неизвестная ось: {axisName}. " +
                                     $"Доступные оси: acceptance, honesty, autonomy");
                    break;
            }
        }

        // ─── Добавление ключей ─────────────────────────────────
        public void AddKey(bool isGolden)
        {
            if (isGolden)
            {
                goldenKeys++;
                Debug.Log($"Получен золотой ключ! Всего: {goldenKeys}");
            }
            else
            {
                silverKeys++;
                Debug.Log($"Получен серебряный ключ! Всего: {silverKeys}");
            }
        }

        // ─── Отладка — вывести все значения ────────────────────
        // Вызови эту функцию если хочешь увидеть состояние игры
        public void DebugPrintAllValues()
        {
            Debug.Log("=== СОСТОЯНИЕ ИГРЫ ===");
            Debug.Log($"Принятие/Сопротивление: {acceptance}");
            Debug.Log($"Честность/Самообман:    {honesty}");
            Debug.Log($"Автономия/Зависимость:  {autonomy}");
            Debug.Log($"Золотых ключей:  {goldenKeys}");
            Debug.Log($"Серебряных ключей: {silverKeys}");
            Debug.Log("=====================");
        }
    }
}
