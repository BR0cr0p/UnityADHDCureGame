using UnityEngine;

public class BackToHubButton : MonoBehaviour
{
    public void Back()
    {
        if (GameEntryManager.Instance == null)
        {
            Debug.LogError("GameEntryManager not found!");
            return;
        }

        GameEntryManager.Instance.BackToHub();
    }
}
