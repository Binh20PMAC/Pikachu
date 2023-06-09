﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private ButtonManager pikachuManager;
    [SerializeField] private GameObject linePikachu;
    [SerializeField] private GameObject prefapPikachu;
    [SerializeField] private CountdownTime countdown;
    [SerializeField] public GameObject[,] mapPikachu { get; private set; }
    [SerializeField] private int height;
    [SerializeField] private int width;
    public int row { get; private set; }
    public int col { get; private set; }
    public int score { get; private set; }
    public int level { get; private set; } = 1;


    private Vec2 pos1;
    private Vec2 pos2;

    private Vector2[][] pos;
    private Color opacity = new Color(255f, 255f, 255f, .5f);
    private Color defaultColor = new Color(255f, 255f, 255f, 1.0f);

    private int[,] mapAll;
    private int minX;
    private int minY;
    private int cellWidth = 28;
    private int cellHeight = 32;
    private int map = 0;

    private const string change = "change";
    private void Start()
    {
        Pikachu(height + 2, width + 2);
        RandomMap();
        map = ((row - 2) * (col - 2)) / 2;
        pikachuManager.PlaySFX(change);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float x = (Input.mousePosition.x - Screen.width / 2) / (Screen.width * ((float)Screen.width / Screen.height));
            float y = (Input.mousePosition.y - Screen.height / 2) / (Screen.height);

            if (Screen.width == 1920)
                x *= 1010;
            else
                x *= 820;

            y *= 320;
            int mouseCol = (int)(x - minX) / (cellWidth);
            int mouseRow = (int)(y - minY) / (cellHeight);

            if (mouseCol < 0 || mouseCol > col - 1 || mouseRow < 0 || mouseRow > row - 1)
            {
                mouseRow = 0;
                mouseCol = 0;
            }

            if (pos1 != null && pos1.C == mouseCol && pos1.R == mouseRow && mapAll[mouseRow, mouseCol] == 0)
            {
                DeSelect();
            }
            else if (pos1 == null && mapAll[mouseRow, mouseCol] == 0)
            {
                Select(new Vec2(mouseRow, mouseCol));
            }
            else if (mapAll[mouseRow, mouseCol] == 0)
            {
                CheckPair(new Vec2(mouseRow, mouseCol));
            }
        }

    }

    private void Select(Vec2 pos)
    {
        pos1 = new Vec2(pos.R, pos.C);
        mapPikachu[pos1.R, pos1.C].GetComponent<SpriteRenderer>().color = opacity;
        Debug.Log("selected " + pos1.Print());
    }
    private void DeSelect()
    {
        mapPikachu[pos1.R, pos1.C].GetComponent<SpriteRenderer>().color = defaultColor;
        pos1 = null;
        pos2 = null;
        Debug.Log("DeSelect");
        pikachuManager.PlaySFX("e-oh");
    }
    private void CheckPair(Vec2 pos)
    {
        pos2 = new Vec2(pos.R, pos.C);
        Debug.Log("CheckPair " + pos1.Print() + " and " + pos2.Print());
        if (mapPikachu[pos1.R, pos1.C].GetComponent<SpriteRenderer>().sprite.name == mapPikachu[pos2.R, pos2.C].GetComponent<SpriteRenderer>().sprite.name)
        {
            mapAll[pos1.R, pos1.C] = -1;
            mapAll[pos2.R, pos2.C] = -1;
            List<Vec2> checkFindPath = FindPath(pos1, pos2);
            if (mapAll[pos2.R + 1, pos2.C] == 0 && mapAll[pos2.R - 1, pos2.C] == 0 && mapAll[pos2.R, pos2.C + 1] == 0 && mapAll[pos2.R, pos2.C - 1] == 0)
            {
                Default();
            }
            else if (checkFindPath != null)
            {
                Line(checkFindPath.Count);

                if (mapPikachu[pos1.R, pos1.C].GetComponent<SpriteRenderer>().sprite.name == mapPikachu[pos2.R, pos2.C].GetComponent<SpriteRenderer>().sprite.name && mapAll[pos2.R, pos2.C] == -1)
                {
                    (mapPikachu[pos1.R, pos1.C]).SetActive(false);
                    (mapPikachu[pos2.R, pos2.C]).SetActive(false);
                    mapAll[pos1.R, pos1.C] = -1;
                    mapAll[pos2.R, pos2.C] = -1;
                    pos1 = null;
                    pos2 = null;
                    score += 10;
                    pikachuManager.PlaySFX("point");
                    --map;
                    if (map == 0)
                    {
                        StartCoroutine(WaitForNextLevel());
                    }
                    if (AutoChange() == true && map > 0)
                    {
                        pikachuManager.ChangeMap();
                    }
                }
                else Default();
            }
            else
            {
                Default();
            }
        }
        else
        {
            Default();
        }

    }

    private void Line(int count)
    {
        linePikachu.GetComponent<LineRenderer>().positionCount = count;
        int counts = 0;

        foreach (Vec2 line in FindPath(pos1, pos2))
        {
            linePikachu.GetComponent<LineRenderer>().SetPosition(counts++, pos[line.R][line.C]);
        }
        linePikachu.SetActive(true);
        if (linePikachu.activeInHierarchy)
        {
            StartCoroutine(WaitForLineOff());
        }
    }

    IEnumerator WaitForLineOff()
    {
        yield return new WaitForSeconds(0.5f);
        linePikachu.SetActive(false);
    }
    IEnumerator WaitForNextLevel()
    {
        yield return new WaitForSeconds(1f);
        ++level;
        Pikachu(height + 2, width + 2);
        RandomMap();
        countdown.SetTime(1f);
        map = (row - 2) * (col - 2) / 2;
        pikachuManager.PlaySFX(change);
    }
    private void Default()
    {
        mapAll[pos1.R, pos1.C] = 0;
        mapAll[pos2.R, pos2.C] = 0;
        mapPikachu[pos1.R, pos1.C].GetComponent<SpriteRenderer>().color = defaultColor;
        mapPikachu[pos2.R, pos2.C].GetComponent<SpriteRenderer>().color = defaultColor;
        pos1 = null;
        pos2 = null;
        pikachuManager.PlaySFX("e-oh");
    }

    private GameObject AddPikachu(int type, Vector2 pos, int width, int height)
    {
        GameObject g = Instantiate(prefapPikachu) as GameObject;
        g.transform.parent = this.transform;
        g.transform.position = pos;
        Sprite sprite = Resources.Load("Images/item/item" + type, typeof(Sprite)) as Sprite;

        g.GetComponent<SpriteRenderer>().sprite = sprite;
        g.transform.localScale = new Vector3(Mathf.Abs(width * 1.0f / sprite.bounds.size.x), Mathf.Abs(-height * 1.0f / sprite.bounds.size.y), 1);
        return g;
    }

    private void RandomMap()
    {
        int dem = 0;
        for (int i = 1; i < row - 1; i++)
            for (int j = 1; j < col - 1; j++)
            {
                mapAll[i, j] = 0;
                dem++;
            }

        if (dem % 2 == 1)
        {
            Debug.LogError("Error");
            return;
        }

        int[] pool = new int[dem];

        for (int i = 0; i < dem / 2; i++)
            pool[i] = Random.Range(0, 35);
        for (int i = dem / 2; i < dem; i++)
            pool[i] = pool[dem - 1 - i];


        for (int i = 0; i < dem / 2; i++)
        {
            int index1 = Random.Range(0, dem);
            int index2 = Random.Range(0, dem);
            int temp = pool[index1];
            pool[index1] = pool[index2];
            pool[index2] = temp;
        }
        for (int i = 1; i < row - 1; i++)
        {
            for (int j = 1; j < col - 1; j++)
            {
                mapPikachu[i, j] = AddPikachu(pool[--dem], pos[i][j], cellWidth, cellHeight);
            }
        }
    }

    private void Pikachu(int rowPikachu, int colPikachu)
    {
        cellHeight = (int)(50f / (row - 2));
        cellWidth = (int)(cellHeight * 0.65f);

        row = rowPikachu;
        col = colPikachu;
        mapPikachu = new GameObject[row, col];
        mapAll = new int[row, col];
        pos = new Vector2[row][];

        minX = -(col) * cellWidth / 2;
        minY = -(row) * cellHeight / 2;

        for (int i = 0; i < row; i++)
        {
            pos[i] = new Vector2[col];
            for (int j = 0; j < col; j++)
            {
                mapAll[i, j] = -1;

                pos[i][j] = new Vector3(0, 0, 0);
                pos[i][j].x = minX + j * cellWidth + cellWidth / 2;
                pos[i][j].y = minY + i * cellHeight + cellHeight / 2;
            }
        }
    }

    private List<Vec2> FindPath(Vec2 start, Vec2 end)
    {
        List<Vec2> openNeighborList = GetNeighbors(start);
        int fastDistance = Vec2.FastDistance(start, end);
        foreach (Vec2 open in openNeighborList)
        {
            if (open.R == end.R && open.C == end.C)
            {
                end.parent = start;
                return Vec2.GeneratePath(end);
            }
        }
        return Find(openNeighborList, start, end);
    }

    private List<Vec2> Find(List<Vec2> openNeighborList, Vec2 start, Vec2 end)
    {
        List<Vec2> openList = new List<Vec2>();
        for (int i = 0; i < openNeighborList.Count; i++)
        {
            if (end.C > start.C && start.C - 1 == openNeighborList[i].C && openNeighborList[i].R == end.R)
            {
                openNeighborList.Remove(openNeighborList[i]);
            }
            else if (end.C < start.C && start.C + 1 == openNeighborList[i].C && openNeighborList[i].R == end.R)
            {
                openNeighborList.Remove(openNeighborList[i]);
            }
        }

        foreach (Vec2 open in openNeighborList)
        {
            List<Vec2> find;
            int vertical; // Up Down
            int horizontal = 1; // Right
            if (end.C > start.C) // Left
            {
                horizontal = -1;
            }

            if ((start.C - 1) == open.C) // Right
            {
                int right = 1;
                if (end.R > open.R) // Down
                {
                    vertical = -1;
                }
                else vertical = 1;
                find = FindLeftAndRight(open, start, end, vertical, right);
                if (find != null)
                    return find;
            }
            else if ((start.C + 1) == open.C) // Left
            {
                int left = -1;
                if (end.R > open.R) // Down
                {
                    vertical = -1;
                }
                else vertical = 1;
                find = FindLeftAndRight(open, start, end, vertical, left);
                if (find != null)
                    return find;
            }
            else if ((start.R - 1) == open.R) // Up
            {
                int up = 1;
                find = FindUpAndDown(open, start, end, up, horizontal, openList);
                if (find != null)
                    return find;
            }
            else if ((start.R + 1) == open.R) // Down
            {
                int down = -1;
                find = FindUpAndDown(open, start, end, down, horizontal, openList);
                if (find != null)
                    return find;
            }
        }
        return null;
    }
    private List<Vec2> FindLeftAndRight(Vec2 open, Vec2 start, Vec2 end, int vertical, int horizontal)
    {
        int fastDistance;
        List<Vec2> horizontalCheck = new List<Vec2>();
        if (horizontal < 0) // Left
        {
            fastDistance = Vec2.FastDistance(open, new Vec2(open.R, col - 1));
        }
        else // Right
        {
            fastDistance = Vec2.FastDistance(open, new Vec2(open.R, 0));
        }
        for (int i = 0; i <= fastDistance; i++)
        {
            if (mapAll[open.R, open.C - (i * horizontal)] == 0)
                break;
            else if (open.C - (i * horizontal) == end.C && open.R == end.R)
            {
                horizontalCheck.Add(start);
                horizontalCheck.Add(end);
                return horizontalCheck;
            }
            Vec2.FastDistance(new Vec2(open.R, open.C - (i * horizontal)), new Vec2(end.R, end.C));
            int distanceVertical = Vec2.r;
            for (int j = 1; j <= distanceVertical; j++)
            {
                if (mapAll[open.R - (j * vertical), open.C - (i * horizontal)] == 0)
                    break;
                else if (open.R - (j * vertical) == end.R && open.C - (i * horizontal) == end.C)
                {
                    horizontalCheck.Clear();
                    horizontalCheck.Add(start);
                    horizontalCheck.Add(new Vec2(open.R, open.C - (i * horizontal)));
                    horizontalCheck.Add(end);
                    return horizontalCheck;
                }
                else if (open.R - (j * vertical) == end.R)
                {
                    horizontalCheck.Add(new Vec2(open.R - (j * vertical), open.C - (i * horizontal)));
                }
            }
        }

        if (horizontalCheck.Count > 0)
        {
            foreach (Vec2 checkHorizontal in horizontalCheck)
            {
                if (end.C > checkHorizontal.C)
                {
                    horizontal = -1;
                }
                else horizontal = 1;
                Vec2.FastDistance(checkHorizontal, end);
                for (int i = 1; i <= Vec2.c; i++)
                {
                    if (mapAll[checkHorizontal.R, checkHorizontal.C - (i * horizontal)] == 0)
                        break;
                    else if (checkHorizontal.C - (i * horizontal) == end.C)
                    {
                        horizontalCheck.Clear();
                        horizontalCheck.Add(start);
                        horizontalCheck.Add(new Vec2(start.R, checkHorizontal.C));
                        horizontalCheck.Add(checkHorizontal);
                        horizontalCheck.Add(end);
                        return horizontalCheck;
                    }
                }
            }
        }

        return null;
    }
    private List<Vec2> FindUpAndDown(Vec2 open, Vec2 start, Vec2 end, int vertical, int horizontal, List<Vec2> openList)
    {
        List<Vec2> verticalCheck = new List<Vec2>();
        Vec2.FastDistance(open, end);
        for (int i = 0; i <= Vec2.c; i++)
        {
            if (mapAll[open.R, open.C - (i * horizontal)] == 0) // Right Left check
            {
                if (mapAll[open.R - 1 * vertical, open.C] == -1) // Check open up or down
                {
                    i = 0;
                    open.R -= 1 * vertical;
                }
                else
                {
                    break;
                }
            }
            else if (open.C - (i * horizontal) == end.C)
            {
                openList.Add(start);
                openList.Add(open);
                if (((open.C - (i * horizontal)) == end.C) && (open.R == end.R))
                {
                    openList.Add(end);
                    return openList;
                }
                else
                {
                    verticalCheck.Add(new Vec2(open.R, open.C - (i * horizontal)));
                }
            }
        }
        if (verticalCheck.Count > 0)
        {
            foreach (Vec2 checkVertical in verticalCheck)
            {
                int tempVertical;
                if (end.R > checkVertical.R)
                {
                    tempVertical = vertical;
                    vertical = -1; // Down
                }
                else
                {
                    tempVertical = vertical;
                    vertical = 1;
                } // Up
                Vec2.FastDistance(checkVertical, end);
                for (int i = 1; i <= Vec2.r; i++)
                {
                    if (mapAll[checkVertical.R - (i * vertical), checkVertical.C] == 0)
                    {
                        if (open.R - (1 * tempVertical) >= 0 && open.R - (1 * tempVertical) < mapAll.GetLength(0))
                        {
                            if (mapAll[open.R - (1 * tempVertical), open.C] == 0)
                            {
                                openList.Clear();
                                break;
                            }
                            else
                            {
                                openList.Clear();
                                return FindUpAndDown(new Vec2(open.R - (1 * tempVertical), open.C), start, end, vertical, horizontal, openList);
                            }
                        }
                        else
                        {
                            openList.Clear();
                            break;
                        }

                    }
                    else if (checkVertical.R - (i * vertical) == end.R && checkVertical.C == end.C)
                    {
                        openList.Add(checkVertical);
                        openList.Add(end);
                        return openList;
                    }
                }
            }
        }
        return null;
    }
    private List<Vec2> GetNeighbors(Vec2 node)
    {
        List<Vec2> neighbor = new List<Vec2>();


        if (node.R > 0 && mapAll[node.R - 1, node.C] == -1) // Up
        {
            neighbor.Add(new Vec2(node.R - 1, node.C));
        }
        if (node.R < mapAll.GetLength(0) - 1 && mapAll[node.R + 1, node.C] == -1) // Down
        {
            neighbor.Add(new Vec2(node.R + 1, node.C));
        }
        if (node.C > 0 && mapAll[node.R, node.C - 1] == -1) // Right
        {
            neighbor.Add(new Vec2(node.R, node.C - 1));
        }
        if (node.C < mapAll.GetLength(1) - 1 && mapAll[node.R, node.C + 1] == -1) // Left
        {
            neighbor.Add(new Vec2(node.R, node.C + 1));
        }
        return neighbor;
    }
    private bool AutoChange()
    {
        List<GameObject> mapAutoCheck = new List<GameObject>();
        for (int i = 0; i < mapPikachu.GetLength(0); i++)
        {
            for (int j = 0; j < mapPikachu.GetLength(1); j++)
            {
                GameObject currentObject = mapPikachu[i, j];
                if (currentObject != null && currentObject.gameObject.activeInHierarchy)
                {
                    mapAutoCheck.Add(currentObject);
                }
            }
        }
        foreach (var start in mapAutoCheck)
        {
            foreach (var end in mapAutoCheck)
            {
                if (end.transform.position == start.transform.position)
                {
                    continue;
                }
                var startPikachu = start.GetComponent<SpriteRenderer>().sprite.name;
                var endPikachu = end.GetComponent<SpriteRenderer>().sprite.name;
                if (startPikachu == endPikachu)
                {
                    float deltaStartX = start.transform.position.x - minX - cellWidth / 2;
                    float deltaStartY = start.transform.position.y - minY - cellHeight / 2;
                    int colStart = Mathf.FloorToInt(deltaStartX / cellWidth);
                    int rowStart = Mathf.FloorToInt(deltaStartY / cellHeight);

                    float deltaEndX = end.transform.position.x - minX - cellWidth / 2;
                    float deltaEndY = end.transform.position.y - minY - cellHeight / 2;
                    int colEnd = Mathf.FloorToInt(deltaEndX / cellWidth);
                    int rowEnd = Mathf.FloorToInt(deltaEndY / cellHeight);
                    mapAll[rowEnd, colEnd] = -1;
                    if (FindPath(new Vec2(rowStart, colStart), new Vec2(rowEnd, colEnd)) != null)
                    {
                        mapAll[rowEnd, colEnd] = 0;
                        Debug.Log("(Dong: " + rowStart + " Cot: " + colStart + ")" + " - " + "(Dong: " + rowEnd + " Cot: " + colEnd + ")");
                        return false;
                    }
                    else mapAll[rowEnd, colEnd] = 0;
                }
            }
        }
        return true;
    }
}
