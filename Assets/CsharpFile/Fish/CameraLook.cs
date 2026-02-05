using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("鼠标控制参数")]
    public float sensitivity = 120f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("自动对准目标")]
    public Transform poolCenter;

    private float pitch;
    private float yaw;
    private bool initialized = false;

    // 不在 Start 做任何事情
    void Update()
    {
        if (!initialized) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // 只在“真正开始游戏”时调用
    public void InitLookAtPool()
    {
        if (poolCenter == null) return;

        transform.LookAt(poolCenter);

        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        initialized = true;
    }

    // 可选：退出游戏时调用
    public void DisableLook()
    {
        initialized = false;
    }
}
