using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    public GameManager gameManager;
    public PathManager pathManager;
    public GameObject panel;
    public TMP_Text debugText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (panel != null) panel.SetActive(!panel.activeSelf);
        }

        if (debugText == null || pathManager == null || gameManager == null) return;

        int segmentCount = pathManager.GetSegmentsInOrder().Count;
        float remainingInk = pathManager.GetRemainingInk();

        debugText.text =
            "State: " + gameManager.GetState() + "\n" +
            "Segments: " + segmentCount + "\n" +
            "Remaining Ink: " + remainingInk.ToString("F0") + "\n" +
            "Stage Index: " + GameSession.currentStageIndex;
    }

    public void OnSkipButtonPressed()
    {
        gameManager.GoToNextStageOrEnd();
    }
}