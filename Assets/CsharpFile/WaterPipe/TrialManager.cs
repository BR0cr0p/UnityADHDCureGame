using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
    [Header("Taps & Bucket")]
    public TapController[] taps;
    public Bucket bucket;

    [Header("UI")]
    public TapResultController resultController;
    public WPInstructionUI instructionUI;

    [Header("Easy Settings")]
    public float easyInterval = 3f;
    public int easyTrials = 4;

    [Header("Medium Settings")]
    public float mediumMinInterval = 1f;
    public float mediumMaxInterval = 5f;
    public int mediumTrials = 6;

    [Header("Hard Settings")]
    public int hardTrials = 6;
    public float sandPenalty = 0.5f;

    [HideInInspector] public TapDifficulty currentDifficulty;
    private PipeHeadSensor pipeSensor;

    private void Awake()
    {
        pipeSensor = FindObjectOfType<PipeHeadSensor>();
        if (pipeSensor != null)
        {
            pipeSensor.bucket = bucket;
            pipeSensor.sandPenalty = sandPenalty;
        }
    }

    public void StartLevel(TapDifficulty difficulty)
    {
        currentDifficulty = difficulty;
        bucket.ResetWater();

        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null) pipe.enableInput = false;

        switch (difficulty)
        {
            case TapDifficulty.Easy: StartCoroutine(EasyFlow()); break;
            case TapDifficulty.Medium: StartCoroutine(MediumFlow()); break;
            case TapDifficulty.Hard: StartCoroutine(HardFlow()); break;
        }
    }

    private IEnumerator EasyFlow()
    {
        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null) pipe.enableInput = true;

        for (int i = 0; i < easyTrials; i++)
        {
            TapController tap = taps[i % taps.Length];
            tap.SetWater();
            pipeSensor.activeTap = tap;
            pipeSensor.ResetCapture();

            yield return new WaitForSeconds(2f);
            tap.SetIdle();
            yield return new WaitForSeconds(easyInterval);
        }

        if (pipe != null) pipe.enableInput = false;
        resultController.ShowResult();
        Debug.Log("Easy Finished");
    }

    private IEnumerator MediumFlow()
    {
        // 场景1：可见水位
        bucket.ResetWater(true);
        yield return StartCoroutine(ShowInstruction("场景1：请接住水流，水位可见！", 2f));
        yield return StartCoroutine(FlowLoop(mediumTrials, true, false));

        // 场景2：不可见水位
        bucket.ResetWater(false);
        yield return StartCoroutine(ShowInstruction("场景2：水位不可见，请接水！", 2f));
        yield return StartCoroutine(FlowLoop(mediumTrials, false, false));

        resultController.ShowResult();
        Debug.Log("Medium Finished");
    }

    private IEnumerator HardFlow()
    {
        // 场景1：可见水位，带沙子
        bucket.ResetWater(true);
        yield return StartCoroutine(ShowInstruction("场景1：注意沙子流出，水位可见！", 2f));
        yield return StartCoroutine(FlowLoop(hardTrials, true, true));

        // 场景2：不可见水位，带沙子
        bucket.ResetWater(false);
        yield return StartCoroutine(ShowInstruction("场景2：水位不可见，避开沙子！", 2f));
        yield return StartCoroutine(FlowLoop(hardTrials, false, true));

        resultController.ShowResult();
        Debug.Log("Hard Finished");
    }

    // 通用流水循环
    private IEnumerator FlowLoop(int trials, bool visibleWater, bool withSand)
    {
        PipeHeadController pipe = FindObjectOfType<PipeHeadController>();
        if (pipe != null) pipe.enableInput = true;

        bucket.canShowWater = visibleWater;

        for (int i = 0; i < trials; i++)
        {
            // ===== 1. 清空全部 =====
            foreach (var t in taps)
                t.SetIdle();

            pipeSensor.ResetCapture();

            // ===== 2. 随机一个水龙头出水 =====
            TapController waterTap = taps[Random.Range(0, taps.Length)];
            waterTap.SetWater();
            pipeSensor.activeTap = waterTap;

            // ===== 3. 随机 1~3 个不同龙头出沙 =====
            if (withSand)
            {
                List<TapController> pool = new List<TapController>(taps);
                pool.Remove(waterTap); // 永远不能和水重合

                int sandCount = Mathf.Min(Random.Range(1, 4), pool.Count);

                for (int s = 0; s < sandCount; s++)
                {
                    int idx = Random.Range(0, pool.Count);
                    pool[idx].SetSand();
                    pool.RemoveAt(idx);
                }
            }

            // ===== 4. 本轮存在 2 秒 =====
            yield return new WaitForSeconds(2f);

            // ===== 5. 全部停止 =====
            foreach (var t in taps)
                t.SetIdle();

            yield return new WaitForSeconds(Random.Range(mediumMinInterval, mediumMaxInterval));
        }

        if (!visibleWater) bucket.ShowTotalWater();
        if (pipe != null) pipe.enableInput = false;
    }




    private IEnumerator ShowInstruction(string text, float duration)
    {
        if (instructionUI != null)
        {
            instructionUI.Show(text, duration, 0.3f);
            yield return new WaitForSeconds(duration + 0.6f);
        }
    }
}
