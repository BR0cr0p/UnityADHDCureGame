using UnityEngine;

public class ExitGame3D : MonoBehaviour
{
    public void Exit()
    {
        if (GameEntryManager.Instance == null)
        {
            Debug.LogError("GameEntryManager not found!");
            return;
        }

        GameEntryManager.Instance.QuitGame();
    }
}
