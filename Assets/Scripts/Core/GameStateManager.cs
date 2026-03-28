using UnityEngine;
using Yarn.Unity;

namespace Core
{
    public class GameStateManager : MonoBehaviour
    {
        // ─── Singleton ─────────────────────────────────────────
        public static GameStateManager Instance { get; private set; }

        // ─── Три оси (единая система с Balance_Table.md) ──────
        // Каждая ось от -100 до +100

        [Header("Control: Зависимость(-100) / Автономия(+100)")]
        [Range(-100, 100)]
        public float control = 0f;

        [Header("World: Принятие(-100) / Сопротивление(+100)")]
        [Range(-100, 100)]
        public float world = 0f;

        [Header("Truth: Самообман(-100) / Честность(+100)")]
        [Range(-100, 100)]
        public float truth = 0f;

        // ─── Ключи ─────────────────────────────────────────────
        [Header("Ключи")]
        public int goldenKeys = 0;
        public int silverKeys = 0;

        // ─── Прогресс мини-игр лабиринта ───────────────────────
        [Header("Labyrinth Progress")]
        public bool hasExtravaganceKey = false; // Пройдена ли мини-игра Экстравагантности
        public bool hasInadequacyKey   = false; // Пройдена ли мини-игра Неполноценности

        // ─── Навигация ─────────────────────────────────────────
        [Header("Navigation")]
        public string currentYarnNode = ""; // Точка возврата из лабиринта / мини-игр
        public bool labyrinthVisualEffectApplied = false; // Умылась ли в Фонтане

        // ─── Флаги состояния ────────────────────────────────────
        [Header("Состояние")]
        public bool memoryLoss = false; // Потеря памяти (для диалогов)
        public bool fountainDone = false;
        public bool pondVisited = false; 
        public bool hallOfSorrowEntered = false;
        

        // ─── Инициализация ─────────────────────────────────────
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ─── Изменение осей ────────────────────────────────────
        // Вызывается из Yarn: <<shift_axis control 10>>
        public void ShiftAxis(string axisName, float delta)
        {
            switch (axisName)
            {
                case "control":
                    control = Mathf.Clamp(control + delta, -100f, 100f);
                    Debug.Log($"[Ось] Control (Завис./Автон.): {control}");
                    break;

                case "world":
                    world = Mathf.Clamp(world + delta, -100f, 100f);
                    Debug.Log($"[Ось] World (Принят./Сопрот.): {world}");
                    break;

                case "truth":
                    truth = Mathf.Clamp(truth + delta, -100f, 100f);
                    Debug.Log($"[Ось] Truth (Самооб./Честн.): {truth}");
                    break;

                default:
                    Debug.LogWarning($"Неизвестная ось: {axisName}. " +
                                     "Доступные: control, world, truth");
                    break;
            }

            UpdateMemoryLoss();
        }

        // ─── Синхронизация осей из Yarn ────────────────────────
        // Вызывается при возврате из лабиринта или смене сцены
        public void SyncAxesFromYarn()
        {
            var storage = FindObjectOfType<InMemoryVariableStorage>();
            if (storage == null)
            {
                Debug.LogWarning("[Sync] InMemoryVariableStorage не найден!");
                return;
            }

            if (storage.TryGetValue("$control", out float c)) control = c;
            if (storage.TryGetValue("$world",   out float w)) world   = w;
            if (storage.TryGetValue("$truth",   out float t)) truth   = t;

            UpdateMemoryLoss();
            Debug.Log($"[Sync] Оси синхронизированы: C={control} W={world} T={truth}");
        }

        // ─── Добавление ключей ─────────────────────────────────
        // Вызывается из Yarn: <<add_key golden>> или <<add_key silver>>
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

