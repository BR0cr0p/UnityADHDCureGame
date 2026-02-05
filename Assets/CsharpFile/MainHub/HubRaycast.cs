using UnityEngine;

public class HubRaycast : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 先尝试进入游戏
            GameEntryPoint entry = hit.collider.GetComponent<GameEntryPoint>();
            if (entry != null)
            {
                entry.EnterGame();
                return;
            }

            // 再尝试退出
            ExitGame3D exit = hit.collider.GetComponent<ExitGame3D>();
            if (exit != null)
            {
                exit.Exit();
            }
        }
    }
}
