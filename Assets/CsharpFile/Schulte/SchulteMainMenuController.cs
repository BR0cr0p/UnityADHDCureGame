using UnityEngine;

public class SchulteMainMenuController : MonoBehaviour
{
    public GameObject menuPanel;          // 第一阶段菜单
    public GameObject difficultyPanel;    // 第二阶段菜单
    public SchulteGridGenerator gridGenerator; // 方格生成器
    public GameObject targetIndicatorCanvas; // TargetIndicator指示器

    void Start()
    {
        menuPanel.SetActive(true); // 显示菜单
    }

    // Play 按钮点击
    public void OnPlayClicked()
    {
        menuPanel.SetActive(false);       // 隐藏主菜单
        difficultyPanel.SetActive(true);  // 显示难度选择
    }

    // Quit 按钮点击
    public void OnQuitClicked()
    {
        Debug.Log("Quit from Main Menu");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnSelectEasy()
    {
        StartGame(SchulteDifficulty.Easy_3x3);
    }

    public void OnSelectMedium()
    {
        StartGame(SchulteDifficulty.Medium_4x4);
    }

    public void OnSelectHard()
    {
        StartGame(SchulteDifficulty.Hard_5x5);
    }

    private void StartGame(SchulteDifficulty difficulty)
    {
        // 隐藏菜单界面
        menuPanel.SetActive(false);

        SchultePauseController pause = FindObjectOfType<SchultePauseController>();
        SchulteController controller = FindObjectOfType<SchulteController>();

        if (controller != null)
            controller.EnableInput();

        if (pause != null)
            pause.EnablePauseInput();

        // 隐藏难度选择界面
        if (difficultyPanel != null)
            difficultyPanel.SetActive(false);

        //Debug.Log("StartGame called, hiding DifficultyPanel");

        // 生成对应难度的方格
        if (gridGenerator != null)
            gridGenerator.SetDifficulty(difficulty);

        // 显示 TargetIndicator
        if (targetIndicatorCanvas != null)
            targetIndicatorCanvas.SetActive(true);

        // 启动计时
        GameTimer timer = FindObjectOfType<GameTimer>();
        if (timer != null)
            timer.StartTimer();
    }

    public void EndGame()
    {
        if (targetIndicatorCanvas != null)
            targetIndicatorCanvas.SetActive(false);
    }

    public void ShowDifficultyPanel()
    {
        menuPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void ReturnToDifficultyFromGame()
    {
        HideGame();
        ShowDifficulty();

        Debug.Log("Returned to Difficulty Selection");
    }

    private void HideGame()
    {
        if (gridGenerator != null)
        {
            gridGenerator.ClearGrid();
            gridGenerator.gameObject.SetActive(false);
        }

        if (targetIndicatorCanvas != null)
            targetIndicatorCanvas.SetActive(false);
    }

    private void ShowDifficulty()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (difficultyPanel != null)
            difficultyPanel.SetActive(true);
    }


}
