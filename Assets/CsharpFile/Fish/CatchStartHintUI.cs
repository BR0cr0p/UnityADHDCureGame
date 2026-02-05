using UnityEngine;
using TMPro;
using System.Collections;

public class CatchStartHintUI : MonoBehaviour
{
    public GameObject hintRoot;          // 整个提示 UI
    public TextMeshProUGUI hintText;
    public float showTime = 1.5f;

    private FishGameController game;

    void Awake()
    {
        game = FindObjectOfType<FishGameController>();
        if (game != null)
        {
            game.OnCatchPhaseStarted += ShowHint;
        }
    }

    void Start()
    {
        if (hintRoot != null)
            hintRoot.SetActive(false);
    }

    void OnDestroy()
    {
        if (game != null)
        {
            game.OnCatchPhaseStarted -= ShowHint;
        }
    }

    void ShowHint()
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        if (hintRoot == null) yield break;

        hintRoot.SetActive(true);

        yield return new WaitForSeconds(showTime);

        hintRoot.SetActive(false);
    }
}
