using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public float untravellableDijkValue = -1000;

    public void DijkFloodStep(ref float[,] map, int x, int y, float value, float floodStepValue, int stepCap)
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

    public List<Vector2Int> RequestAStarPath(ref int[,] obstacleMap, Vector2Int from, Vector2Int to, List<int> ignoreObstacles, int pathStepCap)
    {
        int[,] dir = new int[2 * pathStepCap + 1, 2 * pathStepCap + 1];
        float[,] cellFCost = new float[2 * pathStepCap + 1, 2 * pathStepCap + 1];
        bool[,] minifiedObstacleMap = new bool[2 * pathStepCap + 1, 2 * pathStepCap + 1];
        List<Vector2Int> discoveredCells = new List<Vector2Int>();
        bool endReached = false;

        for (int dy = -pathStepCap; dy < pathStepCap ; dy++)
        {
            for (int dx = -pathStepCap; dx < pathStepCap ; dx++)
            {
                if (from.x + dx < 0 || from.y + dy < 0 || from.x + dx >= obstacleMap.GetLength(0) || from.y + dy >= obstacleMap.GetLength(1)) minifiedObstacleMap[pathStepCap + dx, pathStepCap + dy] = true;
                else if (obstacleMap[from.x + dx, from.y + dy] < 0 && !ignoreObstacles.Contains(obstacleMap[from.x + dx, from.y + dy])) minifiedObstacleMap[pathStepCap + dx, pathStepCap + dy] = true;
                else minifiedObstacleMap[pathStepCap + dx, pathStepCap + dy] = false;
            }
        }

        CheckNextAStarPath(ref minifiedObstacleMap, ref cellFCost, ref discoveredCells, ref dir, from, to, 0, pathStepCap, 0, ref endReached);
        if (!endReached) return null;

        List<Vector2Int> path = new List<Vector2Int>();
        TracePath(ref path, ref dir, to.x, to.y);
        path.Reverse();
        return path;
    }

    public void CheckNextAStarPath(ref bool[,] obstacleMap, ref float[,] cellFCost, ref List<Vector2Int> discoveredCells, ref int[,] dir, Vector2Int from, Vector2Int to, int pathStepCount, int pathStepCap, int prevStepDir, ref bool endFound)
    {
        if (endFound || pathStepCount > pathStepCap) return; //|| from.x < 0 || from.y < 0|| from.x > obstacleMap.GetLength(0) || from.y > obstacleMap.GetLength(1)) return;
        float fCost = (to - from).magnitude + pathStepCount;

        if (cellFCost[from.x, from.y] != 0 && fCost >= cellFCost[from.x, from.y]) return;//if it has been checked b4 and its fcost is lower

        cellFCost[from.x, from.y] = fCost;
        dir[from.x, from.y] = prevStepDir;
        if (from == to)
        {
            endFound = true;
            return;
        }

        discoveredCells.Add(from);

        Vector3 lowestFCost = Vector3.zero;
        //lowestfCost, x&y = fCostcoord, z = fCost
        for (int i = 0; i < discoveredCells.Count; i++)
        {
            Vector2Int dc = discoveredCells[i];
            if (lowestFCost.z != 0 && cellFCost[dc.x, dc.y] < lowestFCost.z) new Vector3(dc.x, dc.y, cellFCost[dc.x, dc.y]);
        }

        discoveredCells.Remove(new Vector2Int((int)lowestFCost.x, (int)lowestFCost.y));
        // 2
        //301
        // 4
        CheckNextAStarPath(ref obstacleMap, ref cellFCost, ref discoveredCells, ref dir, from + Vector2Int.right, to, pathStepCount + 1, pathStepCap, 3, ref endFound);
        CheckNextAStarPath(ref obstacleMap, ref cellFCost, ref discoveredCells, ref dir, from + Vector2Int.up, to, pathStepCount + 1, pathStepCap, 4, ref endFound);
        CheckNextAStarPath(ref obstacleMap, ref cellFCost, ref discoveredCells, ref dir, from + Vector2Int.left, to, pathStepCount + 1, pathStepCap, 1, ref endFound);
        CheckNextAStarPath(ref obstacleMap, ref cellFCost, ref discoveredCells, ref dir, from + Vector2Int.down, to, pathStepCount + 1, pathStepCap, 2, ref endFound);
    }
    public void TracePath(ref List<Vector2Int> path, ref int[,] dir, int x, int y)
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
