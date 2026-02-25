using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{
    // static — значит Unity найдёт метод без указания объекта
    
    [YarnCommand("guide_speaks")]
    public static void GuideSpeaks()
    {
        Debug.Log("Проводник перехватывает управление!");
    }

    [YarnCommand("shift_axis")]
    public static void ShiftAxis(string axisName, float value)
    {
        Debug.Log($"Ось '{axisName}' изменена на {value}");
    }

    [YarnCommand("trigger_glitch")]
    public static void TriggerGlitch(string intensity)
    {
        Debug.Log($"Глитч запущен: уровень {intensity}");
    }
}