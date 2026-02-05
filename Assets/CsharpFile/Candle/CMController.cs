using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RoundRecord
{
    public List<int> clickedNumbers = new();
    public List<float> clickIntervals = new();
}

public class CMController : MonoBehaviour
{
    enum TaskStage
    {
        Task1_ExtinguishOrder,
        Task_ShowSequence,
        Task_InputSequence,
        Finished
    }

    TaskStage stage;

    List<CandleCell> schulteSequence = new();
    List<CandleCell> memorySequence = new();
    List<CandleCell> memoryTarget = new();
    List<CandleCell> playerSequence = new();
    List<float> clickIntervals = new();
    float lastClickTime;
    bool isShowing = false;

    int[] memoryRounds = { 3, 4, 5 };
    int currentRound = 0;
    bool reverseMode = false;

    public CMInstructionUI instructionUI;
    public bool inputEnabled { get; private set; }
    public CMGenerator generator;

    public List<RoundRecord> roundRecords = new();
    private RoundRecord currentRoundRecord;

    public int totalClicks { get; private set; }
    public int correctClicks { get; private set; }
    public int orderErrors { get; private set; }
    public int omissionErrors { get; private set; }
    public int insertionErrors { get; private set; }

    public event Action OnGameFinished;

    void Awake()
    {
        generator = FindObjectOfType<CMGenerator>();
    }

    void Update()
    {
        if (!inputEnabled || isShowing) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CandleCell cell = hit.collider.GetComponentInParent<CandleCell>();
                if (cell != null)
                    HandleClick(cell);
            }
        }
    }

    void SetStage(TaskStage newStage)
    {
        stage = newStage;
        Debug.Log($"=== {stage} | Round {currentRound} | Reverse {reverseMode} ===");
    }

    void HandleClick(CandleCell cell)
    {
        if (!inputEnabled || isShowing || cell.IsDead) return;

        totalClicks++;
        if (playerSequence.Contains(cell)) return;

        // 记录点击间隔
        if (currentRoundRecord != null && currentRoundRecord.clickedNumbers.Count > 0)
        {
            currentRoundRecord.clickIntervals.Add(Time.time - lastClickTime);
        }
        lastClickTime = Time.time;

        playerSequence.Add(cell);
        cell.SetLit(true, false);

        // 记录点击编号
        currentRoundRecord?.clickedNumbers.Add(cell.Number);

        // 判断输入完成
        int requiredCount = (stage == TaskStage.Task_InputSequence) ? memoryTarget.Count : schulteSequence.Count;
        if (playerSequence.Count >= requiredCount)
        {
            inputEnabled = false;
            AnalyzeSequence();

            if (stage == TaskStage.Task1_ExtinguishOrder)
            {
                StartCoroutine(StartMemoryRound());
            }
            else if (stage == TaskStage.Task_InputSequence)
            {
                HandleMemoryProgress();
            }
        }
    }

    void AnalyzeSequence()
    {
        List<CandleCell> reference = (stage == TaskStage.Task_InputSequence) ? memoryTarget : schulteSequence;
        HashSet<CandleCell> targetSet = new(reference);
        HashSet<CandleCell> playerSet = new(playerSequence);

        insertionErrors += playerSet.Count(c => !targetSet.Contains(c));
        omissionErrors += targetSet.Count(c => !playerSet.Contains(c));

        for (int i = 0; i < reference.Count; i++)
        {
            if (i >= playerSequence.Count) break;
            if (playerSequence[i] == reference[i])
                correctClicks++;
            else if (targetSet.Contains(playerSequence[i]))
                orderErrors++;
        }
    }

    void HandleMemoryProgress()
    {
        if (IsIncrementMode())
        {
            if (!reverseMode)
            {
                if (currentRound < memoryRounds.Length - 1)
                {
                    currentRound++;
                    StartCoroutine(StartMemoryRound());
                    return;
                }
                // 正序最后一轮完成 → 进入逆序
                reverseMode = true;
                currentRound = 0;
                StartCoroutine(StartMemoryRound());
                return;
            }
            else
            {
                if (currentRound < memoryRounds.Length - 1)
                {
                    currentRound++;
                    StartCoroutine(StartMemoryRound());
                    return;
                }
                else
                {
                    FinishGame(); // 逆序最后一轮完成
                }
            }
        }
        else
        {
            FinishGame(); // 2x2模式
        }
    }

    void FinishGame()
    {
        SetStage(TaskStage.Finished);
        instructionUI.Hide();
        OnGameFinished?.Invoke();
        CMDataExporter.ExportCSV(roundRecords); // 导出 CSV
    }

    bool IsIncrementMode()
    {
        return generator != null && generator.level == CMLevel.Level2_3x3;
    }

    public void StartNewGame()
    {
        totalClicks = correctClicks = orderErrors = omissionErrors = insertionErrors = 0;
        currentRound = 0;
        reverseMode = false;

        playerSequence.Clear();
        clickIntervals.Clear();
        roundRecords.Clear();
        currentRoundRecord = null;

        var all = FindObjectsOfType<CandleCell>().ToList();
        foreach (var c in all) c.ResetCandle();

        schulteSequence = all.OrderByDescending(c => c.Number).ToList();

        SetStage(TaskStage.Task1_ExtinguishOrder);
        StartCoroutine(ShowInstruction("小朋友，请按照从大到小的顺序，依次点亮蜡烛哦！", 2f));
        inputEnabled = true;

        // 初始化舒尔特轮记录
        currentRoundRecord = new RoundRecord();
        roundRecords.Add(currentRoundRecord);
    }

    public void StartNewGame(CMLevel _) => StartNewGame();

    IEnumerator StartMemoryRound()
    {
        SetStage(TaskStage.Task_ShowSequence);
        isShowing = true;
        inputEnabled = false;

        playerSequence.Clear();
        clickIntervals.Clear();

        yield return StartCoroutine(ShowInstruction("看好了，蜡烛要点亮了！", 1.2f));

        var all = FindObjectsOfType<CandleCell>().ToList();
        foreach (var c in all) c.ResetCandle();

        int count = IsIncrementMode() ? memoryRounds[currentRound] : 2;

        memorySequence = all.OrderBy(c => c.Number).Take(count).ToList();
        var displaySequence = memorySequence.OrderBy(x => UnityEngine.Random.value).ToList();

        memoryTarget = reverseMode
            ? memorySequence.AsEnumerable().Reverse().ToList()
            : new List<CandleCell>(memorySequence);

        // 初始化本轮记录
        currentRoundRecord = new RoundRecord();
        roundRecords.Add(currentRoundRecord);

        yield return new WaitForSeconds(0.4f);

        foreach (var cell in displaySequence)
        {
            cell.SetLit(true, true);
            yield return new WaitForSeconds(1.1f);
            cell.SetLit(false, true);
            yield return new WaitForSeconds(0.4f);
        }

        string tip = reverseMode ? "现在倒着顺序点亮蜡烛！" : "现在按刚才顺序点亮蜡烛！";
        yield return StartCoroutine(ShowInstruction(tip, 1.2f));

        isShowing = false;
        inputEnabled = true;
        SetStage(TaskStage.Task_InputSequence);
    }

    IEnumerator ShowInstruction(string text, float duration)
    {
        instructionUI.Show(text, duration, 0.3f);
        yield return new WaitForSeconds(duration + 0.6f);
    }

    public bool OnCellClicked(CandleCell cell)
    {
        if (!inputEnabled || isShowing) return false;
        HandleClick(cell);
        return true;
    }

    public void EnableInput() => inputEnabled = true;
    public void DisableInput() => inputEnabled = false;
}
