using UnityEngine.SceneManagement;

public static class GameSession
{
    public static readonly string[] stageScenes = { "Stage1Scene", "Stage2Scene", "Stage3Scene" };
    public static int currentStageIndex = 0;

    public static string GetCurrentStageScene()
    {
        return stageScenes[currentStageIndex];
    }

    public static void GoToNextStageOrEnd()
    {
        currentStageIndex++;
        if (currentStageIndex < stageScenes.Length)
        {
            SceneManager.LoadScene(stageScenes[currentStageIndex]);
        }
        else
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    public static void RetryCurrentStage()
    {
        SceneManager.LoadScene(stageScenes[currentStageIndex]);
    }

    public static void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public static void StartFromBeginning()
    {
        currentStageIndex = 0;
        SceneManager.LoadScene(stageScenes[currentStageIndex]);
    }
}