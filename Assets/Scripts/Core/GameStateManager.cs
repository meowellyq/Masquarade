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
        public bool hasExtravaganceKey = false;
        public bool hasInadequacyKey   = false;

        // ─── Навигация ─────────────────────────────────────────
        [Header("Navigation")]
        public string currentYarnNode = "";
        public bool labyrinthVisualEffectApplied = false;
        public string spawnPointId = "default"; 
        public Vector3 labyrinthReturnPosition = Vector3.zero;

        // ─── Флаги состояния ────────────────────────────────────
        [Header("Состояние")]
        public bool memoryLoss = false;
        public bool fountainDone = false;
        public bool pondVisited = false;
        public bool hallOfSorrowEntered = false;
        public int scene10Choice = 0;
        public bool wrathEchoDone = false;
        public string flask = "";  // ← НОВАЯ СТРОКА: "", "empty", "black", "pink", "grey"

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
                    Debug.LogWarning($"Неизвестная ось: {axisName}. Доступные: control, world, truth");
                    break;
            }
            UpdateMemoryLoss();
        }

        // ─── Синхронизация осей из Yarn ────────────────────────
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
        public void AddKey(bool isGolden)
        {
            if (isGolden) { goldenKeys++; Debug.Log($"Получен золотой ключ! Всего: {goldenKeys}"); }
            else          { silverKeys++; Debug.Log($"Получен серебряный ключ! Всего: {silverKeys}"); }
        }

        // ─── Завершение мини-игры ───────────────────────────────
        public void CompleteMiniGame(string miniGameName, bool isGoldenKey)
        {
            switch (miniGameName)
            {
                case "extravagance":
                    hasExtravaganceKey = true;
                    if (isGoldenKey) goldenKeys++; else silverKeys++;
                    Debug.Log($"[MiniGame] Экстравагантность пройдена. Ключ: {(isGoldenKey ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");
                    break;
                case "inadequacy":
                    hasInadequacyKey = true;
                    if (isGoldenKey) goldenKeys++; else silverKeys++;
                    Debug.Log($"[MiniGame] Неполноценность пройдена. Ключ: {(isGoldenKey ? "ЗОЛОТОЙ" : "СЕРЕБРЯНЫЙ")}");
                    break;
                default:
                    Debug.LogWarning($"[MiniGame] Неизвестная мини-игра: {miniGameName}.");
                    break;
            }
        }

        public bool BothMiniGamesCompleted() => hasExtravaganceKey && hasInadequacyKey;
        public bool BothKeysCollected()      => goldenKeys >= 1 && silverKeys >= 1;

        public string GetKeyType()
        {
            if (goldenKeys >= 2) return "golden";
            if (silverKeys >= 2) return "silver";
            return "mixed";
        }

        public int DetermineEnding()
        {
            if (truth >= 60 && control >= 30 && world >= 20) return 2;
            if (truth <= -60 && control <= -40 && world <= 0) return 7;
            if (Mathf.Abs(control) <= 10 && Mathf.Abs(world) <= 10 && Mathf.Abs(truth) <= 10) return 9;

            bool isAutonomy   = control > 0;
            bool isResistance = world > 0;
            bool isHonesty    = truth > 0;

            if (isHonesty && isAutonomy && !isResistance)   return 1;
            if (isHonesty && isAutonomy && isResistance)    return 2;
            if (isHonesty && !isAutonomy && !isResistance)  return 3;
            if (isHonesty && !isAutonomy && isResistance)   return 4;
            if (!isHonesty && isAutonomy && !isResistance)  return 5;
            if (!isHonesty && isAutonomy && isResistance)   return 6;
            if (!isHonesty && !isAutonomy && !isResistance) return 7;
            if (!isHonesty && !isAutonomy && isResistance)  return 8;
            return 9;
        }

        public string FindClosestAxis()
        {
            float absC = Mathf.Abs(control);
            float absW = Mathf.Abs(world);
            float absT = Mathf.Abs(truth);
            if (absC <= absW && absC <= absT) return "control";
            if (absW <= absC && absW <= absT) return "world";
            return "truth";
        }

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

        public int DetermineBreakdownType()
        {
            bool isAutonomy = control > 0;
            bool isHonesty  = truth > 0;
            if (Mathf.Abs(control) <= 15 && Mathf.Abs(truth) <= 15) return 5;
            if (isHonesty && isAutonomy)   return 1;
            if (isHonesty && !isAutonomy)  return 2;
            if (!isHonesty && isAutonomy)  return 3;
            if (!isHonesty && !isAutonomy) return 4;
            return 5;
        }

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

        private void UpdateMemoryLoss()
        {
            memoryLoss = (control <= -30 && world <= -30 && truth <= -30);
        }

        public void DebugPrintAllValues()
        {
            Debug.Log("╔══════════════════════════════════╗");
            Debug.Log("║      СОСТОЯНИЕ ИГРЫ              ║");
            Debug.Log("╠══════════════════════════════════╣");
            Debug.Log($"║ Control:      {control,6}            ║");
            Debug.Log($"║ World:        {world,6}            ║");
            Debug.Log($"║ Truth:        {truth,6}            ║");
            Debug.Log($"║ SpawnPoint:   {spawnPointId,-12}      ║");
            Debug.Log($"║ Финал:        {DetermineEnding(),-12}      ║");
            Debug.Log("╚══════════════════════════════════╝");
        }
    }
}