        // ─── Завершение мини-игры и получение ключа ────────────
        /// <summary>
        /// Вызывается по итогам мини-игры лабиринта.
        /// miniGameName: "extravagance" | "inadequacy"
        /// </summary>
        public void CompleteMiniGame(string miniGameName, bool isGoldenKey)
        {
            switch (miniGameName)
            {
                case "extravagance":
                    hasExtravaganceKey = true;
                    if (isGoldenKey) goldenKeys++; else silverKeys++;
                    Debug.Log($"[MiniGame] Экстравагантность пройдена. " +
                              $"Ключ: {(isGoldenKey ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");
                    break;

                case "inadequacy":
                    hasInadequacyKey = true;
                    if (isGoldenKey) goldenKeys++; else silverKeys++;
                    Debug.Log($"[MiniGame] Неполноценность пройдена. " +
                              $"Ключ: {(isGoldenKey ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");
                    break;

                default:
                    Debug.LogWarning($"[MiniGame] Неизвестная мини-игра: {miniGameName}. " +
                                     "Доступные: extravagance, inadequacy");
                    break;
            }
        }

        // ─── Проверка завершения обеих мини-игр ────────────────
        /// <summary>
        /// Обе мини-игры пройдены → можно открыть финальную дверь лабиринта.
        /// </summary>
        public bool BothMiniGamesCompleted()
        {
            return hasExtravaganceKey && hasInadequacyKey;
        }

        // ─── Проверка обоих ключей (Сцена лабиринта) ───────────
        public bool BothKeysCollected()
        {
            return goldenKeys >= 1 && silverKeys >= 1;
        }

        // ─── Тип ключей для Сцены 9 ────────────────────────────
        // Возвращает: "golden", "silver", "mixed"
        public string GetKeyType()
        {
            if (goldenKeys >= 2) return "golden";
            if (silverKeys >= 2) return "silver";
            return "mixed";
        }

        // ─── Определение финала ────────────────────────────────
        // Возвращает номер финала (1-9)
        public int DetermineEnding()
        {
            // Приоритет 1: Истинный выход (Rebel)
            if (truth >= 60 && control >= 30 && world >= 20)
                return 2;

            // Приоритет 2: Марионетка (Doll)
            if (truth <= -60 && control <= -40 && world <= 0)
                return 7;

            // Приоритет 3: РАСПАД
            if (Mathf.Abs(control) <= 10 && Mathf.Abs(world) <= 10 && Mathf.Abs(truth) <= 10)
                return 9;

            bool isAutonomy   = control > 0;
            bool isResistance = world > 0;
            bool isHonesty    = truth > 0;

            if (isHonesty && isAutonomy && !isResistance)   return 1; // Смиренный странник
            if (isHonesty && isAutonomy && isResistance)    return 2; // Истинный (soft)
            if (isHonesty && !isAutonomy && !isResistance)  return 3; // Мне нужна опора
            if (isHonesty && !isAutonomy && isResistance)   return 4; // Болезненная правда
            if (!isHonesty && isAutonomy && !isResistance)  return 5; // Красивая роль
            if (!isHonesty && isAutonomy && isResistance)   return 6; // Побег
            if (!isHonesty && !isAutonomy && !isResistance) return 7; // Марионетка (soft)
            if (!isHonesty && !isAutonomy && isResistance)  return 8; // Безумное слияние

            return 9; // Fallback — РАСПАД
        }

        // ─── Перепутье: найти самую неопределённую ось ──────────
        // Возвращает "control", "world" или "truth"
        // Используется в Сценах 8.5 и 18.5
        public string FindClosestAxis()
        {
            float absC = Mathf.Abs(control);
            float absW = Mathf.Abs(world);
            float absT = Mathf.Abs(truth);

            if (absC <= absW && absC <= absT) return "control";
            if (absW <= absC && absW <= absT) return "world";
            return "truth";
        }

