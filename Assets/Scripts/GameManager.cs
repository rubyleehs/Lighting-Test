using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AreaStats
{
    public Vector2Int size;
    public float cellSize;
    //have ANOTHER class/SO for storing diffrent tiles, their sprites, and what they obstruct
    [Header("Visual Maps")]//swap to start TileMap[,] where TileMap has tile type, tile meta, character map, highlighted
    [HideInInspector] public int[,] tiles; // -1 wall, 0 unchecked, 1 room, 2 corridor //All obstacles are negative. eg.water, doors 
    [HideInInspector] public int[,] tilesMeta;

    [HideInInspector] public float[,] basicDijkstra; //player char start with 10, have flood fill have a limit, walls are -100
    [HideInInspector] public int[,] characterMaps; //store all characters. So if pathfind > 1 step, if land on a character can either take a step back of attack etc
}

public class GameManager : MonoBehaviour
{
    public GameObject tempChar;
    public static List<Character> chars;

    public static AreaStats areaStats;
    public AreaStats I_areaStats;

    public static AreaGen areaGen;
    public AreaGen I_areaGen;

    public static AreaVisuals areaVis;
    public AreaVisuals I_areaVis;

    private WaitForEndOfFrame waitEF;

    private void Awake()
    {
        areaStats = I_areaStats;
        areaGen = I_areaGen;
        areaVis = I_areaVis;
        chars = new List<Character>();
        waitEF = new WaitForEndOfFrame();

        StartCoroutine(InitGame());
        StartCoroutine(InitLevel());
    }

    IEnumerator InitGame()
    {
        areaVis.Init();
        yield return waitEF;
    }

    IEnumerator InitLevel()
    {
        yield return StartCoroutine(CreateArea());

        yield return StartCoroutine(CreateUnits());
    }

    IEnumerator CreateArea()
    {
        areaGen.GenerateNewArea();
        areaVis.RefreshVisuals();

        yield return waitEF;
    }

    IEnumerator CreateUnits()
    {
        Character c = Instantiate(tempChar,null).GetComponent<Character>();
        c.stats.tileIndex = Vector2Int.one * 10;
        c.transform.position = Vector2.one * 10;
        chars.Add(c);
        //create Player
        //create enemies
        yield return waitEF;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CreateArea());
        }
    }

}
