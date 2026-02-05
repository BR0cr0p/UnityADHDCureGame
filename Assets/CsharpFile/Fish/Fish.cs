using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FishMove))]
public class Fish : MonoBehaviour
{
    [Header("状态控制")]
    public bool isTarget = false;       // 是否是目标鱼
    public bool canBeCaught = false;    // 是否可被点击捕捉

    private bool isCaught = false;
    private bool wrongClicked = false;

    private Renderer rend;
    private FishMove move;
    private float baseSpeed;
    private Color originalColor;

    // 捕捉事件
    public delegate void FishCaughtHandler(Fish fish);
    public event FishCaughtHandler OnFishCaughtEvent;

    [Header("高亮颜色")]
    public Color markColor = new Color(1f, 1f, 0f);         // 标记阶段高亮黄色
    public Color correctCatchColor = new Color(1f, 0.9f, 0.2f); // 捕捉正确金色
    public Color wrongCatchColor = Color.red;               // 点击错误红色

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            rend = GetComponentInChildren<SkinnedMeshRenderer>(); // 支持动画模型
        move = GetComponent<FishMove>();

        if (move != null)
            baseSpeed = move.moveSpeed;

        if (rend != null)
            originalColor = rend.material.color;  // 缓存初始颜色
    }


    // =======================
    // 状态控制
    // =======================

    /// <summary>
    /// 展示阶段标记目标鱼
    /// </summary>
    public void SetMarked(bool marked)
    {

        if (rend == null) return;

        if (marked)
        {
            // 高亮黄色标记，开启发光
            rend.material.color = markColor;

            if (rend.material.HasProperty("_EmissionColor"))
            {
                rend.material.EnableKeyword("_EMISSION");
                rend.material.SetColor("_EmissionColor", markColor * 2f); // 更亮眼
            }
        }
        else
        {
            // 恢复原本颜色
            rend.material.color = originalColor;
            if (rend.material.HasProperty("_EmissionColor"))
            {
                rend.material.SetColor("_EmissionColor", Color.black);
                rend.material.DisableKeyword("_EMISSION");
            }
        }
    }


    public void SetSpeedMultiplier(float multiplier)
    {
        if (move != null)
            move.moveSpeed = baseSpeed * multiplier;
    }

    public void SetMovable(bool movable)
    {
        if (move != null)
            move.SetMovable(movable);
    }

    public void EnableCatch(bool enable)
    {
        canBeCaught = enable;
    }

    // =======================
    // 捕捉逻辑（认知训练版核心）
    // =======================
    public void OnCatch()
    {
        if (!canBeCaught || isCaught) return;

        // ❌ 非目标鱼：错误反馈，不消失
        if (!isTarget)
        {
            if (wrongClicked) return; // 防止刷错误
            wrongClicked = true;
            StartCoroutine(WrongFeedback());
            OnFishCaughtEvent?.Invoke(this);
            return;
        }

        // ✅ 目标鱼：正确 → 金色 → 消失
        isCaught = true;
        StartCoroutine(CorrectFeedback());
    }

    // =======================
    // 正确反馈
    // =======================
    IEnumerator CorrectFeedback()
    {
        if (rend != null)
            rend.material.color = correctCatchColor; // 捕捉正确的金色

        yield return new WaitForSeconds(0.15f);

        gameObject.SetActive(false);

        OnFishCaughtEvent?.Invoke(this);
    }

    // =======================
    // 错误反馈
    // =======================
    IEnumerator WrongFeedback()
    {
        if (rend != null)
            rend.material.color = wrongCatchColor;

        yield return new WaitForSeconds(0.3f);

        // 点击错误后颜色恢复到默认材质颜色
        // 因为你材质不需要初始颜色，所以不用恢复 originalColor
        if (rend != null)
            rend.material.color = rend.material.color; // 保持原材质
    }

    // =======================
    // 重置
    // =======================
    public void ResetFish()
    {
        isTarget = false;
        canBeCaught = false;
        isCaught = false;
        wrongClicked = false;

        SetSpeedMultiplier(1f);
        SetMovable(true);

        // 重置颜色为材质默认
        if (rend != null)
            rend.material.color = rend.material.color;

        gameObject.SetActive(true);
    }
}
