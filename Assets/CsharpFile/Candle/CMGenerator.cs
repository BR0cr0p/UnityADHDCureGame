using System.Collections.Generic;
using UnityEngine;

public enum CMLevel
{
    Level1_2x2,
    Level2_3x3
}

public class CMGenerator : MonoBehaviour
{
    [Header("Level")]
    public CMLevel level = CMLevel.Level1_2x2;

    [Header("Layout")]
    public float boardSize = 1.2f;
    public float cellPadding = 0.15f;

    [Header("References")]
    public CandleCell cellPrefab;
    public Transform gridCenter;

    int gridSize;
    CMController controller;

    void ApplyLevel()
    {
        switch (level)
        {
            case CMLevel.Level1_2x2:
                gridSize = 2;
                break;

            case CMLevel.Level2_3x3:
                gridSize = 3;
                break;
        }
    }

    void Generate()
    {
        List<int> numbers = new();
        for (int i = 1; i <= gridSize * gridSize; i++)
            numbers.Add(i);

        for (int i = 0; i < numbers.Count; i++)
        {
            int r = Random.Range(i, numbers.Count);
            (numbers[i], numbers[r]) = (numbers[r], numbers[i]);
        }

        float cellSize = (boardSize - cellPadding * (gridSize - 1)) / gridSize;
        float spacing = cellSize + cellPadding;
        float start = -(gridSize - 1) * spacing / 2f;

        int index = 0;

        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize; c++)
            {
                Vector3 pos = gridCenter.position + new Vector3(
                    start + c * spacing,
                    start + (gridSize - 1 - r) * spacing,
                    0
                );

                var cell = Instantiate(cellPrefab, pos, Quaternion.identity, gridCenter);
                cell.SetNumber(numbers[index++]);
                cell.SetSize(cellSize);
            }
        }
    }

    public void ClearGrid()
    {
        for (int i = gridCenter.childCount - 1; i >= 0; i--)
            Destroy(gridCenter.GetChild(i).gameObject);
    }

    public void GenerateLevel()
    {
        if (controller == null)
            controller = FindObjectOfType<CMController>();

        ClearGrid();
        ApplyLevel();
        Generate();

        controller.StartNewGame(level);   // ⭐ 把关卡传进去
    }

    public void SetLevel(CMLevel newLevel)
    {
        level = newLevel;
        GenerateLevel();
    }
}
