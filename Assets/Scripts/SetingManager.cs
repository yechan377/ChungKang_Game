using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetingManager : MonoBehaviour
{
    public GameManager gameManager;
    public InputManager inputManager;
    public CharacterMover characterMover;
    public PathManager pathManager;

    private static SetingManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene("MenuScene");
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MenuScene":
                BindButton("StartButton", gameManager.StartFromBeginning);
                BindButton("QuitButton", gameManager.QuitGame);
                break;
            case "FailScene":
                BindButton("RetryButton", gameManager.RetryCurrentStage);
                BindButton("MenuButton", gameManager.ReturnToMenu);
                break;
            case "ClearScene":
                BindButton("NextButton", gameManager.GoToNextStageOrEnd);
                BindButton("MenuButton", gameManager.ReturnToMenu);
                if (GameSession.currentStageIndex >= GameSession.stageScenes.Length - 1)
                {
                    GameObject nextBtn = GameObject.Find("NextButton");
                    if (nextBtn != null) nextBtn.SetActive(false);
                }
                break;
            case "Stage1Scene":
            case "Stage2Scene":
            case "Stage3Scene":
                RebindStageReferences();
                BindButton("MoveButton", characterMover.OnPlayButtonPressed);
                BindButton("ResetButton", inputManager.ResetDrawing);
                break;
        }
    }

    void RebindStageReferences()
    {
        gameManager.EnterStage();

        GameObject startObj = GameObject.Find("StartPoint");
        if (startObj != null) gameManager.startPoint = startObj.transform;

        GameObject goalObj = GameObject.Find("GoalPoint");
        if (goalObj != null) gameManager.goalPoint = goalObj.transform;

        GameObject characterObj = GameObject.Find("Character");
        if (characterObj != null)
        {
            characterMover.SetCharacter(characterObj.transform);

            CharacterCollision collision = characterObj.GetComponent<CharacterCollision>();
            if (collision != null) collision.gameManager = gameManager;
        }

        GameObject segmentParentObj = GameObject.Find("SegmentParent");
        if (segmentParentObj != null) pathManager.segmentParent = segmentParentObj.transform;

        GameObject curveDrawerObj = GameObject.Find("CurveDrawer");
        if (curveDrawerObj != null) pathManager.curveSegmentPrefab = curveDrawerObj;

        GameObject inkObj = GameObject.Find("Ink");
        InkDisplay inkDisplay = null;
        if (inkObj != null)
        {
            inkDisplay = inkObj.GetComponent<InkDisplay>();
        }

        GameObject debugConsoleObj = GameObject.Find("DebugConsole");
        if (debugConsoleObj != null)
        {
            DebugConsole debugConsole = debugConsoleObj.GetComponent<DebugConsole>();
            if (debugConsole != null)
            {
                debugConsole.gameManager = gameManager;
                debugConsole.pathManager = pathManager;
            }
        }

        pathManager.ClearAll();

        if (inkDisplay != null)
        {
            inkDisplay.pathManager = pathManager;
            inkDisplay.Initialize();
        }
    }

    void BindButton(string objectName, UnityEngine.Events.UnityAction action)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null) return;

        Button button = obj.GetComponent<Button>();
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}