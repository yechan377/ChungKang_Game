using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Drawing, Playing, Cleared, Failed }

    public Transform startPoint;
    public Transform goalPoint;

    public float startTolerance = 0.3f;
    public float goalTolerance = 0.3f;

    private GameState currentState = GameState.Drawing;

    public GameState GetState()
    {
        return currentState;
    }

    public void EnterStage()
    {
        currentState = GameState.Drawing;
    }

    public bool IsNearStart(Vector3 point)
    {
        return Vector3.Distance(startPoint.position, point) <= startTolerance;
    }

    public bool IsNearGoal(Vector3 point)
    {
        return Vector3.Distance(goalPoint.position, point) <= goalTolerance;
    }

    public void StartPlaying()
    {
        if (currentState == GameState.Drawing)
        {
            currentState = GameState.Playing;
        }
    }

    public void ClearStage()
    {
        currentState = GameState.Cleared;
        SceneManager.LoadScene("ClearScene");
    }

    public void FailStage()
    {
        currentState = GameState.Failed;
        SceneManager.LoadScene("FailScene");
    }

    public void StartFromBeginning()
    {
        GameSession.StartFromBeginning();
    }

    public void RetryCurrentStage()
    {
        GameSession.RetryCurrentStage();
    }

    public void ReturnToMenu()
    {
        GameSession.ReturnToMenu();
    }

    public void GoToNextStageOrEnd()
    {
        GameSession.GoToNextStageOrEnd();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}