using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FishMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float turnSpeed = 4f;
    public float reachDistance = 1.2f;   // ⭐ 到达判定放大

    [Header("Vertical Movement")]
    public float verticalSpeed = 0.6f;   // ⭐ 上下变化速度
    public float verticalLerp = 2f;       // ⭐ 高度平滑

    [Header("Bounds")]
    public Vector3 minBounds = new Vector3(-6, 1.2f, -6);
    public Vector3 maxBounds = new Vector3(6, 3.5f, 6);

    [Header("Separation")]
    public float separationRadius = 0.8f;
    public float separationForce = 1.2f;
    public float minSeparationDistance = 0.5f; // 硬最小距离

    private Vector3 targetPos;
    private float targetY;                // 独立的目标高度
    private bool movable = true;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        if (!movable) return;

        MoveToTarget();
        ApplySeparation();
        KeepInBounds();
    }

    // =======================
    // 核心移动逻辑
    // =======================
    void MoveToTarget()
    {
        // -------- XZ 平面方向 --------
        Vector3 flatTarget = new Vector3(
            targetPos.x,
            transform.position.y,
            targetPos.z
        );

        Vector3 dir = flatTarget - transform.position;

        if (dir.magnitude < reachDistance)
        {
            PickNewTarget();
            return;
        }

        // 平滑转向（只在 XZ）
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );

        // 前进
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // -------- Y 轴上下浮动 --------
        float newY = Mathf.Lerp(
            transform.position.y,
            targetY,
            verticalLerp * Time.deltaTime
        );

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }

    // =======================
    // 随机目标点（范围更大）
    // =======================
    void PickNewTarget()
    {
        targetPos = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            0f,
            Random.Range(minBounds.z, maxBounds.z)
        );

        // ⭐ 单独生成目标高度
        targetY = Random.Range(minBounds.y, maxBounds.y);
    }

    // =======================
    // 防重叠（轻量）
    // =======================
    void ApplySeparation()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            separationRadius
        );

        Vector3 correction = Vector3.zero;
        int count = 0;

        foreach (var h in hits)
        {
            if (h.transform == transform) continue;
            if (!h.CompareTag("Fish")) continue;

            Vector3 diff = transform.position - h.transform.position;
            float dist = diff.magnitude;

            if (dist < 0.001f) continue;

            // ⭐ 强制最小距离修正
            if (dist < minSeparationDistance)
            {
                float pushAmount = (minSeparationDistance - dist);
                correction += diff.normalized * pushAmount;
                count++;
            }
            else
            {
                // 原来的柔性分离
                correction += diff.normalized / dist;
                count++;
            }
        }

        if (count > 0)
        {
            correction /= count;
            transform.position += correction * separationForce * Time.deltaTime;
        }
    }


    // =======================
    // 边界限制
    // =======================
    void KeepInBounds()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);

        transform.position = pos;
    }

    // =======================
    // 外部控制
    // =======================
    public void SetMovable(bool value)
    {
        movable = value;
    }
}
