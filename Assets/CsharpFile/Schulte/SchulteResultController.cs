using UnityEngine;
using TMPro;

public class SchulteResultController : MonoBehaviour
{
    public GameObject resultCanvas;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI correctRateText;

    private void Awake()
    {
        SchulteController controller = FindObjectOfType<SchulteController>();
        if (controller != null)
        {
            controller.OnGameFinished += OnGameFinished;
        }
    }
    private void Start()
    {
        resultCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        SchulteController controller = FindObjectOfType<SchulteController>();
        if (controller != null)
        {
            controller.OnGameFinished -= OnGameFinished;
        }
    }

    private void OnGameFinished()
    {
        // 停止计时
        GameTimer timer = FindObjectOfType<GameTimer>();
        if (timer != null)
        {
            timer.StopTimer();
            timeText.text = $"Time: {timer.CurrentTime:F2}s";
        }

        //显示正确率
        SchulteController controller = FindObjectOfType<SchulteController>();
        if (controller != null && correctRateText != null)
        {
            float rate = 0f;
            if (controller.totalClicks > 0)
                rate = (float)controller.correctClicks / controller.totalClicks * 100f;

            correctRateText.text = $"Correct Rate: {rate:F1}%";
        }

        //隐藏next指示器
        SchulteMainMenuController menu = FindObjectOfType<SchulteMainMenuController>();
        if (menu != null && menu.targetIndicatorCanvas != null)
            menu.targetIndicatorCanvas.SetActive(false);

        // 显示结算
        resultCanvas.SetActive(true);

        // 禁用暂停
        SchultePauseController pause = FindObjectOfType<SchultePauseController>();
        if (pause != null)
            pause.DisablePauseInput();

        //测试
        Debug.Log("Game Finished → Show Result");

    }

    // 按钮用
    public void BackToDifficulty()
    {
        resultCanvas.SetActive(false);

        SchulteMainMenuController menu = FindObjectOfType<SchulteMainMenuController>();
        if (menu != null)
            menu.ReturnToDifficultyFromGame();
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
