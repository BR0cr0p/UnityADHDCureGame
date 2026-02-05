using UnityEngine;

public class MouseCatch : MonoBehaviour
{
    public float catchDistance = 20f;
    public LayerMask fishLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryCatch();
        }
    }

    void TryCatch()
    {
        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, catchDistance, fishLayer))
        {
            Debug.Log("中心 Ray 命中：" + hit.collider.name);

            Fish fish = hit.collider.GetComponent<Fish>();
            if (fish != null)
            {
                fish.OnCatch();
            }
        }
        else
        {
            Debug.Log("中心 Ray 未命中");
        }
    }
}
