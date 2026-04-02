using UnityEngine;

public class TapPauseController : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isPaused = false;
    private bool escEnabled = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (!escEnabled) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void EnablePauseInput() => escEnabled = true;
    public void DisablePauseInput() => escEnabled = false;

    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    public void BackToDifficulty()
    {
        isPaused = false;
        Time.timeScale = 1f;
        escEnabled = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        TrialManager trial = FindObjectOfType<TrialManager>();
        if (trial != null)
            trial.StopCurrentGame();

        TapMainMenuController menu = FindObjectOfType<TapMainMenuController>();
        if (menu != null)
            menu.ShowDifficultyPanel();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
