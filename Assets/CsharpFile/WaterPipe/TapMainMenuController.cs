using UnityEngine;

public enum TapDifficulty
{
    Easy,
    Medium,
    Hard
}
public enum TapGameState
{
    Menu,
    DifficultySelect,
    Playing,
    Paused,
    Result
}

public class TapMainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject difficultyPanel;

    [Header("Game Manager")]
    public TrialManager trialManager;

    [Header("Pause & Result")]
    public TapPauseController pauseController;
    public TapResultController resultController;

    [HideInInspector] public TapGameState currentState = TapGameState.Menu;

    void Start()
    {
        currentState = TapGameState.Menu;

        if (menuPanel != null) menuPanel.SetActive(true);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);

        if (pauseController != null && pauseController.pausePanel != null)
            pauseController.pausePanel.SetActive(false);

        if (resultController != null && resultController.resultPanel != null)
            resultController.resultPanel.SetActive(false);

        // 初始禁用水管输入
        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null)
            pipe.enableInput = false;
    }

    // 点击 Play 打开难度选择
    public void OnPlayClicked()
    {
        currentState = TapGameState.DifficultySelect;

        if (menuPanel != null) menuPanel.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(true);

        // 禁用水管输入
        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null)
            pipe.enableInput = false;

        // 鼠标可见用于选择难度
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 点击退出
    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 选择难度
    public void OnSelectDifficulty(int difficultyIndex)
    {
        currentState = TapGameState.Playing;

        if (menuPanel != null) menuPanel.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);

        TapDifficulty difficulty = (TapDifficulty)difficultyIndex;

        if (trialManager != null)
            trialManager.StartLevel(difficulty);

        // 启用水管输入
        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null)
            pipe.enableInput = true;

        // 游戏中鼠标隐藏
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        if (pauseController != null)
            pauseController.EnablePauseInput();

        Debug.Log($"{difficulty} Mode Started!");
    }

    // 从 Pause 或 Result 返回难度选择
    public void ShowDifficultyPanel()
    {
        currentState = TapGameState.DifficultySelect;

        if (menuPanel != null) menuPanel.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(true);

        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null)
            pipe.enableInput = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

