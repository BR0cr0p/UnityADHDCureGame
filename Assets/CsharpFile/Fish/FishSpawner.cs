using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;
    public Transform fishRoot;
    public Transform poolCenter;


    [Header("Spawn")]
    [Tooltip("鱼之间的最小生成距离")]
    public float minSpawnDistance = 0.8f;

    [Tooltip("单条鱼最大尝试次数")]
    public int maxTryCount = 30;

    public List<Fish> SpawnFish(int count, Vector3 min, Vector3 max)
    {
        Clear();

        List<Fish> result = new List<Fish>();
        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Vector3.zero;
            bool found = false;

            for (int tryCount = 0; tryCount < maxTryCount; tryCount++)
            {
                Vector3 offset = new Vector3(
                    Random.Range(min.x, max.x),
                    Random.Range(min.y, max.y),
                    Random.Range(min.z, max.z)
                );

                pos = poolCenter.position + offset;


                if (!IsTooClose(pos, usedPositions))
                {
                    found = true;
                    break;
                }
            }

            // ⭐ 如果实在找不到，接受但给一点扰动，避免完全重合
            if (!found && usedPositions.Count > 0)
            {
                pos = usedPositions[usedPositions.Count - 1] +
                      Random.insideUnitSphere * minSpawnDistance;
            }

            usedPositions.Add(pos);

            GameObject go = Instantiate(fishPrefab, pos, Quaternion.identity, fishRoot);

            Fish fish = go.GetComponent<Fish>();
            if (fish != null)
                fish.ResetFish();

            // 初始化 FishMove 的活动范围
            FishMove move = go.GetComponent<FishMove>();
            if (move != null)
            {
                move.minBounds = poolCenter.position + min;
                move.maxBounds = poolCenter.position + max;
            }

            Debug.Log($"min={min}, max={max}, poolCenter={poolCenter.position}");

            result.Add(fish);
        }

        return result;
    }

    bool IsTooClose(Vector3 pos, List<Vector3> list)
    {
        foreach (var p in list)
        {
            if (Vector3.Distance(pos, p) < minSpawnDistance)
                return true;
        }
        return false;
    }

    public void Clear()
    {
        for (int i = fishRoot.childCount - 1; i >= 0; i--)
            Destroy(fishRoot.GetChild(i).gameObject);
    }
}
