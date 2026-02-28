using UnityEngine;
using Yarn.Unity;
using Core;

public class YarnCommands : MonoBehaviour
{
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
    }

    // ─── Проводник перехватывает управление ──────────────────
    // Yarn: <<guide_speaks>>
    [YarnCommand("guide_speaks")]
    public static void GuideSpeaks()
    {
        Debug.Log("Проводник перехватывает управление!");
        // TODO Этап 2: визуальный эффект перехвата голоса
    }

    // ─── Глитч-эффект ──────────────────────────────────────
    // Yarn: <<trigger_glitch low>>
    // Yarn: <<trigger_glitch medium>>
    // Yarn: <<trigger_glitch high>>
    [YarnCommand("trigger_glitch")]
    public static void TriggerGlitch(string intensity)
    {
        Debug.Log($"Глитч запущен: уровень {intensity}");
        // TODO Этап 7: пост-процессинг глитч-шейдер
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
    }

    // ─── Перепутье: определить самую неопределённую ось ──────
    // Yarn: <<check_closest_axis>>
    // После вызова проверяй через: <<if $closest_axis == "control">>
    //
    // ВАЖНО: для работы с $closest_axis нужен YarnVariableSync
    // (см. TODO ниже). Пока значение только в логе.
    [YarnCommand("check_closest_axis")]
    public static void CheckClosestAxis()
    {
        if (GameStateManager.Instance == null) return;
        string axis = GameStateManager.Instance.FindClosestAxis();
        Debug.Log($"[Перепутье] Самая неопределённая ось: {axis}");

        // TODO: записать axis в Yarn InMemoryVariableStorage
        // как $closest_axis, чтобы использовать в <<if>>
    }

    // ─── Определить финал ──────────────────────────────────
    // Yarn: <<determine_ending>>
    // После вызова проверяй: <<if $ending == 1>>
    [YarnCommand("determine_ending")]
    public static void DetermineEnding()
    {
        if (GameStateManager.Instance == null) return;
        int ending = GameStateManager.Instance.DetermineEnding();
        Debug.Log($"[Финал] Определён финал: {ending}");

        // TODO: записать ending в Yarn InMemoryVariableStorage
        // как $ending
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

        // TODO: записать archetype в Yarn InMemoryVariableStorage
        // как $guide_type
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

        // TODO: записать type в Yarn InMemoryVariableStorage
        // как $breakdown_type
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

        // TODO: записать flask в Yarn InMemoryVariableStorage
        // как $flask
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

        // TODO: записать loss в Yarn InMemoryVariableStorage
        // как $memory_loss
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

        // TODO: записать keyType в Yarn InMemoryVariableStorage
        // как $key_type
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