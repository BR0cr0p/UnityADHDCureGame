using UnityEngine;

public class TapController : MonoBehaviour
{
    public ParticleSystem waterFX;
    public ParticleSystem sandFX;
    public TapFlasher flasher;

    public bool isWater;
    public bool isSand;

    public void SetIdle()
    {
        isWater = false;
        isSand = false;

        flasher?.Deactivate();
        waterFX?.Stop();
        sandFX?.Stop();
    }

    public void SetWater()
    {
        SetIdle();
        isWater = true;

        flasher?.Activate();
        waterFX?.Play();
    }

    public void SetSand()
    {
        SetIdle();
        isSand = true;

        sandFX?.Play();
    }
}
