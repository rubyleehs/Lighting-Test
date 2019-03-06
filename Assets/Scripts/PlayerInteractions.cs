using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public Transform test;
    public Vector2Int testFromPos;
    public Vector2Int testToPos;
    public List<Vector2Int> path;

    public Character selectedChar;

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

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit2D hit = Physics2D.Raycast(MainCamera.mousePos, Vector2.zero);

            if (hit == false && selectedChar != null)
            {
                StartCoroutine(selectedChar.Travel(PathFinder.RequestAStarPath(ref GameManager.areaStats.tiles, selectedChar.stats.tileIndex, GetMouseTileSpace(), new List<int>(), 30)));
            }
            else if (hit && hit.transform.CompareTag("Character"))
            {
                if (selectedChar != null) selectedChar.Deselect();
                selectedChar = hit.transform.GetComponent<Character>();
                selectedChar.Select();
            }
        }
    }

    public Vector2Int GetMouseTileSpace()
    {
        Vector2 relativeMousePos = MainCamera.mousePos / GameManager.areaStats.cellSize;
        return new Vector2Int(Mathf.RoundToInt(relativeMousePos.x), Mathf.RoundToInt(relativeMousePos.y));
    }
}
