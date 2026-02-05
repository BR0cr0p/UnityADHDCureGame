using UnityEngine;

public class CMCameraAligner : MonoBehaviour
{
    [Header("Target")]
    public Transform gridCenter;

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 0f, -5f);

    [Header("Look At")]
    public Vector3 lookOffset = Vector3.zero;

    [Header("Settings")]
    public bool alignOnStart = true;

    void Start()
    {
        if (alignOnStart)
        {
            AlignCamera();
        }
    }

    public void AlignCamera()
    {
        if (gridCenter == null)
        {
            Debug.LogWarning("GameCameraAligner: gridCenter not set!");
            return;
        }

        transform.position = gridCenter.position + offset;
        transform.LookAt(gridCenter.position + lookOffset);
    }
}
