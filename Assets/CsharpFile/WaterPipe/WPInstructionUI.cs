using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WPInstructionUI : MonoBehaviour
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

    public void Show(string text, float duration = 2f, float fadeTime = 0.3f)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        instructionText.text = text;
        if (background != null)
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowCoroutine(duration, fadeTime));
    }

    private IEnumerator ShowCoroutine(float duration, float fadeTime)
    {
        // 淡入
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }

        // 等待显示时间
        yield return new WaitForSeconds(duration);

        // 淡出
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }
    }
}
