using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public Transform test;
    public Vector2Int testFromPos;
    public Vector2Int testToPos;
    public List<Vector2Int> path;
    void Update()
    {
        test.position = (Vector2)GetMouseTileSpace();
        if (Input.GetKeyDown(KeyCode.F)) testFromPos = GetMouseTileSpace();
        if (Input.GetKeyDown(KeyCode.T)) testToPos = GetMouseTileSpace();

        if (Input.GetKeyDown(KeyCode.R)) path = PathFinder.RequestAStarPath(ref GameManager.areaStats.tiles, testFromPos, testToPos, new List<int>(), 30);
        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine((Vector2)path[i] * GameManager.areaStats.cellSize, (Vector2)path[i + 1] * GameManager.areaStats.cellSize);
            }
        }
    }

    public Vector2Int GetMouseTileSpace()
    {
        Vector2 relativeMousePos = MainCamera.mousePos / GameManager.areaStats.cellSize;
        return new Vector2Int(Mathf.RoundToInt(relativeMousePos.x), Mathf.RoundToInt(relativeMousePos.y));
    }
}
