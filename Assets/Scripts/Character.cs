using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterStats
{
    public float speed;

    public Vector2Int tileIndex;
    public int moveRange;
    public Vector2Int attackRange;
    //add ignore obstacles after you get ALL chars working!
}

[RequireComponent(typeof(Collider2D))]
public class Character : MonoBehaviour
{
    public new Transform transform;

    public CharacterStats stats;

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    public virtual void Select()
    {
        Debug.Log(transform.name + " Selected");
        GameManager.areaVis.HighlightTilesInRange(stats.tileIndex, stats.moveRange);
    }

    public virtual void Deselect()
    {
        Debug.Log(transform.name + " Deselected");
        GameManager.areaVis.DesummonAllHighlightTiles();
    }

    public virtual IEnumerator Travel(List<Vector2Int> travelNodes)
    {
        for (int i = 0; i < travelNodes.Count; i++)
        {
            Vector2 endPos = (Vector2)travelNodes[i] * GameManager.areaStats.cellSize;
            while (((Vector2)transform.position - endPos).sqrMagnitude > 0.5f)
            {
                transform.Translate((endPos - (Vector2)transform.position).normalized * stats.speed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            transform.position = new Vector3(endPos.x, endPos.y, transform.position.z);
            stats.tileIndex = travelNodes[i];
        }
    }

    public virtual IEnumerator Move(Vector2 endPos)
    {
        while(((Vector2)transform.position - endPos).sqrMagnitude > 0.5f)
        {
            transform.Translate((endPos - (Vector2)transform.position).normalized * stats.speed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(endPos.x, endPos.y, transform.position.z);
    }
}
