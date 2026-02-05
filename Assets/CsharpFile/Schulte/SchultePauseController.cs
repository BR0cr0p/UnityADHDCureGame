using UnityEngine;
using System.Collections;

public class SchultePauseController : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject menuCanvas;       // 主菜单 Canvas
    public SchulteGridGenerator gridGenerator;
    public GameObject targetIndicatorCanvas; // 拖入 TargetIndicator Canvas


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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseCanvas != null) pauseCanvas.SetActive(true);

        SchulteController controller = FindObjectOfType<SchulteController>();
        if (controller != null)
            controller.DisableInput();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pauseCanvas != null) pauseCanvas.SetActive(false);

        StartCoroutine(EnableInputNextFrame());
    }

    private void ResetTargetIndicator()
    {
        if (targetIndicatorCanvas == null) return;

        TargetIndicator indicator =
            targetIndicatorCanvas.GetComponent<TargetIndicator>();

        if (indicator != null)
            indicator.ResetIndicator();
    }


    public void GoToDifficultySelect()
    {
        // 不要调用 ResumeGame
        Time.timeScale = 1f;
        isPaused = false;

        // 菜单态鼠标：显示 + 解锁
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        // 禁用暂停输入
        DisablePauseInput();

        // 禁用游戏输入（非常重要）
        SchulteController controller = FindObjectOfType<SchulteController>();
        if (controller != null)
            controller.DisableInput();

        // 切回主菜单
        SchulteMainMenuController menuController =
            FindObjectOfType<SchulteMainMenuController>();

        if (menuController != null)
            menuController.ReturnToDifficultyFromGame();
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

        SchulteController controller = FindObjectOfType<SchulteController>();
        if (controller != null)
            controller.EnableInput();
    }
}
