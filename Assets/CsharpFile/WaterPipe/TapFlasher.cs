using System.Collections;
using UnityEngine;

public class TapFlasher : MonoBehaviour
{
    [Header("Highlight")]
    public Renderer tapRenderer;

    public Color highlightColor = new Color(1f, 0.8f, 0.2f);
    public float blinkSpeed = 2.5f;

    Material tapMat;
    Coroutine blinkRoutine;

    void Awake()
    {
        if (tapRenderer != null)
        {
            tapMat = tapRenderer.material;
            tapMat.EnableKeyword("_EMISSION");
            tapMat.SetColor("_EmissionColor", Color.black);
        }
    }

    public void Activate()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(Blink());
    }

    public void Deactivate()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (tapMat != null)
            tapMat.SetColor("_EmissionColor", Color.black);

    }

    IEnumerator Blink()
    {
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * blinkSpeed;
            float intensity = (Mathf.Sin(t) + 1f) * 0.5f;

            Color emission =
                highlightColor * Mathf.LinearToGammaSpace(intensity * 2f);

            if (tapMat != null)
                tapMat.SetColor("_EmissionColor", emission);

            yield return null;
        }
    }
}
