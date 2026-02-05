using UnityEngine;

public class CMMainMenuController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject levelPanel;         // 原 difficultyPanel 改成关卡面板
    public CMGenerator gridGenerator;

    void Start()
    {
        menuPanel.SetActive(true);
        if (levelPanel != null)
            levelPanel.SetActive(false);
    }

    // ================= 主菜单 =================

    public void OnPlayClicked()
    {
        menuPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
        Debug.Log("Quit from Main Menu");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ================= 关卡选择 =================

    public void OnSelectLevel1()
    {
        StartGame(CMLevel.Level1_2x2);
    }

    public void OnSelectLevel2()
    {
        StartGame(CMLevel.Level2_3x3);
    }

    private void StartGame(CMLevel level)
    {
        // 隐藏菜单
        menuPanel.SetActive(false);
        if (levelPanel != null)
            levelPanel.SetActive(false);

        CMPauseController pause = FindObjectOfType<CMPauseController>();
        if (pause != null)
            pause.EnablePauseInput();

        // 启用生成器（防止之前被隐藏）
        if (gridGenerator != null)
            gridGenerator.gameObject.SetActive(true);

        // 生成关卡
        if (gridGenerator != null)
            gridGenerator.SetLevel(level);
    }

    // ================= 返回流程 =================

    public void ReturnToLevelSelectFromGame()
    {
        HideGame();
        ShowLevelSelect();

        Debug.Log("Returned to Level Selection");
    }

    private void HideGame()
    {
        if (gridGenerator != null)
        {
            gridGenerator.ClearGrid();
            gridGenerator.gameObject.SetActive(false);
        }
    }

    private void ShowLevelSelect()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (levelPanel != null)
            levelPanel.SetActive(true);
    }
}
