using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaVisuals : MonoBehaviour
{
    public static List<Transform> activeHighlightTiles;
    public static List<Transform> inactiveHighlightTiles;
    public GameObject hightlightTile;//add tag. if player click on check if blue/red. if blue move, red move + attack. if click anothe rcharacter switch slected char, if hit nothing, deselect

    public int spritesPixelSize;

    public Transform meshParent;

    public Transform decoParent;
    public GameObject decoGO;
    public GameObject decoLightGO;
    public Chance chance;

    public MeshLayer[] meshLayer;
    public SpriteArray[] decoArch; //0 is hori, 1 is verti
    public SpriteArray[] decoTorch;

    protected List<Vector3> vertices;
    protected List<int> triangles;
    protected List<Vector2> uvs;

    new Transform transform;
    private Vector2 uvPerCell;
    private List<Transform> decoList;
    private int[] primes = new int[] { 2, 3, 5, 7};
    private int[] multiples = new int[] { 1, 2, 3, 5, 6, 7, 10, 14, 15, 21, 30, 35, 42, 70, 105, 210 };

    public void Init()
    {
        activeHighlightTiles = new List<Transform>();
        inactiveHighlightTiles = new List<Transform>();
        decoList = new List<Transform>();

        if (transform == null) transform = GetComponent<Transform>();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        uvPerCell = new Vector2(1f / (GameManager.areaStats.size.x), 1f / (GameManager.areaStats.size.y));

        for (int y = 0; y < GameManager.areaStats.size.y; y++)
        {
            for (int x = 0; x < GameManager.areaStats.size.x; x++)
            {
                TriangulateCell(new Vector2Int(x, y));
            }
        }
        Apply();
        meshParent.position = -Vector2.one * GameManager.areaStats.cellSize * 0.5f; 
    }

    public void RefreshVisuals()
    {
        CalculateTilesMeta();
        CreateAreaTex();
        ClearDeco();
        PlaceDeco();
    }

    public void CalculateTilesMeta()
    {
        GameManager.areaStats.tilesMeta = new int[GameManager.areaStats.size.x , GameManager.areaStats.size.y];
        for (int y = 1; y < GameManager.areaStats.size.y - 1; y++)
        {
            for (int x = 1; x < GameManager.areaStats.size.x - 1; x++)
            {
                int primedID = 1;
                bool isSolid = GameManager.areaStats.tiles[x, y] < 0;
                if ((GameManager.areaStats.tiles[x + 1, y] < 0) == isSolid) primedID *= primes[0];
                if ((GameManager.areaStats.tiles[x, y + 1] < 0) == isSolid) primedID *= primes[1];
                if ((GameManager.areaStats.tiles[x - 1, y] < 0) == isSolid) primedID *= primes[2];
                if ((GameManager.areaStats.tiles[x, y - 1] < 0) == isSolid) primedID *= primes[3];

               GameManager.areaStats.tilesMeta[x, y] = System.Array.IndexOf(multiples, primedID);
            }
        }
    }

    public void CreateAreaTex()
    {
        for (int i = 0; i < meshLayer.Length; i++)
        {
            meshLayer[i].mainTex.Resize(GameManager.areaStats.size.x * spritesPixelSize, GameManager.areaStats.size.y * spritesPixelSize);

            for (int y = 0; y < GameManager.areaStats.size.y; y++)
            {
                for (int x = 0; x < GameManager.areaStats.size.x; x++)
                {
                    int id = GameManager.areaStats.tiles[x, y];
                    int metaID = GameManager.areaStats.tilesMeta[x, y];
                    Color[] spriteTex = null;

                    if ((id > 0 && meshLayer[i].pSprites[id - 1].metaSprites.Length <= metaID) || (id < 0 && meshLayer[i].nSprites[-id - 1].metaSprites.Length <= metaID)) metaID = 0;

                    if (id > 0) spriteTex = meshLayer[i].pSprites[id - 1].metaSprites[metaID].texture.GetPixels(0, 0, spritesPixelSize, spritesPixelSize);
                    else if (id < 0) spriteTex = meshLayer[i].nSprites[-id - 1].metaSprites[metaID].texture.GetPixels(0, 0, spritesPixelSize, spritesPixelSize);

                    meshLayer[i].mainTex.SetPixels(x * spritesPixelSize, y * spritesPixelSize, spritesPixelSize, spritesPixelSize, spriteTex);
                }
            }

            meshLayer[i].mainTex.Apply();
        }
    }

    public void PlaceDeco()
    {
        for (int y = 0; y < GameManager.areaStats.size.y; y++)
        {
            for (int x = 0; x < GameManager.areaStats.size.x; x++)
            {
                switch (GameManager.areaStats.tiles[x, y])
                {
                    case 1:
                        PlaceTorch(x, y);
                        break;
                    case 2:
                        PlaceArch(x,y);
                        break;
                    default: break;
                }
            }
        }
    }

    public void ClearDeco()
    {
        for (int i = 0; i < decoList.Count; i++)
        {
            Destroy(decoList[i].gameObject);
        }
        decoList.Clear();
    }

    public void PlaceArch(int x, int y)
    {
        if (Random.Range(0f, 1f) > chance.arch) return;
        int _mID = GameManager.areaStats.tilesMeta[x, y];
        Sprite s = null;
        switch (_mID)
        {
            case 6: s = decoArch[0].metaSprites[Random.Range(0, decoArch[0].metaSprites.Length)];
                break;
            case 9: s = decoArch[1].metaSprites[Random.Range(0, decoArch[1].metaSprites.Length)];
                break;
            default: break;
        }
        
        if(s != null)
        {
            SpriteRenderer sr = Instantiate(decoGO, new Vector2(x, y) * GameManager.areaStats.cellSize, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            sr.transform.localScale *= GameManager.areaStats.cellSize;
            sr.sprite = s;
            decoList.Add(sr.transform);
        }
    }

    public void PlaceTorch(int x,int y)
    {
        if (Random.Range(0f, 1f) > chance.torch) return;
        int _mID = GameManager.areaStats.tilesMeta[x, y];
        Sprite s = null;
        switch (_mID)
        {
            case 13:
                s = decoTorch[0].metaSprites[Random.Range(0, decoTorch[0].metaSprites.Length)];
                break;

            default: break;
        }

        if (s != null)
        {
            SpriteRenderer sr = Instantiate(decoLightGO, new Vector2(x, y) * GameManager.areaStats.cellSize, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            sr.transform.localScale *= GameManager.areaStats.cellSize;
            sr.sprite = s;
            decoList.Add(sr.transform);
        }
    }

    public void TriangulateCell(Vector2Int sw) //, MapNode se, MapNode nw, MapNode ne)
    {
        int _vertexIndex = vertices.Count;

        AddQuad((Vector2)sw * GameManager.areaStats.cellSize, (sw + Vector2.up) * GameManager.areaStats.cellSize, (sw + Vector2.one) * GameManager.areaStats.cellSize, (sw + Vector2.right) * GameManager.areaStats.cellSize);

        uvs.Add(Vector2.Scale(sw, uvPerCell));
        uvs.Add(Vector2.Scale(sw + Vector2.up, uvPerCell));
        uvs.Add(Vector2.Scale(sw + Vector2.one, uvPerCell));
        uvs.Add(Vector2.Scale(sw + Vector2.right, uvPerCell));
    }

    public virtual void Apply()
    {
        for (int i = 0; i < meshLayer.Length; i++)
        {
            meshLayer[i].meshFilter.mesh.vertices = vertices.ToArray();
            meshLayer[i].meshFilter.mesh.triangles = triangles.ToArray();
            meshLayer[i].meshFilter.mesh.uv = uvs.ToArray();
        }
    }

    public virtual void Clear()
    {
        for (int i = 0; i < meshLayer.Length; i++)
        {
            meshLayer[i].meshFilter.mesh.Clear();
        }
        vertices.Clear();
        triangles.Clear();
    }

    protected virtual void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    public void HighlightTilesInRange(Vector2Int center, int range)
    {
        float[,] minifiedMap = new float[2 * range + 1, 2 * range + 1];
        for (int dy = -range; dy < range; dy++)
        {
            for (int dx = -range; dx < range; dx++)
            {
                if (center.x + dx < 0 || center.y + dy < 0 || center.x + dx > GameManager.areaStats.size.x || center.y + dy > GameManager.areaStats.size.y) minifiedMap[range + dx, range + dy] = (int)PathFinder.untravellableDijkValue;
                else if (GameManager.areaStats.tiles[center.x + dx, center.y + dy] < 0) minifiedMap[range + dx, range + dy] = (int)PathFinder.untravellableDijkValue; 

            }
        }
        PathFinder.DijkFloodStep(ref minifiedMap, center.x, center.y, 1, 1, range);
        for (int dy = -range; dy < range; dy++)
        {
            for (int dx = -range; dx < range; dx++)
            {
                Debug.Log(minifiedMap[dx + range, dy + range]);
                if (minifiedMap[dx + range, dy + range] > 0) SummonHighlightTile(center + new Vector2Int(dx, dy), new Color(255, 0, 0, 0.5f));
            }
        }
    }

    public void SummonHighlightTile(Vector2Int index, Color color)
    {
        Transform tile;
        if (inactiveHighlightTiles.Count == 0)
        {
            tile = Instantiate(hightlightTile, (Vector2)index * GameManager.areaStats.cellSize, Quaternion.identity, null).transform;
            tile.localScale *= GameManager.areaStats.cellSize;
            tile.GetComponent<SpriteRenderer>().color = color;
            activeHighlightTiles.Add(tile);
        }
        else
        {
            tile = inactiveHighlightTiles[0];
            inactiveHighlightTiles.RemoveAt(0);
            tile.position = (Vector2)index * GameManager.areaStats.cellSize;
            tile.gameObject.SetActive(true);
            tile.GetComponent<SpriteRenderer>().color = color;
            activeHighlightTiles.Add(tile);
        }
    }
}

[System.Serializable]
public struct SpriteArray
{
    public Sprite[] metaSprites;
}

[System.Serializable]
public struct MeshLayer
{
    //public Mesh mesh;
    public MeshFilter meshFilter;
    public Texture2D mainTex;
    public SpriteArray[] pSprites;
    public SpriteArray[] nSprites;
}

[System.Serializable]
public struct Chance
{
    [Range(0,1)]
    public float arch;
    [Range(0, 1)]
    public float torch;
}
