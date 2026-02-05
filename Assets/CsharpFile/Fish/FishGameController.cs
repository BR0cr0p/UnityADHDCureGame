using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGameController : MonoBehaviour
{
    [System.Serializable]
    public class DifficultySetting
    {
        public string name;
        public int fishCount;
        public int targetCount;
        public float markTime;
        public float confuseTime;
        public float speedMultiplier;
        public Vector3 minBounds = new Vector3(-7, 1f, -7);
        public Vector3 maxBounds = new Vector3(7, 3f, 7);
    }

    [Header("Difficulty Settings")]
    public DifficultySetting easy;
    public DifficultySetting medium;
    public DifficultySetting hard;

    [HideInInspector]
    public DifficultySetting currentDifficulty;

    [Header("References")]
    public FishSpawner fishSpawner;

    private List<Fish> fishes = new List<Fish>();

    // 游戏状态：开始提示
    public event System.Action OnCatchPhaseStarted;

    // =======================
    // 数据统计
    // =======================

    private int correctCatchCount;
    private int totalClickCount;
    private float catchStartTime;
    private bool isCatching;
    private bool gameFinished;

    // =======================
    // 游戏完成事件（Result UI 用）
    // =======================
    public event System.Action<float, float, int, int> OnGameFinished;

    // =======================
    // UI 唯一入口
    // =======================
    public void StartGame(DifficultySetting difficulty)
    {
        StopAllCoroutines();

        currentDifficulty = difficulty;

        // 重置状态
        correctCatchCount = 0;
        totalClickCount = 0;
        isCatching = false;
        gameFinished = false;

        fishes = fishSpawner.SpawnFish(
            difficulty.fishCount,
            difficulty.minBounds,
            difficulty.maxBounds
        );

        StartCoroutine(GameFlow());
    }

    // =======================
    // 游戏流程
    // =======================
    IEnumerator GameFlow()
    {
        yield return new WaitForSeconds(0.3f);

        // 1️⃣ 随机目标鱼
        List<Fish> shuffled = new List<Fish>(fishes);
        Shuffle(shuffled);

        for (int i = 0; i < currentDifficulty.targetCount && i < shuffled.Count; i++)
            shuffled[i].isTarget = true;

        // 2️⃣ 标记阶段
        foreach (var fish in fishes)
        {
            fish.SetMarked(fish.isTarget);
            fish.SetMovable(false);
            fish.EnableCatch(false);
            fish.SetSpeedMultiplier(1f);

            fish.OnFishCaughtEvent -= OnFishCaught;
            fish.OnFishCaughtEvent += OnFishCaught;
        }

        yield return new WaitForSeconds(currentDifficulty.markTime);

        // 3️⃣ 混乱阶段
        foreach (var fish in fishes)
        {
            fish.SetMarked(false);
            fish.SetMovable(true);
            fish.EnableCatch(false);
            fish.SetSpeedMultiplier(currentDifficulty.speedMultiplier);
        }

        yield return new WaitForSeconds(currentDifficulty.confuseTime);

        // 4️⃣ 捕捉阶段
        foreach (var fish in fishes)
        {
            fish.SetMovable(false);
            fish.EnableCatch(true);
        }

        catchStartTime = Time.time;
        isCatching = true;

        OnCatchPhaseStarted?.Invoke();

        Debug.Log(">>> 开始捕捉目标鱼");
    }

    // =======================
    // 捕捉回调
    // =======================
    void OnFishCaught(Fish fish)
    {
        if (!isCatching || gameFinished) return;

        totalClickCount++;

        if (fish.isTarget)
            correctCatchCount++;

        if (AllTargetsCaught())
        {
            FinishGame();
        }
    }

    // =======================
    // 游戏完成
    // =======================
    void FinishGame()
    {
        if (gameFinished) return;

        gameFinished = true;
        isCatching = false;

        float totalTime = Time.time - catchStartTime;
        float accuracy = totalClickCount > 0
            ? (float)correctCatchCount / totalClickCount * 100f
            : 0f;

        Debug.Log($"=== 完成 === 用时 {totalTime:F2}s | 正确率 {accuracy:F1}%");

        // 取消事件订阅
        foreach (var f in fishes)
            f.OnFishCaughtEvent -= OnFishCaught;

        // 🔔 通知 Result UI
        OnGameFinished?.Invoke(
            totalTime,
            accuracy,
            correctCatchCount,
            totalClickCount
        );
    }

    bool AllTargetsCaught()
    {
        foreach (var f in fishes)
        {
            if (f.isTarget && f.gameObject.activeSelf)
                return false;
        }
        return true;
    }

    void Shuffle(List<Fish> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }


}
