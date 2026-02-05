using UnityEngine;

public class CMGameTimer : MonoBehaviour
{
    public float CurrentTime { get; private set; }
    public bool IsRunning { get; private set; }

    void Update()
    {
        if (IsRunning)
            CurrentTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        CurrentTime = 0f;
        IsRunning = true;
        Debug.Log("TIMER START");
    }

    public void StopTimer()
    {
        IsRunning = false;
    }
}
