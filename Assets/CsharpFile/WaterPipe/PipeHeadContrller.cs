using UnityEngine;

public class PipeHeadController : MonoBehaviour
{
    public Transform movementPlane;   // 提供 Z 位置
    public float smooth = 5f;
    public Vector2 xLimit = new Vector2(-6f, 6f);
    public Vector2 yLimit = new Vector2(-4f, 4f);

    [HideInInspector] public bool enableInput = false; // 初始禁止移动

    Camera cam;
    Vector3 targetPos;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }

        if (cam == null || movementPlane == null)
        {
            Debug.LogError("Missing Camera or MovementPlane!");
            enabled = false;
            return;
        }

        targetPos = transform.position;
    }

    void Update()
    {
        if (!enableInput) return; // 游戏外阶段不响应鼠标

        UpdateTargetPosition();
        MoveSmooth();
    }

    void UpdateTargetPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, movementPlane.position.z));

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);

            // **加入灵敏度缩放**
            float sensitivity = 0.5f; // 0.1~1.0 调整灵敏度，越小越慢
            hit = Vector3.Lerp(transform.position, hit, sensitivity);

            hit.x = Mathf.Clamp(hit.x, xLimit.x, xLimit.y);
            hit.y = Mathf.Clamp(hit.y, yLimit.x, yLimit.y);
            hit.z = movementPlane.position.z;
            targetPos = hit;
        }
    }

    void MoveSmooth()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);
    }

    public void EnableInput()
    {
        enableInput = true;
        targetPos = transform.position; // 关键：同步当前位置
    }

}
