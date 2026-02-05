using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Hose : MonoBehaviour
{
    [Header("Pipe Points")]
    public Transform pipeHead;
    public Transform pipeBase;
    public int segments = 30;

    [Header("Shape Control")]
    [Range(0f, 1f)]
    public float normalStiffness = 0.35f;
    [Range(0f, 1f)]
    public float flowStiffness = 0.2f;

    [Header("Gravity")]
    public float normalGravity = 1.5f;
    public float flowGravity = 2.0f;

    [Header("Velocity Control")]
    public float maxVelocity = 1.2f;
    [Range(0f, 1f)]
    public float damping = 0.92f;   // 核心稳定器（非常重要）

    [Header("Flow Visual Sway")]
    public float flowSwayStrength = 0.08f;
    public float swaySpeed = 0.5f;

    [Header("Line Width")]
    public float startWidth = 0.08f;
    public float endWidth = 0.05f;

    [Header("Ground Constraint")]
    public LayerMask groundMask;
    public float groundOffset = 0.02f;

    [HideInInspector]
    public bool isFlowing = false;

    LineRenderer line;
    Vector3[] pos;
    Vector3[] vel;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments;
        line.startWidth = startWidth;
        line.endWidth = endWidth;

        pos = new Vector3[segments];
        vel = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            pos[i] = Vector3.Lerp(
                pipeBase.position,
                pipeHead.position,
                i / (float)(segments - 1)
            );
            vel[i] = Vector3.zero;
        }
    }

    void Update()
    {
        // 固定两端
        pos[0] = pipeBase.position;
        pos[segments - 1] = pipeHead.position;

        float stiffness = isFlowing ? flowStiffness : normalStiffness;
        float gravity = isFlowing ? flowGravity : normalGravity;

        for (int i = 1; i < segments - 1; i++)
        {
            // ===== 重力 =====
            vel[i] += Vector3.down * gravity * Time.deltaTime;

            // ===== 平滑水流扰动（无随机跳变）=====
            if (isFlowing)
            {
                float noise = Mathf.PerlinNoise(
                    Time.time * swaySpeed,
                    i * 0.15f
                ) - 0.5f;

                vel[i] += Vector3.right * noise * flowSwayStrength;
            }

            // ===== 阻尼 & 限速（稳定核心）=====
            vel[i] *= damping;
            vel[i] = Vector3.ClampMagnitude(vel[i], maxVelocity);

            // ===== 积分 =====
            pos[i] += vel[i] * Time.deltaTime;

            // ===== 形态约束（保持水管顺滑）=====
            Vector3 target = (pos[i - 1] + pos[i + 1]) * 0.5f;
            pos[i] = Vector3.Lerp(pos[i], target, stiffness);

            // ===== 地面约束 =====
            if (Physics.Raycast(
                pos[i] + Vector3.up * 0.3f,
                Vector3.down,
                out RaycastHit hit,
                1f,
                groundMask))
            {
                float minY = hit.point.y + groundOffset;
                if (pos[i].y < minY)
                {
                    pos[i].y = minY;
                    vel[i].y = 0f;
                }
            }
        }

        // 更新 LineRenderer
        for (int i = 0; i < segments; i++)
        {
            line.SetPosition(i, pos[i]);
        }
    }
}