        // ─── Определение архетипа Проводника ────────────────────
        // Используется в Сцене 13
        // Возвращает: "iconoclast", "doll", "victim", "wanderer"
        public string DetermineGuideArchetype()
        {
            bool isAutonomy   = control > 0;
            bool isResistance = world > 0;
            bool isHonesty    = truth > 0;

            if (isHonesty && isAutonomy && isResistance)    return "iconoclast";
            if (!isHonesty && !isAutonomy && !isResistance) return "doll";
            if (!isHonesty && !isAutonomy && isResistance)  return "victim";
            if (isHonesty && isAutonomy && !isResistance)   return "wanderer";

            if (isAutonomy && isResistance)   return "iconoclast";
            if (!isAutonomy && !isResistance) return "doll";
            if (!isAutonomy && isResistance)  return "victim";
            return "wanderer";
        }

        // ─── Определение варианта Слома (Сцена 14) ──────────────
        // Возвращает: 1-5
        public int DetermineBreakdownType()
        {
            bool isAutonomy = control > 0;
            bool isHonesty  = truth > 0;

            if (Mathf.Abs(control) <= 15 && Mathf.Abs(truth) <= 15)
                return 5; // РАСПАД — хаос интерфейса

            if (isHonesty && isAutonomy)   return 1;
            if (isHonesty && !isAutonomy)  return 2;
            if (!isHonesty && isAutonomy)  return 3;
            if (!isHonesty && !isAutonomy) return 4;

            return 5;
        }

        // ─── Определение содержимого Флакона (Сцена 17) ─────────
        // Возвращает: "black", "pink", "grey"
        public string DetermineFlaskContent()
        {
            bool isAutonomy   = control > 0;
            bool isResistance = world > 0;
            bool isHonesty    = truth > 0;

            if (isHonesty && isAutonomy && isResistance)    return "black";
            if (!isHonesty && !isAutonomy && !isResistance) return "pink";
            if (!isHonesty && isAutonomy && isResistance)   return "grey";

            if (isHonesty) return "black";
            if (Mathf.Abs(truth) <= 10) return "grey";
            return "pink";
        }

        // ─── Потеря памяти ─────────────────────────────────────
        private void UpdateMemoryLoss()
        {
            memoryLoss = (control <= -30 && world <= -30 && truth <= -30);
        }

        // ─── Отладка ───────────────────────────────────────────
        public void DebugPrintAllValues()
        {
            Debug.Log("╔══════════════════════════════════╗");
            Debug.Log("║      СОСТОЯНИЕ ИГРЫ              ║");
            Debug.Log("╠══════════════════════════════════╣");
            Debug.Log($"║ Control (Завис./Автон.):  {control,6} ║");
            Debug.Log($"║ World   (Принят./Сопр.):  {world,6} ║");
            Debug.Log($"║ Truth   (Самооб./Честн.): {truth,6} ║");
            Debug.Log("╠══════════════════════════════════╣");
            Debug.Log($"║ Золотых ключей:    {goldenKeys,5}      ║");
            Debug.Log($"║ Серебряных ключей: {silverKeys,5}      ║");
            Debug.Log($"║ Потеря памяти:     {memoryLoss,5}      ║");
            Debug.Log($"║ Оба ключа:         {BothKeysCollected(),5}      ║");
            Debug.Log("╠══════════════════════════════════╣");
            Debug.Log($"║ Экстравагантность: {hasExtravaganceKey,5}      ║");
            Debug.Log($"║ Неполноценность:   {hasInadequacyKey,5}      ║");
            Debug.Log($"║ Обе мини-игры:     {BothMiniGamesCompleted(),5}      ║");
            Debug.Log("╠══════════════════════════════════╣");
            Debug.Log($"║ Текущий узел:       {currentYarnNode,-12} ║");
            Debug.Log($"║ Ближайшая ось к 0:  {FindClosestAxis(),-12} ║");
            Debug.Log($"║ Архетип Проводника: {DetermineGuideArchetype(),-12} ║");
            Debug.Log($"║ Тип Слома:          {DetermineBreakdownType(),-12} ║");
            Debug.Log($"║ Финал:              {DetermineEnding(),-12} ║");
            Debug.Log("╚══════════════════════════════════╝");
        }
    }
}
