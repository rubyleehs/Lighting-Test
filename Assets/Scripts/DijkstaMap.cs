using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstaMap : MonoBehaviour
{
    public float untravellableDijkValue = -1000;

    public void FloodStep(ref float[,] map, int x, int y, float value, float floodStepValue, int stepCap)
    {
        if (stepCap <= 0 || x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return;

        if ((floodStepValue > 0 && value <= map[x, y]) || (floodStepValue < 0 && value >= map[x, y]))
        {
            map[x, y] = value;
            FloodStep(ref map, x + 1, y, value + floodStepValue, floodStepValue, stepCap - 1);
            FloodStep(ref map, x, y + 1, value + floodStepValue, floodStepValue, stepCap - 1);
            FloodStep(ref map, x - 1, y, value + floodStepValue, floodStepValue, stepCap - 1);
            FloodStep(ref map, x, y - 1, value + floodStepValue, floodStepValue, stepCap - 1);
        }
    }

    public void CreateFreshDijkstraMap(ref int[,] obstacleMap, out float[,] dijkMap, List<int> ignoreObstacles)
    {
        //obstacles in obtacle map are negative
        dijkMap = new float[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];

        for (int y = 0; y < obstacleMap.GetLength(1); y++)
        {
            for (int x = 0; x < obstacleMap.GetLength(0); x++)
            {
                if (obstacleMap[x, y] < 0 && !ignoreObstacles.Contains(obstacleMap[x,y])) dijkMap[x, y] = untravellableDijkValue;
                else dijkMap[x, y] = 0;
            }
        }
    }
}
