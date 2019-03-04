using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AreaStats
{
    public Vector2Int size;
    public float cellSize;

    [Header("Visual Maps")]
    [HideInInspector] public int[,] tiles; // -1 wall, 0 unchecked, 1 room, 2 corridor //All obstacles are negative. eg.water, doors 
    [HideInInspector] public int[,] tilesMeta;

    [HideInInspector] public int[,] basicDijkstra;
    //player char start with 10, have flood fill have a limit, walls are -100
}

public class GameManager : MonoBehaviour
{
    public static AreaStats areaStats;
    public AreaStats I_areaStats;

    public AreaGen areaGen;
    public AreaVisuals areaVis;

    private WaitForEndOfFrame waitEF;

    private void Awake()
    {
        areaStats = I_areaStats;
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
