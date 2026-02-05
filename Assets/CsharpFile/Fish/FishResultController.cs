using UnityEngine;
using TMPro;

public class FishResultController : MonoBehaviour
{
    [Header("UI")]
    public GameObject resultCanvas;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI accuracyText;

    private FishGameController game;

    void Awake()
    {
        game = FindObjectOfType<FishGameController>();
        if (game != null)
        {
            game.OnGameFinished += OnGameFinished;
        }
    }

    void Start()
    {
        if (resultCanvas != null)
            resultCanvas.SetActive(false);
    }

    void OnDestroy()
    {
        if (game != null)
        {
            game.OnGameFinished -= OnGameFinished;
        }
    }

    // =======================
    // 游戏完成回调
    // =======================
    void OnGameFinished(
        float totalTime,
        float accuracy,
        int correctCount,
        int totalClick
    )
    {
        // 显示结果
        if (timeText != null)
            timeText.text = $"Time: {totalTime:F2}s";

        if (accuracyText != null)
            accuracyText.text = $"Accuracy: {accuracy:F1}%";

        if (resultCanvas != null)
            resultCanvas.SetActive(true);

        // 禁用暂停
        FishPauseController pause = FindObjectOfType<FishPauseController>();
        if (pause != null)
            pause.DisablePauseInput();

        // 解锁鼠标，方便点 UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Fish Result → Show Result UI");
    }

    // =======================
    // UI 按钮
    // =======================
    public void OnBackClicked()
    {
        if (resultCanvas != null)
            resultCanvas.SetActive(false);

        FishMainMenuController menu =
            FindObjectOfType<FishMainMenuController>();

        if (menu != null)
            menu.ReturnToDifficulty();
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
