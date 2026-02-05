using UnityEngine;

public class FishPauseController : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseCanvas;

    [Header("Player Control")]
    public CameraLook cameraLook;
    public MouseCatch mouseCatch;

    [Header("Menu")]
    public FishMainMenuController mainMenu;

    private bool isPaused = false;
    private bool escEnabled = false;

    void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
    }

    void Update()
    {
        if (!escEnabled) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // =======================
    // 外部控制
    // =======================
    public void EnablePauseInput()
    {
        escEnabled = true;
    }

    public void DisablePauseInput()
    {
        escEnabled = false;

        if (isPaused)
            ResumeGame();
    }

    // =======================
    // 暂停 / 恢复
    // =======================
    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);

        // 禁用玩家输入
        if (cameraLook != null)
        {
            cameraLook.enabled = false;
        }

        if (mouseCatch != null)
            mouseCatch.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        // 恢复玩家输入（注意这里不重新 Init）
        if (cameraLook != null)
            cameraLook.enabled = true;

        if (mouseCatch != null)
            mouseCatch.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // =======================
    // UI 按钮
    // =======================
    public void OnClickResume()
    {
        ResumeGame();
    }

    public void OnClickReturnToDifficulty()
    {
        ResumeGame();
        DisablePauseInput();

        if (mainMenu != null)
            mainMenu.ReturnToDifficulty();
    }

    public void OnClickQuit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
