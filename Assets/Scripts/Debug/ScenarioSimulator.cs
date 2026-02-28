using UnityEngine;
using System.Collections.Generic;

public class ScenarioSimulator : MonoBehaviour
{
    [System.Serializable]
    public struct GameState
    {
        public int Control; // -100 (Dependence) ... +100 (Autonomy)
        public int World;   // -100 (Acceptance) ... +100 (Resistance)
        public int Truth;   // -100 (SelfDeception) ... +100 (Honesty)

        public void Add(int c, int w, int t)
        {
            Control = Mathf.Clamp(Control + c, -100, 100);
            World = Mathf.Clamp(World + w, -100, 100);
            Truth = Mathf.Clamp(Truth + t, -100, 100);
        }
        public override string ToString() => $"C:{Control}, W:{World}, T:{Truth}";
    }

    // Настройки поведения бота
    public enum StrategyType { 
        Rebel,      // Честность + Автономия + Сопротивление (True Ending)
        Doll,       // Самообман + Зависимость + Принятие (Doll Ending)
        Stoic,      // Честность + Автономия + Принятие (Финал 1)
        Trapped,    // Честность + Зависимость + Сопротивление (Финал 4)
        Actor,      // Самообман + Автономия + Принятие (Финал 5)
        Runner,     // Самообман + Автономия + Сопротивление (Финал 6)
        Chaos       // Случайные выборы (Финал 9 - Распад)
    }

    void Start()
    {
        Debug.Log("=== ПОЛНАЯ ПРОВЕРКА 9 ФИНАЛОВ (v2.1 FIXED) ===");
        SimulateGame("1. Истинный (Rebel)", StrategyType.Rebel);
        SimulateGame("2. Кукла (Doll)", StrategyType.Doll);
        SimulateGame("3. Смиренный (Stoic)", StrategyType.Stoic);
        SimulateGame("4. Загнанный (Trapped)", StrategyType.Trapped);
        SimulateGame("5. Актер (Actor)", StrategyType.Actor);
        SimulateGame("6. Беглец (Runner)", StrategyType.Runner);
        SimulateGame("7. Хаос (Random)", StrategyType.Chaos);
    }

    void SimulateGame(string botName, StrategyType strategy)
    {
        GameState state = new GameState();

        // --- ПРОХОЖДЕНИЕ СЦЕН ---
        // Сцена 1: Ворота
        MakeChoice(ref state, strategy, -5, 0, 0, 5, 0, 0); 

        // Сцена 2: Фонтан
        MakeChoice(ref state, strategy, 0, -5, 0, 0, 5, 0); 

        // Сцена 3: Вход
        if (IsResistance(strategy)) state.Add(0, 10, 5); // Резко
        else state.Add(0, -10, -5); // Принять

        // Сцена 4: Игривость
        if (IsHonesty(strategy)) state.Add(0, 5, 10);
        else if (IsAutonomy(strategy)) state.Add(0, 5, -10); 
        else state.Add(-10, -5, 0); 

        // Сцена 5: Экстравагантность
        if (IsAutonomy(strategy)) state.Add(10, 0, 10); 
        else state.Add(0, -5, -10); 

        // Сцена 7: Фонтан 2
        if (IsAutonomy(strategy)) state.Add(5, 0, 5); 
        else state.Add(-5, -5, 0);

        // Сцена 8: Ключи
        if (IsHonesty(strategy)) state.Add(0, 0, 15); else state.Add(0, 0, -15);

        // Сцена 8.5: Перепутье 1
        if (IsAutonomy(strategy)) state.Add(25, 0, 5); else state.Add(-25, 0, -5);

        // Сцена 10-12 (Зал Печали)
        if (IsResistance(strategy)) state.Add(0, 10, 0); else state.Add(0, -10, 0);
        if (IsAutonomy(strategy)) state.Add(10, 0, 0); else state.Add(-5, 0, 0);
        
        // Сцена 15: Уязвимость
        if (IsResistance(strategy)) state.Add(10, 5, 0); else state.Add(-10, -5, 0);
        
        // Сцена 16: Эхо
        if (IsHonesty(strategy)) state.Add(10, 0, 15); else state.Add(10, 0, -15);

        // Сцена 17: Наполнение
        if (IsHonesty(strategy)) state.Add(0, 10, 15); 
        else if (IsAutonomy(strategy)) state.Add(20, 0, -5); 
        else state.Add(0, -10, -15); 

        // Сцена 18.5: Перепутье 2
        if (IsAutonomy(strategy)) state.Add(25, 0, 0); else state.Add(-25, 0, 0);

        // Сцена 19: Желание
        if (IsHonesty(strategy) && IsResistance(strategy)) state.Add(0, 15, 15);
        else if (IsAutonomy(strategy)) state.Add(15, 15, 0);
        else state.Add(-15, -15, 0);

        // --- АНАЛИЗ ФИНАЛА ---
        Debug.Log($"Бот '{botName}': {state}");
        DetermineEnding(state);
    }

