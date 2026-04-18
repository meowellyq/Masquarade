using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

public class DebugOverlay : MonoBehaviour
{
    // ─── Singleton — фикс дубликатов ────────────────────
    private static DebugOverlay _instance;

    private bool _isVisible = true;
    private Vector2 _flagsScrollPos;

    private readonly string[] _sceneNodes = new string[]
    {
        "Scene01_Start",
        "Scene02_Start",
        "Scene03_Start",
        "Scene04_Start",
        "Scene05_Start",
        "Scene07_Start",
        "Scene08_Start",
        "Scene08_5_Pond",
        "Scene09_ReturnToFountain",
        "Scene10_HallOfSorrow",
        "Scene11_Inadequacy",
        "Scene12_Guilt",
        "Scene13_Guide",
        "Scene15_Wrath",
        "Scene16_Echo",
        "Scene17_Inadequacy",
        "Scene18_GazeboReturn",
        "Scene18_5_Gazebo",
        "Scene19_Start",
        "Scene20_Start"
    };

    private void Awake()
    {
        // Если экземпляр уже есть — уничтожаем дубликат
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            _isVisible = !_isVisible;
    }

    private void OnGUI()
    {
        if (!_isVisible) return;

        var gsm = GameStateManager.Instance;
        if (gsm == null) return;

        GUI.skin.box.fontSize    = 14;
        GUI.skin.button.fontSize = 11;
        GUI.skin.label.fontSize  = 13;

        float panelWidth  = 340f;
        float panelHeight = Screen.height - 20f;
        float x = Screen.width - panelWidth - 10f;
        float y = 10f;

        GUI.Box(new Rect(x, y, panelWidth, panelHeight), "");
        GUILayout.BeginArea(new Rect(x + 8, y + 8, panelWidth - 16, panelHeight - 16));
        _flagsScrollPos = GUILayout.BeginScrollView(_flagsScrollPos);

        var bold    = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 15 };
        var rich    = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 12 };

        GUILayout.Label("<b><color=yellow>DEBUG PANEL  (F1)</color></b>", bold);
        GUILayout.Space(4);

        // ─── Оси ────────────────────────────────────────────
        string cColor = gsm.control > 0 ? "lime" : gsm.control < 0 ? "red" : "white";
        string wColor = gsm.world   > 0 ? "lime" : gsm.world   < 0 ? "red" : "white";
        string tColor = gsm.truth   > 0 ? "lime" : gsm.truth   < 0 ? "red" : "white";

        GUILayout.Label($"<b>Control:</b> <color={cColor}>{gsm.control:+0;-0;0}</color>  Завис.(−) / Автон.(+)", rich);
        GUILayout.Label($"<b>World:</b>   <color={wColor}>{gsm.world:+0;-0;0}</color>  Принят.(−) / Сопрот.(+)", rich);
        GUILayout.Label($"<b>Truth:</b>   <color={tColor}>{gsm.truth:+0;-0;0}</color>  Самооб.(−) / Честн.(+)", rich);
        GUILayout.EndScrollView();
        int ending = gsm.DetermineEnding();
        GUILayout.Label($"<b>Ending: <color=cyan>{ending}</color></b>  | Keys G:{gsm.goldenKeys} S:{gsm.silverKeys}  | Flask:<color=cyan>{(string.IsNullOrEmpty(gsm.flask) ? "—" : gsm.flask)}</color>", rich);
        GUILayout.Label($"Guide: <color=cyan>{gsm.DetermineGuideArchetype()}</color>  | Memory loss: <color={(gsm.memoryLoss ? "red" : "lime")}>{gsm.memoryLoss}</color>  | Spawn: <color=cyan>{gsm.spawnPointId}</color>", rich);
        GUILayout.Space(4);

        // ─── Кнопки осей ────────────────────────────────────
        GUILayout.Label("<b>Оси:</b>", rich);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("C +10")) gsm.ShiftAxis("control",  10);
        if (GUILayout.Button("C -10")) gsm.ShiftAxis("control", -10);
        if (GUILayout.Button("C = 0")) gsm.control = 0;
        if (GUILayout.Button("C=+80")) gsm.control =  80;
        if (GUILayout.Button("C=-80")) gsm.control = -80;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("W +10")) gsm.ShiftAxis("world",  10);
        if (GUILayout.Button("W -10")) gsm.ShiftAxis("world", -10);
        if (GUILayout.Button("W = 0")) gsm.world = 0;
        if (GUILayout.Button("W=+80")) gsm.world =  80;
        if (GUILayout.Button("W=-80")) gsm.world = -80;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("T +10")) gsm.ShiftAxis("truth",  10);
        if (GUILayout.Button("T -10")) gsm.ShiftAxis("truth", -10);
        if (GUILayout.Button("T = 0")) gsm.truth = 0;
        if (GUILayout.Button("T=+80")) gsm.truth =  80;
        if (GUILayout.Button("T=-80")) gsm.truth = -80;
        GUILayout.EndHorizontal();

        // Быстрые пресеты осей для финалов
        GUILayout.Label("<b>Пресеты финалов:</b>", rich);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Ф1 ++-")) { gsm.truth= 60; gsm.control= 60; gsm.world=-60; }
        if (GUILayout.Button("Ф2 +++")) { gsm.truth= 80; gsm.control= 80; gsm.world= 80; }
        if (GUILayout.Button("Ф3 +--")) { gsm.truth= 60; gsm.control=-60; gsm.world=-60; }
        if (GUILayout.Button("Ф4 +-+")) { gsm.truth= 60; gsm.control=-60; gsm.world= 60; }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Ф5 --+")) { gsm.truth=-60; gsm.control= 60; gsm.world=-60; }  // самообман+автон+принятие: world отрицательный
        if (GUILayout.Button("Ф6 -++")) { gsm.truth=-60; gsm.control= 60; gsm.world= 60; }
        if (GUILayout.Button("Ф7 ---")) { gsm.truth=-60; gsm.control=-60; gsm.world=-60; }
        if (GUILayout.Button("Ф8 --+")) { gsm.truth=-60; gsm.control=-60; gsm.world= 60; }
        if (GUILayout.Button("Ф9 000")) { gsm.truth=  0; gsm.control=  0; gsm.world=  0; }
        GUILayout.EndHorizontal();
        GUILayout.Space(4);

        // ─── Ключи ──────────────────────────────────────────
        GUILayout.Label("<b>Ключи:</b>", rich);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("G+1")) { gsm.goldenKeys++; gsm.hasExtravaganceKey = true; }
        if (GUILayout.Button("G-1")) gsm.goldenKeys = Mathf.Max(0, gsm.goldenKeys - 1);
        if (GUILayout.Button("S+1")) { gsm.silverKeys++; gsm.hasInadequacyKey = true; }
        if (GUILayout.Button("S-1")) gsm.silverKeys = Mathf.Max(0, gsm.silverKeys - 1);
        if (GUILayout.Button("2G"))  { gsm.goldenKeys = 2; gsm.silverKeys = 0; gsm.hasExtravaganceKey = true; gsm.hasInadequacyKey = true; }
        if (GUILayout.Button("2S"))  { gsm.silverKeys = 2; gsm.goldenKeys = 0; gsm.hasExtravaganceKey = true; gsm.hasInadequacyKey = true; }
        GUILayout.EndHorizontal();
        GUILayout.Space(4);

        // ─── Флакон ─────────────────────────────────────────
        GUILayout.Label("<b>Флакон:</b>", rich);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("empty")) gsm.flask = "empty";
        if (GUILayout.Button("black")) gsm.flask = "black";
        if (GUILayout.Button("pink"))  gsm.flask = "pink";
        if (GUILayout.Button("grey"))  gsm.flask = "grey";
        if (GUILayout.Button("clear")) gsm.flask = "";
        GUILayout.EndHorizontal();
        GUILayout.Space(4);

        // ─── Флаги ──────────────────────────────────────────
        GUILayout.Label("<b>Флаги прогресса:</b>", rich);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Lbl("Pond",     gsm.pondVisited)))         gsm.pondVisited         = !gsm.pondVisited;
        if (GUILayout.Button(Lbl("Fountain", gsm.fountainDone)))        gsm.fountainDone        = !gsm.fountainDone;
        if (GUILayout.Button(Lbl("Hall",     gsm.hallOfSorrowEntered))) gsm.hallOfSorrowEntered = !gsm.hallOfSorrowEntered;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Lbl("WrathEcho",   gsm.wrathEchoDone)))   gsm.wrathEchoDone   = !gsm.wrathEchoDone;
        if (GUILayout.Button(Lbl("GazeboRet",   gsm.gazeboReturnDone)))gsm.gazeboReturnDone = !gsm.gazeboReturnDone;
        if (GUILayout.Button(Lbl("MemLoss",     gsm.memoryLoss)))      gsm.memoryLoss      = !gsm.memoryLoss;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Lbl("Extrav",  gsm.hasExtravaganceKey))) gsm.hasExtravaganceKey = !gsm.hasExtravaganceKey;
        if (GUILayout.Button(Lbl("Inad",    gsm.hasInadequacyKey)))   gsm.hasInadequacyKey   = !gsm.hasInadequacyKey;
        GUILayout.EndHorizontal();

        // Sc10 choice
        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b>Sc10Choice:</b> <color=cyan>{gsm.scene10Choice}</color>", rich, GUILayout.Width(130));
        if (GUILayout.Button("1")) gsm.scene10Choice = 1;
        if (GUILayout.Button("2")) gsm.scene10Choice = 2;
        if (GUILayout.Button("3")) gsm.scene10Choice = 3;
        GUILayout.EndHorizontal();
        GUILayout.Space(4);

        // ─── Быстрый полный стейт для Scene18+ ─────────────
        GUILayout.Label("<b>Быстрый стейт для теста:</b>", rich);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("→ Scene18+"))
        {
            gsm.hasExtravaganceKey = true; gsm.hasInadequacyKey = true;
            gsm.fountainDone = true; gsm.pondVisited = true;
            gsm.hallOfSorrowEntered = true; gsm.wrathEchoDone = true;
        }
        if (GUILayout.Button("→ Scene19+"))
        {
            gsm.hasExtravaganceKey = true; gsm.hasInadequacyKey = true;
            gsm.fountainDone = true; gsm.pondVisited = true;
            gsm.hallOfSorrowEntered = true; gsm.wrathEchoDone = true;
            gsm.gazeboReturnDone = true; gsm.flask = "black";
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(4);

        // ─── Сцены ──────────────────────────────────────────
        GUILayout.Label("<b>Загрузить сцену:</b>", rich);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("DialogueScene"))  SceneManager.LoadScene("DialogueScene");
        if (GUILayout.Button("LabyrinthScene")) SceneManager.LoadScene("LabyrinthScene");
        GUILayout.EndHorizontal();
        GUILayout.Space(4);

        // ─── Yarn ноды ──────────────────────────────────────
        GUILayout.Label("<b>Перейти к ноде:</b>", rich);
        

        for (int i = 0; i < _sceneNodes.Length; i += 3)
        {
            GUILayout.BeginHorizontal();
            for (int j = i; j < Mathf.Min(i + 3, _sceneNodes.Length); j++)
            {
                string node  = _sceneNodes[j];
                string label = node
                    .Replace("Scene", "S")
                    .Replace("_Start", "")
                    .Replace("ReturnToFountain", ".Fount")
                    .Replace("_Inadequacy", ".Inad")
                    .Replace("_GazeboReturn", ".GazRet")
                    .Replace("HallOfSorrow", ".Hall")
                    .Replace("_Wrath", ".Wr")
                    .Replace("_Echo", ".Echo")
                    .Replace("_Guilt", ".Guilt")
                    .Replace("_Guide", ".Guide")
                    .Replace("_Pond", ".Pond")
                    .Replace("_5_Gazebo", ".5.Gaz");
                if (GUILayout.Button(label, GUILayout.Width(96)))
                    JumpToNode(node);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.Space(4);

        // ─── Сброс / Лог ────────────────────────────────────
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset ALL"))
        {
            gsm.control = 0; gsm.world = 0; gsm.truth = 0;
            gsm.goldenKeys = 0; gsm.silverKeys = 0;
            gsm.hasExtravaganceKey = false; gsm.hasInadequacyKey = false;
            gsm.fountainDone = false; gsm.pondVisited = false;
            gsm.hallOfSorrowEntered = false; gsm.wrathEchoDone = false;
            gsm.gazeboReturnDone = false; gsm.memoryLoss = false;
            gsm.flask = ""; gsm.scene10Choice = 0;
            Debug.Log("[Debug] Полный сброс");
        }
        if (GUILayout.Button("Log state")) gsm.DebugPrintAllValues();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    // Кнопка-переключатель с цветом по значению флага
    private static string Lbl(string name, bool val)
        => val ? $"<color=lime>✓{name}</color>" : $"<color=red>✗{name}</color>";

    private void JumpToNode(string nodeName)
    {
        // Всегда сохраняем ноду и перезагружаем сцену —
        // это гарантирует что RestoreVariables вызовется с актуальными значениями
        GameStateManager.Instance.currentYarnNode = nodeName;
        SceneManager.LoadScene("DialogueScene");
        Debug.Log($"[Debug] Jump → {nodeName}");
    }
}