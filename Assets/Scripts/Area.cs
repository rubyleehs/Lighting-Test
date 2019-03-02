using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public Transform debugWallParent;
    public GameObject debugWall;
    public Vector2Int areaSize;

    public int minSubAreaSize = 6;
    public int maxSplitVariance = 5;

    public int[,] tiles;
    public SubArea mainSubArea;


    // Start is called before the first frame update
    void Start()
    {
        tiles = new int[areaSize.x, areaSize.y];
        mainSubArea = new SubArea
        {
            size = new Vector2Int(areaSize.x -4, areaSize.y -4),
            anchor = Vector2Int.one * 2,
        };
        MaxSplit(mainSubArea);
        Place(mainSubArea);
        Replace(1, 0);
        PlaceAll(mainSubArea);
        Replace(0, 2);
        Show();
    }
    
    void MaxSplit(SubArea a0)
    {
        if(a0.childSubAreas == null)
        {
            a0.SplitArea(maxSplitVariance, minSubAreaSize);

            for (int i = 0; i < a0.childSubAreas.Count; i++)
            {
                MaxSplit(a0.childSubAreas[i]);
            }
        }
    }

    void PlaceAll(SubArea a0)
    {
        if (a0.childSubAreas.Count == 0)
        {
            Place(a0);
        }
        else
        {
            for (int i = 0; i < a0.childSubAreas.Count; i++)
            {
                PlaceAll(a0.childSubAreas[i]);
            }
        }

    }

    void Place(SubArea a0)
    {
        for (int dy = -1; dy < a0.size.y + 1; dy++)
        {
            for (int dx = -1; dx < a0.size.x +1; dx++)
            {
                if((dx == -1) || (dy == -1) || (dx == a0.size.x) || (dy == a0.size.y)) tiles[a0.anchor.x + dx, a0.anchor.y + dy] = -1;
                else tiles[a0.anchor.x + dx, a0.anchor.y + dy] = 1;
            }
        }
    }

    void Replace(int from, int to)
    {
        for (int dy = 0; dy < tiles.GetLength(1); dy++)
        {
            for (int dx = 0; dx < tiles.GetLength(0); dx++)
            {
                if (tiles[dx, dy] == from) tiles[dx, dy] = to;
            }
        }
    }

    void Show()
    {
        for (int dy = 0; dy < tiles.GetLength(1); dy++)
        {
            for (int dx = 0; dx < tiles.GetLength(0); dx++)
            {
                if (tiles[dx, dy] == -1) Instantiate(debugWall,new Vector2(dx,dy),Quaternion.identity,debugWallParent);
            }
        }
    }
}

[System.Serializable]
public class SubArea
{
    public SubArea parentSubArea;
    public List<SubArea> childSubAreas;
    public Vector2Int anchor;
    public Vector2Int size;

    public void SplitArea(int maxSplitVariance, int minSubAreaSize)
    {
        bool splitX = Random.Range(0, size.x + size.y) > size.y;
        int variance = Random.Range(-maxSplitVariance, maxSplitVariance + 1);
        childSubAreas = new List<SubArea>
        {
            new SubArea(), new SubArea(),
        };

        childSubAreas[0].parentSubArea = this;
        childSubAreas[1].parentSubArea = this;

        if (splitX)
        {
            childSubAreas[0].size = new Vector2Int(Mathf.FloorToInt(size.x * 0.5f) + variance - 1, size.y);
            childSubAreas[0].anchor = anchor;

            childSubAreas[1].size = new Vector2Int(Mathf.FloorToInt(size.x * 0.5f) - variance - 1, size.y);
            childSubAreas[1].anchor = anchor + Vector2Int.right * (childSubAreas[0].size.x + 2);
        }
        else
        {
            childSubAreas[0].size = new Vector2Int(size.x, Mathf.FloorToInt(size.y * 0.5f) + variance - 1);
            childSubAreas[0].anchor = anchor;

            childSubAreas[1].size = new Vector2Int(size.x, Mathf.FloorToInt(size.y * 0.5f) - variance - 1);
            childSubAreas[1].anchor = anchor + Vector2Int.up * (childSubAreas[0].size.y + 2);
        }

        for (int i = 0; i < childSubAreas.Count; i++)
        {
            if (childSubAreas[i].size.x < minSubAreaSize || childSubAreas[i].size.y < minSubAreaSize)
            {
                childSubAreas.RemoveAt(i);
                i--;
            }
        }
    }
}