    bool IsAutonomy(StrategyType s) => s == StrategyType.Rebel || s == StrategyType.Stoic || s == StrategyType.Actor || s == StrategyType.Runner;
    bool IsResistance(StrategyType s) => s == StrategyType.Rebel || s == StrategyType.Trapped || s == StrategyType.Runner || (s == StrategyType.Chaos && Random.value > 0.5f);
    bool IsHonesty(StrategyType s) => s == StrategyType.Rebel || s == StrategyType.Stoic || s == StrategyType.Trapped;

    void MakeChoice(ref GameState state, StrategyType s, int c1, int w1, int t1, int c2, int w2, int t2)
    {
        bool takeSecond = IsAutonomy(s); 
        if (takeSecond) state.Add(c2, w2, t2);
        else state.Add(c1, w1, t1);
    }

    void DetermineEnding(GameState s)
    {
        // 1. ПРИОРИТЕТ 1: ИСТИННЫЙ ФИНАЛ (Rebel)
        // ТРЕБОВАНИЕ: Высокая Честность, Высокая Автономия, ЧЕТКОЕ Сопротивление (не ноль)
        if (s.Truth >= 60 && s.Control >= 30 && s.World >= 20) 
        {
            LogEnd(2, "ИСТИННЫЙ ВЫХОД (Rebel)");
            return;
        }

        // 2. ПРИОРИТЕТ 2: ФИНАЛ МАРИОНЕТКИ (Doll)
        // ТРЕБОВАНИЕ: Глубокий Самообман и Зависимость
        if (s.Truth <= -60 && s.Control <= -40) 
        {
            LogEnd(7, "МАРИОНЕТКА (Doll)");
            return;
        }

        // 3. ОСТАЛЬН��Е ФИНАЛЫ (По Знаку Оси)
        // Считаем НОЛЬ (0) за ОТРИЦАТЕЛЬНОЕ значение (Принятие / Зависимость / Самообман)
        // Это закрывает дыры для "пограничных" состояний
        
        bool isAutonomy = s.Control > 0; // > 0 = Auto, <= 0 = Dep
        bool isResistance = s.World > 0; // > 0 = Res, <= 0 = Acc
        bool isHonesty = s.Truth > 0;    // > 0 = Hon, <= 0 = SelfDec

        if (isHonesty && isAutonomy && !isResistance) LogEnd(1, "Я остаюсь собой (Stoic)");
        
        else if (isHonesty && !isAutonomy && !isResistance) LogEnd(3, "Мне нужна опора");
        
        else if (isHonesty && !isAutonomy && isResistance) LogEnd(4, "Болезненная правда (Trapped)");
        
        else if (!isHonesty && isAutonomy && !isResistance) LogEnd(5, "Красивая роль (Actor)");
        
        else if (!isHonesty && isAutonomy && isResistance) LogEnd(6, "Побег (Runner)");
        
        else if (!isHonesty && !isAutonomy && isResistance) LogEnd(8, "Безумное слияние");
        
        // Сюда попадают те, кто не дотянул до Истинного/Куклы, но имеет их характеристики
        else if (!isHonesty && !isAutonomy && !isResistance) LogEnd(7, "МАРИОНЕТКА (Soft Variant)");
        else if (isHonesty && isAutonomy && isResistance) LogEnd(2, "ИСТИННЫЙ (Soft Variant)"); // Не дотянул очков, но направление верное
        
        else LogEnd(9, "РАСПАД / ОШИБКА");
        
        Debug.Log("-----------------------------");
    }

    void LogEnd(int id, string name)
    {
        string color = (id == 2) ? "green" : (id == 7) ? "red" : "white";
        Debug.Log($"<color={color}>>>> ФИНАЛ {id}: {name}</color>");
    }
}