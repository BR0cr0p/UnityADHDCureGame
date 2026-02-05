using System.Collections.Generic;
using UnityEngine;

public enum SchulteDifficulty
{
    Easy_3x3,
    Medium_4x4,
    Hard_5x5
}

public class SchulteGridGenerator : MonoBehaviour
{
    [Header("Difficulty")]
    public SchulteDifficulty difficulty = SchulteDifficulty.Easy_3x3;

    [Header("Layout")]
    public float boardSize = 1.2f;     // 整个方格总宽度（世界单位）
    public float cellPadding = 0.15f;  // 格子之间的间隙

    [Header("References")]
    public GridCell cellPrefab;
    public Transform gridCenter;

    private int gridSize;

    void Start()
    {
        //GenerateWithDifficulty();
    }

    void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case SchulteDifficulty.Easy_3x3:
                gridSize = 3;
                break;
            case SchulteDifficulty.Medium_4x4:
                gridSize = 4;
                break;
            case SchulteDifficulty.Hard_5x5:
                gridSize = 5;
                break;
        }
    }


    void Generate()
    {
        // 1️⃣ 生成并打乱数字
        List<int> numbers = new List<int>();
        for (int i = 1; i <= gridSize * gridSize; i++)
            numbers.Add(i);

        for (int i = 0; i < numbers.Count; i++)
        {
            int r = Random.Range(i, numbers.Count);
            (numbers[i], numbers[r]) = (numbers[r], numbers[i]);
        }

        // 2️⃣ 根据 boardSize 自动计算尺寸
        float cellSize = (boardSize - cellPadding * (gridSize - 1)) / gridSize;
        float spacing = cellSize + cellPadding;

        float start = -(gridSize - 1) * spacing / 2f;
        int index = 0;

        // 3️⃣ 生成网格
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Vector3 offset = new Vector3(
                    start + col * spacing,
                    start + (gridSize - 1 - row) * spacing,
                    0f
                );

                GridCell cell = Instantiate(
                    cellPrefab,
                    gridCenter.position + offset,
                    Quaternion.identity,
                    gridCenter
                );

                cell.SetNumber(numbers[index++]);
                cell.SetSize(cellSize);   // ⭐ 核心：同步缩放
            }
        }
    }

    public void ClearGrid()
    {
        for (int i = gridCenter.childCount - 1; i >= 0; i--)
        {
            Destroy(gridCenter.GetChild(i).gameObject);
        }
    }

    public void SetDifficulty(SchulteDifficulty difficulty)
    {
        this.difficulty = difficulty;
        GenerateWithDifficulty();
    }

    public void GenerateWithDifficulty()
    {

        ClearGrid();
        ApplyDifficulty();

        Generate();   // 先生成 GridCell

        int max = gridSize * gridSize;

        SchulteController controller = FindObjectOfType<SchulteController>();
        if (controller != null)
            controller.StartNewGame(max);   // 再开始游戏

    }

}
