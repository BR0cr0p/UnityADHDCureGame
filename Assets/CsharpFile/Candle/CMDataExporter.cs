using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CMDataExporter
{
    public static void ExportCSV(List<RoundRecord> records)
    {
        string folder = Application.persistentDataPath; // 永久存储路径
        string filePath = Path.Combine(folder, $"CMData_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv");

        using StreamWriter writer = new(filePath);

        writer.WriteLine("Round,ClickIndex,Number,Interval");

        for (int r = 0; r < records.Count; r++)
        {
            var record = records[r];
            for (int i = 0; i < record.clickedNumbers.Count; i++)
            {
                float interval = i == 0 ? 0f : record.clickIntervals[i - 1];
                writer.WriteLine($"{r + 1},{i + 1},{record.clickedNumbers[i]},{interval:F3}");
            }
        }

        Debug.Log($"CM 数据已保存到: {filePath}");
    }
}
