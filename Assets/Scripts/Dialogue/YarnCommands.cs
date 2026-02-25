using UnityEngine;
using Yarn.Unity;
using Core;

public class YarnCommands : MonoBehaviour
{
    [YarnCommand("guide_speaks")]
    public static void GuideSpeaks()
    {
        // Пока лог — логику добавим в Этапе 2
        Debug.Log("Проводник перехватывает управление!");
    }

    [YarnCommand("shift_axis")]
    public static void ShiftAxis(string axisName, float value)
    {
        // Проверяем что GameStateManager существует на сцене
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager не найден на сцене!");
            return;
        }

        // Теперь реально меняем ось!
        GameStateManager.Instance.ShiftAxis(axisName, value);
    }

    [YarnCommand("trigger_glitch")]
    public static void TriggerGlitch(string intensity)
    {
        // Пока лог — глитч сделаем в Этапе 3
        Debug.Log($"Глитч запущен: уровень {intensity}");
    }
}