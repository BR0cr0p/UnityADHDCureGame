using UnityEngine;

public class PipeHeadSensor : MonoBehaviour
{
    [HideInInspector] public TapController activeTap;
    [HideInInspector] public Bucket bucket;
    [HideInInspector] public float sandPenalty = 0.5f;

    public bool hasCaptured = false; // 每轮第一次接触有效

    void OnTriggerEnter(Collider other)
    {
        if (hasCaptured) return;

        TapController tap = other.GetComponentInParent<TapController>();
        if (tap == null) return;

        if (tap.isWater)
        {
            bucket.AddWaterOnce();
            Debug.Log("Correct water!");
            hasCaptured = true;
        }
        else if (tap.isSand)
        {
            bucket.SubtractWater(sandPenalty);
            Debug.Log("Hit sand!");
            hasCaptured = true;
        }
    }


    public void ResetCapture()
    {
        hasCaptured = false;
    }
}
