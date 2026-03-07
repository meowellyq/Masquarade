using UnityEngine;
using Core;

public class DebugOverlay : MonoBehaviour
{
    private bool _isVisible = true;
    private Vector2 _scrollPos;

    // Список доступных Yarn-нод для быстрого перехода
    private readonly string[] _sceneNodes = new string[]
    {
        "Scene01_Start",
        "Scene02_Start",
        "Scene03_Start",
        "Scene04_Start",
        "Scene05_Start",
        "Scene07_Start",
        "Scene08_Start",
        "Scene08_5_Start",
        "Scene10_Start",
        "Scene11_Start",
        "Scene12_Start",
        "Scene14_Start",
        "Scene15_Start",
        "Scene16_Start",
        "Scene17_Start",
        "Scene18_5_Start",
        "Scene19_Start",
        "Scene20_Start"
    };

    private void Awake()
    {
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

        GUI.skin.box.fontSize = 14;
        GUI.skin.button.fontSize = 12;
        GUI.skin.label.fontSize = 14;

        float panelWidth = 320f;
        float panelHeight = 460f;
        float x = Screen.width - panelWidth - 10f;
        float y = 10f;

        GUI.Box(new Rect(x, y, panelWidth, panelHeight), "");

        GUILayout.BeginArea(new Rect(x + 10, y + 10, panelWidth - 20, panelHeight - 20));

        var titleStyle = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 };
        GUILayout.Label("<b><color=yellow>DEBUG PANEL (F1)</color></b>", titleStyle);
        GUILayout.Space(5);

        // ─── Оси ────────────────────────────────────────────
        string cColor = gsm.control > 0 ? "lime" : gsm.control < 0 ? "red" : "white";
        string wColor = gsm.world > 0 ? "lime" : gsm.world < 0 ? "red" : "white";
        string tColor = gsm.truth > 0 ? "lime" : gsm.truth < 0 ? "red" : "white";

        var richLabel = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 15 };

        GUILayout.Label($"<b>Control:</b> <color={cColor}>{gsm.control}</color>  " +
                        "(Zavis./Avton.)", richLabel);
        GUILayout.Label($"<b>World:</b>   <color={wColor}>{gsm.world}</color>  " +
                        "(Prinyat./Sopr.)", richLabel);
        GUILayout.Label($"<b>Truth:</b>   <color={tColor}>{gsm.truth}</color>  " +
                        "(Samoob./Chestn.)", richLabel);

        GUILayout.Space(3);

        int ending = gsm.DetermineEnding();
        GUILayout.Label($"<b>Ending: <color=cyan>{ending}</color></b>  " +
                        $"Keys: G:{gsm.goldenKeys} S:{gsm.silverKeys}", richLabel);

        GUILayout.Space(8);

        // ─── Кнопки осей ────────────────────────────────────
        GUILayout.Label("<b>Shift axes:</b>", richLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("C +10")) gsm.ShiftAxis("control", 10);
        if (GUILayout.Button("C -10")) gsm.ShiftAxis("control", -10);
        if (GUILayout.Button("C = 0")) gsm.control = 0;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("W +10")) gsm.ShiftAxis("world", 10);
        if (GUILayout.Button("W -10")) gsm.ShiftAxis("world", -10);
        if (GUILayout.Button("W = 0")) gsm.world = 0;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("T +10")) gsm.ShiftAxis("truth", 10);
        if (GUILayout.Button("T -10")) gsm.ShiftAxis("truth", -10);
        if (GUILayout.Button("T = 0")) gsm.truth = 0;
        GUILayout.EndHorizontal();

        GUILayout.Space(8);

        // ─── Переход к сценам ───────────────────────────────
        GUILayout.Label("<b>Jump to scene:</b>", richLabel);

        _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(150));

        for (int i = 0; i < _sceneNodes.Length; i += 3)
        {
            GUILayout.BeginHorizontal();
            for (int j = i; j < Mathf.Min(i + 3, _sceneNodes.Length); j++)
            {
                string nodeName = _sceneNodes[j];
                string shortName = nodeName.Replace("Scene", "Sc.")
                                           .Replace("_Start", "")
                                           .Replace("_5", ".5");

                if (GUILayout.Button(shortName, GUILayout.Width(85)))
                {
                    JumpToNode(nodeName);
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUILayout.Space(5);

        // ─── Сброс ─────────────────────────────────────────
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset all"))
        {
            gsm.control = 0;
            gsm.world = 0;
            gsm.truth = 0;
            gsm.goldenKeys = 0;
            gsm.silverKeys = 0;
            Debug.Log("[Debug] All axes reset");
        }
        if (GUILayout.Button("Log state"))
        {
            gsm.DebugPrintAllValues();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    // ─── Переход к Yarn-ноде ────────────────────────────────
    private void JumpToNode(string nodeName)
    {
        // Ищем DialogueRunner через FindObjectOfType
        var runner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        if (runner == null)
        {
            Debug.LogError("[Debug] DialogueRunner not found in scene!");
            return;
        }

        if (runner.IsDialogueRunning)
        {
            runner.Stop();
        }

        runner.StartDialogue(nodeName);
        Debug.Log($"[Debug] Jump to: {nodeName}");
    }
}