using UnityEngine;

public class TapResultController : MonoBehaviour
{
    public GameObject resultPanel;
    public TapPauseController pauseController;

    void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    // 显示结算界面
    public void ShowResult()
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);

        // 禁用暂停
        if (pauseController != null)
            pauseController.DisablePauseInput();

        // 禁用水管输入
        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null)
            pipe.enableInput = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Show Result UI");
    }

    // 点击返回菜单
    public void BackToMenu()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        // 显示主菜单 / 难度选择
        TapMainMenuController menu = FindObjectOfType<TapMainMenuController>();
        if (menu != null)
        {
            menu.ShowDifficultyPanel(); // 或者menuPanel.SetActive(true)
        }

        // 停止游戏逻辑
        TrialManager trial = FindObjectOfType<TrialManager>();
        if (trial != null)
        {
            trial.StopAllCoroutines(); // 停止当前水流循环
        }

        // 禁用水管输入，防止鼠标乱动
        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null)
            pipe.enableInput = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Back to Menu from Result");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
