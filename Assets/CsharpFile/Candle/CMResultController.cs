using UnityEngine;

public class CMResultController : MonoBehaviour
{
    public GameObject resultCanvas;

    private void Awake()
    {
        CMController controller = FindObjectOfType<CMController>();
        if (controller != null)
        {
            controller.OnGameFinished += OnGameFinished;
        }
    }

    private void Start()
    {
        if (resultCanvas != null)
            resultCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        CMController controller = FindObjectOfType<CMController>();
        if (controller != null)
        {
            controller.OnGameFinished -= OnGameFinished;
        }
    }

    private void OnGameFinished()
    {
        // 显示结算界面
        if (resultCanvas != null)
            resultCanvas.SetActive(true);

        // 禁用暂停
        CMPauseController pause = FindObjectOfType<CMPauseController>();
        if (pause != null)
            pause.DisablePauseInput();

        Debug.Log("Game Finished → Show Result");
    }

    // ================= 按钮功能 =================

    public void BackToDifficulty()
    {
        if (resultCanvas != null)
            resultCanvas.SetActive(false);

        CMMainMenuController menu = FindObjectOfType<CMMainMenuController>();
        if (menu != null)
            menu.ReturnToLevelSelectFromGame();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
