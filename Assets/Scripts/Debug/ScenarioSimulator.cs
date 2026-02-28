using UnityEngine;

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
            World   = Mathf.Clamp(World + w, -100, 100);
            Truth   = Mathf.Clamp(Truth + t, -100, 100);
        }
        public override string ToString() => $"C:{Control} W:{World} T:{Truth}";
    }

    // ─── Все 9 стратегий ────────────────────────────────────
    public enum StrategyType
    {
        Stoic,      // Ч+А+П → Финал 1
        Rebel,      // Ч+А+С → Финал 2
        Supporter,  // Ч+З+П → Финал 3
        Trapped,    // Ч+З+С → Финал 4
        Actor,      // С+А+П → Финал 5
        Runner,     // С+А+С → Финал 6
        Doll,       // С+З+П → Финал 7
        Fusion,     // С+З+С → Финал 8
        Collapse    // ~0~0~0 → Финал 9
    }

    // ─── Запуск ─────────────────────────────────────────────
    void Start()
    {
        Debug.Log("╔══════════════════════════════════════════╗");
        Debug.Log("║   ПРОВЕРКА ВСЕХ 9 ФИНАЛОВ (v3.0)        ║");
        Debug.Log("╚══════════════════════════════════════════╝\n");

        SimulateGame("Финал 1 — Смиренный странник",  StrategyType.Stoic);
        SimulateGame("Финал 2 — Истинный выход",      StrategyType.Rebel);
        SimulateGame("Финал 3 — Мне нужна опора",     StrategyType.Supporter);
        SimulateGame("Финал 4 — Болезненная правда",   StrategyType.Trapped);
        SimulateGame("Финал 5 — Красивая роль",       StrategyType.Actor);
        SimulateGame("Финал 6 — Побег",               StrategyType.Runner);
        SimulateGame("Финал 7 — Марионетка",          StrategyType.Doll);
        SimulateGame("Финал 8 — Безумное слияние",    StrategyType.Fusion);
        SimulateGame("Финал 9 — РАСПАД",              StrategyType.Collapse);
    }

    // ─── Определители стратегий ─────────────────────────────
    bool WantsAutonomy(StrategyType s) =>
        s == StrategyType.Rebel || s == StrategyType.Stoic ||
        s == StrategyType.Actor || s == StrategyType.Runner;

    bool WantsResistance(StrategyType s) =>
        s == StrategyType.Rebel || s == StrategyType.Trapped ||
        s == StrategyType.Runner || s == StrategyType.Fusion;

    bool WantsHonesty(StrategyType s) =>
        s == StrategyType.Rebel || s == StrategyType.Stoic ||
        s == StrategyType.Supporter || s == StrategyType.Trapped;

    // ─── Симуляция ──────────────────────────────────────────
    void SimulateGame(string label, StrategyType strat)
    {
        GameState gs = new GameState();

        if (strat == StrategyType.Collapse)
        {
            SimulateCollapse(ref gs);
        }
        else
        {
            SimulateNormal(ref gs, strat);
        }

        Debug.Log($"[{label}] → {gs}");
        int ending = DetermineEnding(gs);
        string endName = GetEndingName(ending);
        string color = ending == 2 ? "green" : ending == 7 ? "red" : ending == 9 ? "magenta" : "yellow";
        Debug.Log($"<color={color}>  >>> ФИНАЛ {ending}: {endName}</color>");
        Debug.Log("─────────────────────────────────────────\n");
    }

    // ─── Нормальная симуляция (8 стратегий) ──────────────────
    void SimulateNormal(ref GameState gs, StrategyType s)
    {
        bool a = WantsAutonomy(s);
        bool r = WantsResistance(s);
        bool h = WantsHonesty(s);

        // ── Сцена 1: Ворота ──
        // Робко = Control -5 | Холодно = Control +5
        gs.Add(a ? 5 : -5, 0, 0);

        // ── Сцена 2: Фонтан (подавление голоса) ──
        // Спокойствие = World -5 | Раздражение = World +5
        gs.Add(0, r ? 5 : -5, 0);

        // ── Сцена 3: Зал Радости (ритуал) ──
        // Принять = W-10 T-5 | Резко = W+10 T+5
        if (r) gs.Add(0, 10, 5);
        else   gs.Add(0, -10, -5);

        // ── Сцена 4: Игривость (контракт) ──
        // Попросить Проводника = C-10 W-5
        // Понравиться = T-10 C+5
        // Жёстко = T+10 W+5
        if (h)       gs.Add(0, 5, 10);     // жёстко и честно
        else if (a)  gs.Add(5, 0, -10);    // понравиться (сама, но обман)
        else         gs.Add(-10, -5, 0);   // попросить Проводника

        // ── Сцена 5: Экстравагантность (роль) ──
        // Согласие = T-10 W-5 | Отказ = T+10 C+5 | Сделка = C+10 T-5
        if (h)       gs.Add(5, 0, 10);     // отказ (честность)
        else if (a)  gs.Add(10, 0, -5);    // сделка (автономия)
        else         gs.Add(0, -5, -10);   // полное согласие

        // ── Сцена 7: Фонтан в лабиринте ──
        // Правила = C-5 W-5 | Брат = C+5 T+5 | Проводник = C-10
        if (a && h)  gs.Add(5, 0, 5);      // упор на брата
        else if (a)  gs.Add(5, 0, 0);      // упор на брата (но без честности)
        else         gs.Add(-5, -5, 0);    // согласиться на правила

        // ── Сцена 8: Мини-игры (ключи) ──
        // Золотой = T+15 | Серебряный = T-15
        gs.Add(0, 0, h ? 15 : -15);

        // ── Сцена 8.5: Перепутье 1 ──
        // Автономия: C+20 T+5 | Зависимость: C-20 T-5
        if (a)  gs.Add(20, 0, 5);
        else    gs.Add(-20, 0, -5);

        // ── Сцена 10: Вход в Зал Печали ──
        // Осторожно = W-5 | Искать = W+5 | Остановиться = C+5
        gs.Add(0, r ? 5 : -5, 0);

        // ── Сцена 11: Неполноценность (реакция + чувство) ──
        // Настоять = C+5 | Вежливо = W-5 | Замкнуться = C-5
        if (a)       gs.Add(5, 0, 0);
        else if (r)  gs.Add(0, 5, 0);
        else         gs.Add(-5, -5, 0);

        // Чувство: Ярость = W+5 T+5 | Согласие = W-5 T-5 | Замешательство = C-5
        if (r && h)  gs.Add(0, 5, 5);      // ярость
        else if (!r) gs.Add(0, -5, -5);    // согласие
        else         gs.Add(-5, 0, 0);     // замешательство

        // ── Сцена 12: Вина (мысли-выводы) ──
        // Справилась = C+10 | Страшно = T+10 | Несправедливо = W+10
        if (a)       gs.Add(10, 0, 0);     // справилась
        else if (h)  gs.Add(0, 0, 10);     // страшно
        else if (r)  gs.Add(0, 10, 0);     // несправедливо
        else         gs.Add(0, -5, -5);    // нет подходящего — усиливаем принятие

        // ── Сцена 15: Уязвимость ──
        // Помощь = C-10 W-5 | Терпеть = W-10 C+5 | Резко = W+10 C+5
        if (r)       gs.Add(5, 10, 0);     // ответить резко
        else if (a)  gs.Add(5, -10, 0);    // терпеть (автономия, но принятие)
        else         gs.Add(-10, -5, 0);   // искать помощи

        // ── Сцена 15: Ярость (принятие гнева) ──
        // Не хочу = T-10 C-5 | Страшно но приму = T+10 W-5 | Приму (сила) = W+10 C+5
        if (h)       gs.Add(0, -5, 10);    // страшно но приму
        else if (r)  gs.Add(5, 10, 0);     // приму с силой
        else         gs.Add(-5, 0, -10);   // не хочу принимать

        // ── Сцена 16: Эхо Ярости (задание) ──
        // Помогу = T+15 C+5 | Ради брата = T-15 C+5
        gs.Add(5, 0, h ? 15 : -15);

        // ── Сцена 17: Наполнение флакона ──
        // Приговор = T+15 W+10 | Забвение = T-15 W-10 | Прагматизм = C+15 T-5
        if (h && r)      gs.Add(0, 10, 15);    // холодный приговор
        else if (h)      gs.Add(0, 0, 15);     // приговор (без сопротивления)
        else if (a && r) gs.Add(15, 0, -5);    // прагматизм
        else             gs.Add(0, -10, -15);  // сладкое забвение

        // ── Сцена 18.5: Перепутье 2 ──
        // Автономия: C+20 | Зависимость: C-20
        if (a)  gs.Add(20, 0, 0);
        else    gs.Add(-20, 0, 0);

        // ── Сцена 19: Желание ──
        // 1. Сорвать маски = T+15 W+15
        // 2. Счастливы здесь = T-15 W-15
        // 3. Забираю сама = C+15 W+15
        // 4. Скажи как спасти = C-15 W-15
        // 5. Любую цену = T+10 C-15
        if (h && r)           gs.Add(0, 15, 15);    // сорвать маски
        else if (a && r)      gs.Add(15, 15, 0);    // забираю ��ама
        else if (h && !a)     gs.Add(-15, 0, 10);   // любую цену
        else if (!h && !r)    gs.Add(-15, -15, 0);  // счастливы здесь
        else                  gs.Add(0, -15, -15);  // fallback: счастливы здесь
    }

    // ─── Симуляция РАСПАДА (нерешительный игрок) ─────────────
    void SimulateCollapse(ref GameState gs)
    {
        // Игрок чередует противоположные выборы → оси остаются ~0
        gs.Add(5, 0, 0);     // Сц1: холодно (+автономия)
        gs.Add(0, -5, 0);    // Сц2: спокойствие (+принятие)
        gs.Add(0, 10, 5);    // Сц3: резко (+сопротивление)
        gs.Add(-10, -5, 0);  // Сц4: попросить Проводника (+зависимость)
        gs.Add(0, -5, -10);  // Сц5: согласие (+самообман)
        gs.Add(5, 0, 5);     // Сц7: упор на брата (+автономия, +честность)
        gs.Add(0, 0, -15);   // Сц8: серебряный ключ (+самообман)
        gs.Add(0, 5, 5);     // Сц8.5: чередуем (маленький сдвиг)
        gs.Add(0, -5, 0);    // Сц10: осторожно (+принятие)
        gs.Add(-5, 0, 0);    // Сц11: замкнуться (+зависимость)
        gs.Add(0, 5, 5);     // Сц11р: ярость
        gs.Add(0, 0, 10);    // Сц12: страшно (+честность, но раньше был самообман)
        gs.Add(-10, -5, 0);  // Сц15у: помощь (+зависимость)
        gs.Add(0, -5, 10);   // Сц15я: страшно но приму
        gs.Add(5, 0, -15);   // Сц16: ради брата (+самообман)
        gs.Add(0, -10, -15); // Сц17: забвение
        gs.Add(0, 5, 0);     // Сц18.5: маленький сдвиг
        gs.Add(0, 15, 15);   // Сц19: сорвать маски (противоречит всему)
        // Итог: оси должны быть близки к 0
    }

    // ─── Определение финала (зеркалирует GameStateManager) ──
    int DetermineEnding(GameState s)
    {
        // Приоритет 1: Истинный выход
        if (s.Truth >= 60 && s.Control >= 30 && s.World >= 20)
            return 2;

        // Приоритет 2: Марионетка (обязательно Принятие — World <= 0!)
        if (s.Truth <= -60 && s.Control <= -40 && s.World <= 0)
            return 7;

        // Приоритет 3: РАСПАД
        if (Mathf.Abs(s.Control) <= 10 &&
            Mathf.Abs(s.World) <= 10 &&
            Mathf.Abs(s.Truth) <= 10)
            return 9;

        // По знаку осей
        bool isA = s.Control > 0;
        bool isR = s.World > 0;
        bool isH = s.Truth > 0;

        if (isH && isA && !isR)   return 1; // Смиренный
        if (isH && isA && isR)    return 2; // Истинный (soft)
        if (isH && !isA && !isR)  return 3; // Опора
        if (isH && !isA && isR)   return 4; // Загнанный
        if (!isH && isA && !isR)  return 5; // Актёр
        if (!isH && isA && isR)   return 6; // Беглец
        if (!isH && !isA && !isR) return 7; // Марионетка (soft)
        if (!isH && !isA && isR)  return 8; // Слияние

        return 9; // Fallback
    }

    string GetEndingName(int id) => id switch
    {
        1 => "Я остаюсь собой (Смиренный странник)",
        2 => "ИСТИННЫЙ ВЫХОД (Бунтарь)",
        3 => "Мне нужна опора",
        4 => "Болезненная правда (Загнанный)",
        5 => "Красивая роль (Актёр)",
        6 => "Побег (Беглец)",
        7 => "Марионетка / Преемница",
        8 => "Безумное слияние (Близнецы)",
        9 => "РАСПАД",
        _ => "ОШИБКА"
    };
}