using System.Collections;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    [Header("Water Settings")]
    public float water = 0f;
    public float maxWater = 100f;
    [HideInInspector] public int totalClicks = 6; // 自动根据难度设置

    [Header("Visual")]
    public Transform waterMesh;
    public Transform bucketMouth;

    float maxHeight;
    float increment;
    Coroutine fillRoutine;

    [HideInInspector] public bool canShowWater = true; // 是否显示动画

    void Start()
    {
        maxHeight = bucketMouth.localPosition.y;
        RecalculateIncrement();
        SetWaterVisual(0f);
    }

    public void RecalculateIncrement()
    {
        increment = maxWater / Mathf.Max(totalClicks, 1);
    }

    public void AddWater(float amount, float duration = 0.5f)
    {
        RecalculateIncrement();
        float targetWater = Mathf.Clamp(water + amount, 0f, maxWater);

        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        if (canShowWater)
            fillRoutine = StartCoroutine(FillWaterSmooth(targetWater, duration));
        else
            water = targetWater; // 不显示动画
    }

    /// <summary>
    /// 正常接水调用
    /// </summary>
    public void AddWaterOnce(float duration = 0.5f)
    {
        AddWater(increment, duration);
    }

    /// <summary>
    /// 扣水专用
    /// </summary>
    public void SubtractWater(float amount, float duration = 0.5f)
    {
        AddWater(-Mathf.Abs(amount), duration);
    }


    IEnumerator FillWaterSmooth(float target, float duration)
    {
        float startWater = water;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(duration, 0.01f);
            water = Mathf.Lerp(startWater, target, t);
            SetWaterVisual(water / maxWater);
            yield return null;
        }

        water = target;
        SetWaterVisual(water / maxWater);
        fillRoutine = null;
    }

    void SetWaterVisual(float t)
    {
        t = Mathf.Clamp01(t);
        if (waterMesh == null) return;

        Vector3 scale = waterMesh.localScale;
        scale.y = Mathf.Lerp(0.001f, maxHeight, t);
        scale.y = Mathf.Min(scale.y, maxHeight);
        waterMesh.localScale = scale;
    }

    public void ResetWater(bool showWater = true)
    {
        if (fillRoutine != null)
        {
            StopCoroutine(fillRoutine);
            fillRoutine = null;
        }

        water = 0f;
        SetWaterVisual(0f);
        canShowWater = showWater;
    }

    public void ShowTotalWater(float duration = 0.5f)
    {
        canShowWater = true;
        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        fillRoutine = StartCoroutine(FillWaterSmooth(water, duration));
    }

    public bool IsFull => water >= maxWater;
}
