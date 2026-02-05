using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapCameraSwitcher : MonoBehaviour
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
