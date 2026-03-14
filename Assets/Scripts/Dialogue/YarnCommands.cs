using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using Core;

public class YarnCommands : MonoBehaviour
{
    // ─── Вспомогательный метод записи в Yarn Storage ───────
    // Используется всеми командами, которым нужно вернуть
    // значение обратно в Yarn-скрипт через переменную.
    private static InMemoryVariableStorage GetStorage()
    {
        var storage = FindObjectOfType<InMemoryVariableStorage>();
        if (storage == null)
            Debug.LogWarning("[YarnCommands] InMemoryVariableStorage не найден!");
        return storage;
    }

    // ─── Сдвиг оси ─────────────────────────────────────────
    // Yarn: <<shift_axis control 10>>
    // Yarn: <<shift_axis world -15>>
    // Yarn: <<shift_axis truth 20>>
    [YarnCommand("shift_axis")]
    public static void ShiftAxis(string axisName, float value)
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager не найден на сцене!");
            return;
        }
        GameStateManager.Instance.ShiftAxis(axisName, value);

        // Синхронизируем изменённую ось обратно в Yarn Storage
        var storage = GetStorage();
        if (storage != null)
        {
            storage.SetValue("$control", GameStateManager.Instance.control);
            storage.SetValue("$world",   GameStateManager.Instance.world);
            storage.SetValue("$truth",   GameStateManager.Instance.truth);
        }
    }

    // ─── Добавить ключ ─────────────────────────────────────
    // Yarn: <<add_key golden>>
    // Yarn: <<add_key silver>>
    [YarnCommand("add_key")]
    public static void AddKey(string keyType)
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager не найден на сцене!");
            return;
        }
        bool isGolden = keyType == "golden";
        GameStateManager.Instance.AddKey(isGolden);

        // Обновляем ключи и флаг BothKeysCollected в Storage
        var storage = GetStorage();
        if (storage != null)
        {
            storage.SetValue("$golden_keys",        GameStateManager.Instance.goldenKeys);
            storage.SetValue("$silver_keys",        GameStateManager.Instance.silverKeys);
            storage.SetValue("$both_keys_collected", GameStateManager.Instance.BothKeysCollected());
        }
    }

    // ─── Перепутье: определить самую неопределённую ось ──────
    // Yarn: <<check_closest_axis>>
    // После вызова: <<if $closest_axis == "control">>
    [YarnCommand("check_closest_axis")]
    public static void CheckClosestAxis()
    {
        if (GameStateManager.Instance == null) return;
        string axis = GameStateManager.Instance.FindClosestAxis();
        Debug.Log($"[Перепутье] Самая неопределённая ось: {axis}");

        var storage = GetStorage();
        storage?.SetValue("$closest_axis", axis);
    }

    // ─── Определить финал ──────────────────────────────────
    // Yarn: <<determine_ending>>
    // После вызова: <<if $ending == 1>>
    [YarnCommand("determine_ending")]
    public static void DetermineEnding()
    {
        if (GameStateManager.Instance == null) return;
        int ending = GameStateManager.Instance.DetermineEnding();
        Debug.Log($"[Финал] Определён финал: {ending}");

        var storage = GetStorage();
        storage?.SetValue("$ending", ending);
    }

    // ─── Определить архетип Проводника ──────────────────────
    // Yarn: <<determine_guide>>
    // После вызова: <<if $guide_type == "iconoclast">>
    [YarnCommand("determine_guide")]
    public static void DetermineGuide()
    {
        if (GameStateManager.Instance == null) return;
        string archetype = GameStateManager.Instance.DetermineGuideArchetype();
        Debug.Log($"[Проводник] Архетип: {archetype}");

        var storage = GetStorage();
        storage?.SetValue("$guide_type", archetype);
    }

    // ─── Определить тип Слома ──────────────────────────────
    // Yarn: <<determine_breakdown>>
    // После вызова: <<if $breakdown_type == 1>>
    [YarnCommand("determine_breakdown")]
    public static void DetermineBreakdown()
    {
        if (GameStateManager.Instance == null) return;
        int type = GameStateManager.Instance.DetermineBreakdownType();
        Debug.Log($"[Слом] Тип слома: {type}");

        var storage = GetStorage();
        storage?.SetValue("$breakdown_type", type);
    }

    // ─── Определить содержимое Флакона ──────────────────────
    // Yarn: <<determine_flask>>
    // После вызова: <<if $flask == "black">>
    [YarnCommand("determine_flask")]
    public static void DetermineFlask()
    {
        if (GameStateManager.Instance == null) return;
        string flask = GameStateManager.Instance.DetermineFlaskContent();
        Debug.Log($"[Флакон] Содержимое: {flask}");

        var storage = GetStorage();
        storage?.SetValue("$flask", flask);
    }

    // ─── Проверить потерю памяти ────────────────────────────
    // Yarn: <<check_memory>>
    // После вызова: <<if $memory_loss == true>>
    [YarnCommand("check_memory")]
    public static void CheckMemory()
    {
        if (GameStateManager.Instance == null) return;
        bool loss = GameStateManager.Instance.memoryLoss;
        Debug.Log($"[Память] Потеря памяти: {loss}");

        var storage = GetStorage();
        storage?.SetValue("$memory_loss", loss);
    }

    // ─── Проверить тип ключей ───────────────────────────────
    // Yarn: <<check_keys>>
    // После вызова: <<if $key_type == "golden">>
    [YarnCommand("check_keys")]
    public static void CheckKeys()
    {
        if (GameStateManager.Instance == null) return;
        string keyType = GameStateManager.Instance.GetKeyType();
        Debug.Log($"[Ключи] Тип: {keyType}");

        var storage = GetStorage();
        storage?.SetValue("$key_type", keyType);
    }

    // ─── Загрузка Unity-сцены ──────────────────────────────
    // Yarn: <<load_scene LabyrinthScene>>
    [YarnCommand("load_scene")]
    public static void LoadScene(string sceneName)
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SyncAxesFromYarn();

        Debug.Log($"[Сцена] Загрузка: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // ─── Установить точку возврата в Yarn ──────────────────
    // Yarn: <<set_yarn_node Scene09_ReturnToFountain>>
    [YarnCommand("set_yarn_node")]
    public static void SetYarnNode(string nodeName)
    {
        if (GameStateManager.Instance == null) return;
        GameStateManager.Instance.currentYarnNode = nodeName;
        Debug.Log($"[Навигация] Точка возврата: {nodeName}");
    }

    // ─── Проводник перехватывает управление ──────────────────
    // Yarn: <<guide_speaks>>
    [YarnCommand("guide_speaks")]
    public static void GuideSpeaks()
    {
        Debug.Log("[Проводник] Перехватывает управление!");
        // TODO Этап 2: визуальный эффект перехвата голоса
    }

    // ─── Глитч-эффект ──────────────────────────────────────
    // Yarn: <<trigger_glitch low>>
    // Yarn: <<trigger_glitch medium>>
    // Yarn: <<trigger_glitch high>>
    [YarnCommand("trigger_glitch")]
    public static void TriggerGlitch(string intensity)
    {
        Debug.Log($"[Глитч] Уровень: {intensity}");
        // TODO Этап 7: пост-процессинг глитч-шейдер
    }

    // ─── Визуальный эффект (общий) ─────────────────────────
    // Yarn: <<trigger_visual_effect glitch_labyrinth>>
    [YarnCommand("trigger_visual_effect")]
    public static void TriggerVisualEffect(string effectName)
    {
        Debug.Log($"[Визуал] Эффект: {effectName}");
        // TODO: вызов пост-процессинга или анимации
    }

    // ─── Вывести отладку ────────────────────────────────────
    // Yarn: <<debug_state>>
    [YarnCommand("debug_state")]
    public static void DebugState()
    {
        if (GameStateManager.Instance == null) return;
        GameStateManager.Instance.DebugPrintAllValues();
    }
}
