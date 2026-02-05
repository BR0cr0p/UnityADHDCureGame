using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [Header("Target Scene Name")]
    public string sceneName;

    public void EnterGame()
    {
        if (GameEntryManager.Instance == null)
        {
            Debug.LogError("GameEntryManager not found!");
            return;
        }

        GameEntryManager.Instance.EnterScene(sceneName);
    }
}
