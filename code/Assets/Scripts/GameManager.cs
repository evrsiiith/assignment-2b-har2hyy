using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public Bowl bowl;
    public PourableContainer cerealBox;
    public PourableContainer milkCarton;
    public SpoonController spoon;
    public TargetZone targetZone;
    public Camera mainCamera;

    [Header("UI")]
    public Text feedbackText;   // Bottom center — shows action hints & state messages
    public Text controlsText;   // Top corner — persistent controls reminder

    private const string ControlsHint =
        "CONTROLS\n" +
        "Left-click + drag  →  Move object\n" +
        "Scroll while dragging  →  Push / pull object closer or farther\n" +
        "Right-click + drag  →  Rotate camera\n" +
        "Click cereal box / milk carton  →  Pour into bowl\n" +
        "Click spoon (near bowl)  →  Scoop food\n" +
        "Drag scooped spoon toward camera, then release  →  Eat";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (controlsText != null)
            controlsText.text = ControlsHint;

        ShowMessage("Step 1: Left-click and drag the BOWL onto the glowing prep zone on the table.");
    }

    public void ShowMessage(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;

        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), 4f);
    }

    private void ClearMessage()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
