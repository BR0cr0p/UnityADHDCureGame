using UnityEngine;

public class ArrowFloat : MonoBehaviour
{
    public float amplitude = 0.3f;
    public float speed = 2f;

    Vector3 startPos;

    void OnEnable()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = startPos + Vector3.up * y;
    }
}
