using UnityEngine;
using System.Collections;

public class CMPauseController : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject menuCanvas;       // 主菜单 Canvas
    public CMGenerator gridGenerator;


    private bool isPaused = false;
    private bool escEnabled = false;


    void Update()
    {
        if (!escEnabled) return;

        // 按 Esc 或 P 暂停/恢复
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void EnablePauseInput()
    {
        escEnabled = true;
    }

    public void DisablePauseInput()
    {
        escEnabled = false;

        // 保险：禁用时一定恢复状态
        if (isPaused)
            ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseCanvas != null) pauseCanvas.SetActive(true);

        CMController controller = FindObjectOfType<CMController>();
        if (controller != null)
            controller.DisableInput();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseCanvas != null) pauseCanvas.SetActive(false);

        StartCoroutine(EnableInputNextFrame());
    }

    public void ReturnToMainMenu()
    {
        ResumeGame(); // 恢复时间
        DisablePauseInput();

        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);            
        }

        // 确保主菜单面板显示
        CMMainMenuController menuController = FindObjectOfType<CMMainMenuController>();
        if (menuController != null)
            menuController.menuPanel.SetActive(true);

        if (gridGenerator != null)
        {
            gridGenerator.ClearGrid();
            gridGenerator.gameObject.SetActive(false);
        }

    }

    public void GoToDifficultySelect()
    {
        ResumeGame(); // 恢复时间 & 关闭暂停界面

        CMPauseController pause = FindObjectOfType<CMPauseController>();
        if (pause != null)
            pause.DisablePauseInput();

        CMMainMenuController menuController = FindObjectOfType<CMMainMenuController>();
        if (menuController != null)
        {
            menuController.ReturnToLevelSelectFromGame();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit from Main Menu");

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }

    IEnumerator EnableInputNextFrame()
    {
        yield return null; // ⭐ 等一帧，吃掉 Resume 的点击

        CMController controller = FindObjectOfType<CMController>();
        if (controller != null)
            controller.EnableInput();
    }
}
