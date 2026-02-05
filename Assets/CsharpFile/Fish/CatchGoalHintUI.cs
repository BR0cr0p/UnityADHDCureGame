using UnityEngine;
using TMPro;
using System.Collections;

public class GameStartHintUI : MonoBehaviour
{
    public GameObject hintRoot;          // 整个提示 UI
    public TextMeshProUGUI hintText;     // 已在Inspector中设置了文本和颜色
    public float showTime = 1.5f;        // 提示显示时间

    void Start()
    {
        // 在开始时隐藏提示UI
        if (hintRoot != null)
            hintRoot.SetActive(false);
    }

    // 这个方法可以绑定到按钮的On Click()事件
    public void ShowHint()
    {
        // 直接开始显示流程，不修改文本和颜色
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        if (hintRoot == null) yield break;

        // 显示提示
        hintRoot.SetActive(true);

        // 等待指定时间
        yield return new WaitForSeconds(showTime);

        // 隐藏提示
        hintRoot.SetActive(false);
    }

    // 停止显示提示（如果需要提前隐藏）
    public void StopHint()
    {
        StopAllCoroutines();
        if (hintRoot != null)
            hintRoot.SetActive(false);
    }
}