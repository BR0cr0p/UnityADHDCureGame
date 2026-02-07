using UnityEngine;

public class TapCameraSwitcher : MonoBehaviour
{
    public Camera sceneCamera;  // 编辑器/主菜单摄像头
    public Camera gameCamera;   // 游戏摄像头

    void Start()
    {
        ShowSceneView();
    }

    public void ShowSceneView()
    {
        if (sceneCamera != null) sceneCamera.gameObject.SetActive(true);
        if (gameCamera != null) gameCamera.gameObject.SetActive(false);
    }

    public void SwitchToGameView()
    {
        if (sceneCamera != null) sceneCamera.gameObject.SetActive(false);
        if (gameCamera != null) gameCamera.gameObject.SetActive(true);
    }
}
