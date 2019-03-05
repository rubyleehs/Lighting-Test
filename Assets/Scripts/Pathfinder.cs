using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder: MonoBehaviour
{
    private static float untravellableDijkValue = -1000;

    public static void DijkFloodStep(ref float[,] map, int x, int y, float value, float floodStepValue, int stepCap)
    {
        if (stepCap <= 0 || x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return;

        if ((floodStepValue > 0 && value <= map[x, y]) || (floodStepValue < 0 && value >= map[x, y]))
        {
            map[x, y] = value;
            DijkFloodStep(ref map, x + 1, y, value + floodStepValue, floodStepValue, stepCap - 1);
            DijkFloodStep(ref map, x, y + 1, value + floodStepValue, floodStepValue, stepCap - 1);
            DijkFloodStep(ref map, x - 1, y, value + floodStepValue, floodStepValue, stepCap - 1);
            DijkFloodStep(ref map, x, y - 1, value + floodStepValue, floodStepValue, stepCap - 1);
        }
    }

    public static void CreateFreshDijkstraMap(ref int[,] obstacleMap, out float[,] dijkMap, List<int> ignoreObstacles)
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

    public static List<Vector2Int> RequestAStarPath(ref int[,] obstacleMap, Vector2Int from, Vector2Int to, List<int> ignoreObstacles, int pathStepCap)
    {
        int[,] dir = new int[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        float[,] cellFCost = new float[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        int[,] _obstacleMap = new int[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        List<Vector2Int> discoveredCells = new List<Vector2Int>();
        bool endReached = false;

        for (int y = 0; y < obstacleMap.GetLength(1); y++)
        {
            for (int x = 0; x < obstacleMap.GetLength(0); x++)
            {
                if ((obstacleMap[x, y] < 0 && !ignoreObstacles.Contains(obstacleMap[x, y]))) _obstacleMap[x, y] = -1;
            }
        }

        DiscoverCell(ref _obstacleMap, ref cellFCost, ref dir, ref discoveredCells, from, to, 0, 0);
        CheckNextAStarPath(ref _obstacleMap, ref cellFCost, ref dir, ref discoveredCells, to, pathStepCap, ref endReached);
        if (!endReached) return null;

        List<Vector2Int> path = new List<Vector2Int>();
        TracePath(ref path, ref dir, to.x, to.y);
        path.Reverse();
        return path;
    }

    public static void CheckNextAStarPath(ref int[,] obstacleMap, ref float[,] cellFCost, ref int[,] dir, ref List<Vector2Int> discoveredCells, Vector2Int to, int pathStepCap, ref bool endReached)
    {

        //lowestfCost, x&y = fCostcoord, z = fCost
        Vector3 lowestFCost = Vector3.forward * Mathf.Infinity;
        for (int i = 0; i < discoveredCells.Count; i++)
        {
            Vector2Int dc = discoveredCells[i];
            if (cellFCost[dc.x, dc.y] < lowestFCost.z) lowestFCost = new Vector3(dc.x, dc.y, cellFCost[dc.x, dc.y]);
        }
        if (lowestFCost.x == to.x && lowestFCost.y == to.y)
        {
            endReached = true;
            return;
        }
        else if (obstacleMap[(int)lowestFCost.x,(int)lowestFCost.y] >= pathStepCap) return;
        Debug.Log(obstacleMap[(int)lowestFCost.x, (int)lowestFCost.y] + "|| " + lowestFCost.x + ", " + lowestFCost.y + ", " + lowestFCost.z);
        discoveredCells.Remove(new Vector2Int((int)lowestFCost.x, (int)lowestFCost.y));
        DiscoverCell(ref obstacleMap, ref cellFCost, ref dir, ref discoveredCells, new Vector2Int((int)lowestFCost.x + 1, (int)lowestFCost.y), to, 3, obstacleMap[(int)lowestFCost.x, (int)lowestFCost.y] + 1);
        DiscoverCell(ref obstacleMap, ref cellFCost, ref dir, ref discoveredCells, new Vector2Int((int)lowestFCost.x, (int)lowestFCost.y + 1), to, 4, obstacleMap[(int)lowestFCost.x, (int)lowestFCost.y] + 1);
        DiscoverCell(ref obstacleMap, ref cellFCost, ref dir, ref discoveredCells, new Vector2Int((int)lowestFCost.x - 1, (int)lowestFCost.y), to, 1, obstacleMap[(int)lowestFCost.x, (int)lowestFCost.y] + 1);
        DiscoverCell(ref obstacleMap, ref cellFCost, ref dir, ref discoveredCells, new Vector2Int((int)lowestFCost.x, (int)lowestFCost.y - 1), to, 2, obstacleMap[(int)lowestFCost.x, (int)lowestFCost.y] + 1);

        CheckNextAStarPath(ref obstacleMap, ref cellFCost, ref dir, ref discoveredCells, to, pathStepCap, ref endReached);
    }

    public static void DiscoverCell(ref int[,] obstacleMap, ref float[,] cellFCost, ref int[,] dir, ref List<Vector2Int> discoveredCells, Vector2Int from, Vector2Int to, int prevStepDir, int pathStepCount)
    {
        float fCost = (to - from).magnitude + pathStepCount;
        
        if (from.x < 0 || from.y < 0 || from.x >= obstacleMap.GetLength(0) || from.y >= obstacleMap.GetLength(1) || obstacleMap[from.x,from.y] < 0) return;
        if (fCost >= cellFCost[from.x, from.y] && cellFCost[from.x, from.y] != 0 || fCost < 0) return;
        //Debug.Log(pathStepCount);
        obstacleMap[from.x, from.y] = pathStepCount;
        cellFCost[from.x, from.y] = fCost;
        dir[from.x, from.y] = prevStepDir;
        discoveredCells.Add(from);
    }

    public static void TracePath(ref List<Vector2Int> path, ref int[,] dir, int x, int y)
    {
        path.Add(new Vector2Int(x, y));
        switch (dir[x, y])
        {
            case 1:
                TracePath(ref path, ref dir, x + 1, y);
                break;
            case 2:
                TracePath(ref path, ref dir, x, y + 1);
                break;
            case 3:
                TracePath(ref path, ref dir, x - 1, y);
                break;
            case 4:
                TracePath(ref path, ref dir, x, y - 1);
                break;
            default:
                break;
        }
    }
    
}
