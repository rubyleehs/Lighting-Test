using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaGen : MonoBehaviour
{
    public int minSubAreaSize = 6;
    public int maxSplitVariance = 5;

    public SubArea mainSubArea;

    public void GenerateNewArea()
    {
        GameManager.areaStats.tiles = new int[GameManager.areaStats.size.x, GameManager.areaStats.size.y];
        mainSubArea = new SubArea
        {
            size = new Vector2Int(GameManager.areaStats.size.x - 4, GameManager.areaStats.size.y - 4),
            anchor = Vector2Int.one * 2,
        };
        MaxSplit(mainSubArea);
        Place(mainSubArea);
        Replace(1, 0);
        PlaceAll(mainSubArea);
        Replace(0, -1);
        ConnectAll(mainSubArea);
    }
    
    void MaxSplit(SubArea a0)
    {
        if(a0.childsSubArea == null)
        {
            a0.SplitArea(maxSplitVariance, minSubAreaSize);

            for (int i = 0; i < a0.childsSubArea.Count; i++)
            {
                MaxSplit(a0.childsSubArea[i]);
            }
        }
    }

    void PlaceAll(SubArea a0)
    {
        if (a0.childsSubArea.Count == 0)
        {
            Place(a0);
        }
        else
        {
            for (int i = 0; i < a0.childsSubArea.Count; i++)
            {
                PlaceAll(a0.childsSubArea[i]);
            }
        }

    }

    void Place(SubArea a0)
    {
        for (int dy = -1; dy < a0.size.y + 1; dy++)
        {
            for (int dx = -1; dx < a0.size.x +1; dx++)
            {
                if (a0.isTooSmall)
                {
                    GameManager.areaStats.tiles[a0.anchor.x + dx, a0.anchor.y + dy] = 1;
                }
                else
                {
                    if ((dx == -1) || (dy == -1) || (dx == a0.size.x) || (dy == a0.size.y)) GameManager.areaStats.tiles[a0.anchor.x + dx, a0.anchor.y + dy] = -1;
                    else GameManager.areaStats.tiles[a0.anchor.x + dx, a0.anchor.y + dy] = 1;
                }
            }
        }
    }

    void Replace(int from, int to)
    {
        for (int dy = 0; dy < GameManager.areaStats.tiles.GetLength(1); dy++)
        {
            for (int dx = 0; dx < GameManager.areaStats.tiles.GetLength(0); dx++)
            {
                if (GameManager.areaStats.tiles[dx, dy] == from) GameManager.areaStats.tiles[dx, dy] = to;
            }
        }
    }

    void ConnectAll(SubArea a0)
    {
        for (int i = 0; i < a0.childsSubArea.Count; i++)
        {
            ConnectAll(a0.childsSubArea[i]);
        }
        Connect(a0);
    }

    void Connect(SubArea a)
    {
        a.connection = new List<Vector2Int>();
        if (a.childsSubArea.Count != 2) return;
        SubArea a0 = a.childsSubArea[0];
        SubArea a1 = a.childsSubArea[1];
        int dy = 0;
        int dx = 0;
        int fs = 0;

        if (a0.anchor.y == a1.anchor.y) dy = Random.Range(1, a1.size.y -1);
        else dx = Random.Range(1, a1.size.x -1);
        if(!a1.isTooSmall) a.connection.Add(new Vector2Int(a1.anchor.x + dx, a1.anchor.y + dy));

        while (true)
        {
            fs++;
            if (a0.anchor.y == a1.anchor.y) dx--;
            else dy--;

            if (a1.anchor.x + dx == 1|| a1.anchor.y + dy == 1 || (GameManager.areaStats.tiles[a1.anchor.x + dx, a1.anchor.y + dy] > 0 && fs > 1)) break;
            else
            {
                a.connection.Add(new Vector2Int(a1.anchor.x + dx, a1.anchor.y + dy));
            }
        }

        for (int i = 0; i < a.connection.Count; i++)
        {
            GameManager.areaStats.tiles[a.connection[i].x, a.connection[i].y] = 2;
        }
    }
}

[System.Serializable]
public class SubArea
{
    public SubArea parentSubArea;
    public List<SubArea> childsSubArea;
    public List<Vector2Int> connection;
    public Vector2Int anchor;
    public Vector2Int size;
    public bool isTooSmall = false;

    public void SplitArea(int maxSplitVariance, int minSubAreaSize)
    {
        childsSubArea = new List<SubArea>();
        if (size.x < 2* minSubAreaSize || size.y < 2 * minSubAreaSize)
        {
            if(size.x < minSubAreaSize || size.y < minSubAreaSize) isTooSmall = true;
            return;
        }

        bool splitX = Random.Range(0, size.x + size.y) > size.y;
        int variance = Random.Range(-maxSplitVariance, maxSplitVariance + 1);

        childsSubArea.Add(new SubArea());
        childsSubArea.Add(new SubArea());
        childsSubArea[0].parentSubArea = this;
        childsSubArea[1].parentSubArea = this;

        if (splitX)
        {
            childsSubArea[0].size = new Vector2Int(Mathf.FloorToInt(size.x * 0.5f) + variance - 1, size.y);
            childsSubArea[0].anchor = anchor;

            childsSubArea[1].size = new Vector2Int(Mathf.FloorToInt(size.x * 0.5f) - variance - 1, size.y);
            childsSubArea[1].anchor = anchor + Vector2Int.right * (childsSubArea[0].size.x + 2);
        }
        else
        {
            childsSubArea[0].size = new Vector2Int(size.x, Mathf.FloorToInt(size.y * 0.5f) + variance - 1);
            childsSubArea[0].anchor = anchor;

            childsSubArea[1].size = new Vector2Int(size.x, Mathf.FloorToInt(size.y * 0.5f) - variance - 1);
            childsSubArea[1].anchor = anchor + Vector2Int.up * (childsSubArea[0].size.y + 2);
        }
    }
}