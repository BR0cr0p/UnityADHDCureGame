using UnityEngine;

public class FishMainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject difficultyPanel;

    [Header("Player Control")]
    public CameraLook cameraLook;
    public MouseCatch mouseCatch;

    [Header("Game")]
    public FishGameController fishGame;

    void Start()
    {
        menuPanel.SetActive(true);
        difficultyPanel.SetActive(false);
        DisableGameInput();

        cameraLook.enabled = false;
        mouseCatch.enabled = false;
    }

    public void OnPlayClicked()
    {
        menuPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnSelectEasy() => StartGame(fishGame.easy);
    public void OnSelectMedium() => StartGame(fishGame.medium);
    public void OnSelectHard() => StartGame(fishGame.hard);

    private void StartGame(FishGameController.DifficultySetting difficulty)
    {
        difficultyPanel.SetActive(false);

        fishGame.StartGame(difficulty);

        EnableGameInput();

        cameraLook.enabled = true;
        cameraLook.InitLookAtPool();

        mouseCatch.enabled = true;

        FishPauseController pause = FindObjectOfType<FishPauseController>();
        if (pause != null)
            pause.EnablePauseInput();
    }

    public void ReturnToDifficulty()
    {
        DisableGameInput();

        cameraLook.DisableLook();
        cameraLook.enabled = false;
        mouseCatch.enabled = false;

        menuPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    void EnableGameInput()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void DisableGameInput()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
