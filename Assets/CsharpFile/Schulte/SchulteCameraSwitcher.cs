using UnityEngine;

public class SchulteCameraSwitcher : MonoBehaviour
{
    public Camera sceneCamera;
    public Camera gameCamera;

    void Start()
    {
        ShowSceneView();
    }

    public void ShowSceneView()
    {
        sceneCamera.gameObject.SetActive(true);
        gameCamera.gameObject.SetActive(false);
    }

    public void SwitchToGameView()
    {
        sceneCamera.gameObject.SetActive(false);
        gameCamera.gameObject.SetActive(true);
    }
}
