using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CMInstructionUI : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public Image background;

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// 显示字幕
    /// </summary>
    /// <param name="text">要显示的文本</param>
    /// <param name="duration">显示时间，秒</param>
    /// <param name="fadeTime">淡入淡出时间，秒</param>
    public void Show(string text, float duration = 2f, float fadeTime = 0.3f)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);  // 自动激活

        instructionText.text = text;

        if (background != null)
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowCoroutine(duration, fadeTime));
    }

    public void Hide(float fadeTime = 0.3f)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(FadeOut(fadeTime));
    }

    private IEnumerator ShowCoroutine(float duration, float fadeTime)
    {
        // 淡入
        yield return StartCoroutine(Fade(0f, 1f, fadeTime));

        // 等待显示时间
        yield return new WaitForSeconds(duration);

        // 淡出
        yield return StartCoroutine(Fade(1f, 0f, fadeTime));
    }

    private IEnumerator Fade(float start, float end, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, t / time);
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        yield return StartCoroutine(Fade(canvasGroup.alpha, 0f, fadeTime));
    }
}
