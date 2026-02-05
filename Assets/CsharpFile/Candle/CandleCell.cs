using System.Collections;
using UnityEngine;
using TMPro;

public class CandleCell : MonoBehaviour
{
    public bool isDead = false;
    public bool isLit = false;

    public bool IsDead => isDead;
    public bool IsLit => isLit;

    int number;
    public int Number => number;

    [Header("Candle")]
    public GameObject flameObject;
    public Light flameLight;

    [Header("Number")]
    public TextMeshPro numberText;
    public float fontScale = 0.6f;
    public float minFontSize = 3f;
    public float maxFontSize = 3f;

    [Header("Number Colors")]
    public Color normalColor = Color.black;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;


    void Awake()
    {
        // prefab 防污染重置
        isDead = false;
        isLit = false;

        if (numberText == null)
            numberText = GetComponentInChildren<TextMeshPro>(true);

        if (numberText != null)
            numberText.color = normalColor;

        ApplyFlame(false);
    }

    // ================= 点击 =================
    public void OnClicked()
    {
        if (isDead) return;

        CMController controller = FindObjectOfType<CMController>();
        if (controller == null || !controller.inputEnabled) return;

        controller.OnCellClicked(this);
    }

    // ================= Controller =================
    public void RegisterCorrect()
    {
        if (isDead) return;

        isDead = true;
        SetLit(true, true);

        if (numberText != null)
            numberText.color = correctColor;
    }

    public void RegisterWrong()
    {
        if (isDead) return;
        StartCoroutine(WrongFlash());
    }

    // 演示用
    public void SetLit(bool on, bool force = false)
    {
        if (isDead && !force) return;
        isLit = on;
        ApplyFlame(on);
    }

    // ================= 火焰 =================
    void ApplyFlame(bool on)
    {
        if (flameObject != null)
            flameObject.SetActive(on);

        if (flameLight != null)
            flameLight.enabled = on;
    }

    IEnumerator WrongFlash()
    {
        if (numberText != null)
            numberText.color = wrongColor;

        flameObject.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        flameObject.SetActive(false);

        if (!isDead && numberText != null)
            numberText.color = normalColor;
    }

    // ================= UI =================
    public void SetSize(float cellSize)
    {
        transform.localScale = Vector3.one * cellSize;

        if (numberText != null)
        {
            float size = cellSize * fontScale * 10f;
            numberText.fontSize = Mathf.Clamp(size, minFontSize, maxFontSize);
        }
    }

    public void SetNumber(int num)
    {
        number = num;
        if (numberText != null)
            numberText.text = number.ToString();
    }

    public void ResetCandle()
    {
        isDead = false;
        SetLit(false, true);
        if (numberText != null)
            numberText.color = normalColor;
    }
}
