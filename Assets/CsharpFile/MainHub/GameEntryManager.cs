using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntryManager : MonoBehaviour
{
    public static GameEntryManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 跨场景存在（关键）
    }

    // 进入任意小游戏
    public void EnterScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // 返回主枢纽
    public void BackToHub()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainHub");
    }

    // 真正退出程序（唯一出口）
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
