using System.Collections;
using UnityEngine;
using TMPro;
using Ilumisoft.HealthSystem;

public class GridCell : MonoBehaviour
{
    public int number;

    private Health health;

    Coroutine regenCoroutine;

    [Header("Candle")]
    public GameObject flameObject;
    public Light flameLight;
    public bool IsDead => health != null && !health.IsAlive;


    [Header("Number")]
    public TextMeshPro numberText;
    public float fontScale = 0.6f;
    public float minFontSize = 3f;
    public float maxFontSize = 3f;

    [Header("Healthbar Control")]
    public GameObject healthbarObject;   // 拖你的血条 prefab
    public float showDuration = 3f;      // 显示 1 秒
    private Coroutine hideRoutine;

    [Header("Candle Health")]
    public int minHits = 1;
    public int maxHits = 3;

    [Header("State")]
    [SerializeField]
    private bool isActive = false;
    public bool IsActive => isActive;

    [Header("Highlight")]
    public Renderer candleRenderer;   // 蜡烛 MeshRenderer
    public Color highlightColor = new Color(1f, 0.8f, 0.2f); // 金色
    public float blinkSpeed = 2.5f;

    private Material candleMat;
    private Coroutine highlightCoroutine;

    void Awake()
    {

        if (numberText == null)
            numberText = GetComponentInChildren<TextMeshPro>(true);

        health = GetComponent<Health>();

        if (healthbarObject != null)
            healthbarObject.SetActive(false);

        if (health != null)
        {
            health.OnHealthEmpty += OnCandleDead;
        }

        if (candleRenderer != null)
        {
            candleMat = candleRenderer.material; // 实例化材质
            candleMat.EnableKeyword("_EMISSION");
            candleMat.SetColor("_EmissionColor", Color.black);
        }

        SetFlame(true);

        InitRandomHealth();
    }

    public void SetNumber(int num)
    {
        number = num;
        if (numberText != null)
            numberText.text = number.ToString();
    }

    public void SetSize(float cellSize)
    {
        transform.localScale = Vector3.one * cellSize;

        if (numberText != null)
        {
            float calculatedSize = cellSize * fontScale * 10f;
            numberText.fontSize = Mathf.Clamp(calculatedSize, minFontSize, maxFontSize);
        }
    }

    // ===============================
    // 被 SchulteController 调用
    // ===============================

    // 点对
    public void RegisterCorrect()
    {
        if (IsDead) return;

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        health.ApplyDamage(1);     // 扣 1 点血
        StartCoroutine(CorrectFlash());

        ShowHealthbarTemporarily();
    }

    // 点错
    public void RegisterWrong()
    {
        StartCoroutine(WrongFlash());
    }

    // ===============================
    // 火焰控制
    // ===============================

    void SetFlame(bool on)
    {
        if (flameObject != null)
            flameObject.SetActive(on);

        if (flameLight != null)
            flameLight.enabled = on;
    }

    void OnCandleDead()
    {
        Extinguish();
    }


    void Extinguish()
    {
        SetFlame(false);

        if (numberText != null)
            numberText.color = Color.green;
    }

    void ShowHealthbarTemporarily()
    {
        if (healthbarObject == null || health == null) return;

        healthbarObject.SetActive(true);

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideThenRegen());
    }

    void StartRegen()
    {
        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);

        regenCoroutine = StartCoroutine(RegenLoop());
    }

    void InitRandomHealth()
    {
        if (health == null) return;

        int hits = Random.Range(minHits, maxHits + 1);

        health.MaxHealth = hits;
        health.SetHealth(hits); // 一定要同时设当前血量
    }


    public void Activate()
    {
        if (IsDead) return;

        isActive = true;

        SetFlame(true);

        if (highlightCoroutine != null)
            StopCoroutine(highlightCoroutine);

        highlightCoroutine = StartCoroutine(HighlightBlink());
    }

    public void Deactivate()
    {
        isActive = false;

        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
        }

        if (candleMat != null)
            candleMat.SetColor("_EmissionColor", Color.black);
    }


    IEnumerator RegenLoop()
    {
        while (!IsDead && health.CurrentHealth < health.MaxHealth)
        {
            yield return new WaitForSeconds(3f);

            // 3 秒内如果被再次点击，就会被中断
            health.AddHealth(1);
        }

        regenCoroutine = null;
    }

    IEnumerator HideThenRegen()
    {
        yield return new WaitForSeconds(showDuration);

        healthbarObject.SetActive(false);

        if (!IsDead)
            StartRegen();
    }

    // ===============================
    // 动画
    // ===============================
    IEnumerator CorrectFlash()
    {
        if (IsDead) yield break;

        SetFlame(false);
        yield return new WaitForSeconds(0.1f);

        if (!IsDead)
            SetFlame(true);
    }


    IEnumerator WrongFlash()
    {
        if (IsDead) yield break;

        SetFlame(false);
        yield return new WaitForSeconds(0.1f);
        SetFlame(true);

        if (!IsDead)
            SetFlame(true);
    }

    IEnumerator HighlightBlink()
    {
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * blinkSpeed;
            float intensity = (Mathf.Sin(t) + 1f) * 0.5f; // 0~1

            Color emission =
                highlightColor * Mathf.LinearToGammaSpace(intensity * 2f);

            if (flameLight != null)
            {
                flameLight.intensity = Mathf.Lerp(0.8f, 1.6f, intensity);
                flameLight.color = highlightColor;
            }

            if (candleMat != null)
                candleMat.SetColor("_EmissionColor", emission);

            yield return null;
        }
    }

}
