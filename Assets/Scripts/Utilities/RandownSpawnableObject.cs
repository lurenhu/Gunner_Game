using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandownSpawnableObject<T>
{
    private struct chanceBoundaries
    {
        public T spawnableObject;
        public int lowBoundaryValue;
        public int highBoundaryValue;
    }

    private int ratioValueToTal = 0;
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectByLevel<T>> spawnableObjectByLevelList;

    public RandownSpawnableObject(List<SpawnableObjectByLevel<T>> spawnableObjectByLevels)
    {
        this.spawnableObjectByLevelList = spawnableObjectByLevels;
    }

    public T GetItem()
    {
        int upperBoundary = -1;
        ratioValueToTal = 0;
        chanceBoundariesList.Clear();
        T spawnableObject = default(T);

        foreach (SpawnableObjectByLevel<T> spawnableObjectByLevel in spawnableObjectByLevelList)
        {
            if (spawnableObjectByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectByLevel.spawnableObjectRatioList)
                {
                    int lowerBoundary = upperBoundary + 1;

                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;

                    ratioValueToTal += spawnableObjectRatio.ratio;

                    chanceBoundariesList.Add(new chanceBoundaries()
                    {
                        spawnableObject = spawnableObjectRatio.dungeonObject,
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary,

                    });
                }
            }
        }

        if (chanceBoundariesList.Count == 0)
        {
            return default(T);
        }

        int lookUpValue = Random.Range(0,ratioValueToTal);

        foreach (chanceBoundaries spawnChance in chanceBoundariesList)
        {
            if (lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue)
            {
                spawnableObject = spawnChance.spawnableObject;
                break;
            }

        }

        return spawnableObject; 

    }
}
