using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject linePikachu;
    [SerializeField] private GameObject prefap_pikachu;
    [SerializeField] public static GameObject[,] map_pikachu;
    [SerializeField] private int Width;
    [SerializeField] private int Height;
    public static int ROW, COL;
    private int[,] MAP;
    private bool[][] SHIFT;
    private bool[][] SHIFT_ROOT_POS;
    private Vec2 POS1;
    private Vec2 POS2;
    private Vector2[][] POS;
    private int MIN_X;
    private int MIN_Y;
    private int CELL_WIDH = 28;
    private int CELL_HEIGHT = 32;
    private Color opacity = new Color(255f, 255f, 255f, .5f);
    private Color defaultColor = new Color(255f, 255f, 255f, 1.0f);
    public static int score = 0;
    public static int level = 1;
    private int map = 0;
    private void Start()
    {
        Pikachu(Width + 2, Height + 2);
        RandomMap();
        map = ((ROW - 2) * (COL - 2)) / 2;
        AudioManager.instance.PlaySFX("change");
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float x = (Input.mousePosition.x - Screen.width / 2) / (Screen.width * (Screen.width * 1.0f / Screen.height));
            float y = (Input.mousePosition.y - Screen.height / 2) / (Screen.height);

            if (Screen.width == 1920)
                x *= 1010;
            else
                x *= 820;

            y *= 320;
            int mouse_col = (int)(x - MIN_X) / (CELL_WIDH);
            int mouse_row = (int)(y - MIN_Y) / (CELL_HEIGHT);

            if (mouse_col < 0 || mouse_col > COL - 1 || mouse_row < 0 || mouse_row > ROW - 1)
            {
                mouse_row = 0;
                mouse_col = 0;
            }

            if (POS1 != null && POS1.C == mouse_col && POS1.R == mouse_row && MAP[mouse_row, mouse_col] == 0)
            {
                DeSelect();
            }
            else if (POS1 == null && MAP[mouse_row, mouse_col] == 0)
            {
                Select(new Vec2(mouse_row, mouse_col));
            }
            else if (MAP[mouse_row, mouse_col] == 0)
            {
                CheckPair(new Vec2(mouse_row, mouse_col));
            }
        }

    }

    private void Select(Vec2 pos)
    {
        POS1 = new Vec2(pos.R, pos.C);
        map_pikachu[POS1.R, POS1.C].GetComponent<SpriteRenderer>().color = opacity;
        Debug.Log("selected " + POS1.Print());
    }
    private void DeSelect()
    {
        map_pikachu[POS1.R, POS1.C].GetComponent<SpriteRenderer>().color = defaultColor;
        POS1 = null;
        POS2 = null;
        Debug.Log("DeSelect");
        AudioManager.instance.PlaySFX("e-oh");
    }
    private void CheckPair(Vec2 pos)
    {
        POS2 = new Vec2(pos.R, pos.C);
        Debug.Log("CheckPair " + POS1.Print() + " and " + POS2.Print());
        if (map_pikachu[POS1.R, POS1.C].GetComponent<SpriteRenderer>().sprite.name == map_pikachu[POS2.R, POS2.C].GetComponent<SpriteRenderer>().sprite.name)
        {
            MAP[POS1.R, POS1.C] = -1;
            MAP[POS2.R, POS2.C] = -1;
            SHIFT_ROOT_POS[POS2.R][POS2.R] = true;

            if (MAP[POS2.R + 1, POS2.C] == 0 && MAP[POS2.R - 1, POS2.C] == 0 && MAP[POS2.R, POS2.C + 1] == 0 && MAP[POS2.R, POS2.C - 1] == 0)
            {
                Default();
            }
            else if (!SHIFT[POS1.R][POS1.C] && !SHIFT[POS2.R][POS2.C] && FindPath(POS1, POS2) != null)
            {
                Line(FindPath(POS1, POS2).Count);

                if (map_pikachu[POS1.R, POS1.C].GetComponent<SpriteRenderer>().sprite.name == map_pikachu[POS2.R, POS2.C].GetComponent<SpriteRenderer>().sprite.name && MAP[POS2.R, POS2.C] == -1)
                {
                    map--;
                    (map_pikachu[POS1.R, POS1.C]).SetActive(false);
                    (map_pikachu[POS2.R, POS2.C]).SetActive(false);
                    MAP[POS1.R, POS1.C] = -1;
                    MAP[POS2.R, POS2.C] = -1;
                    SHIFT_ROOT_POS[POS2.R][POS2.R] = false;
                    POS1 = null;
                    POS2 = null;
                    score += 10;
                    AudioManager.instance.PlaySFX("point");
                    if (map == 0)
                    {
                        StartCoroutine(NextLevel());
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

        foreach (Vec2 line in FindPath(POS1, POS2))
        {
            linePikachu.GetComponent<LineRenderer>().SetPosition(counts++, POS[line.R][line.C]);
        }
        linePikachu.SetActive(true);
        if (linePikachu.activeInHierarchy)
        {
            StartCoroutine(LineOff());
        }
    }
    private int LimitLine(List<Vec2> Find, int count)
    {
        linePikachu.GetComponent<LineRenderer>().positionCount = count;
        int limit = 0;
        float tempX = POS1.R;
        float tempY = POS1.C;
        float temp = -1;
        bool x = true;
        bool y = true;
        foreach (Vec2 line in Find)
        {
            if (tempX == line.R && tempY != line.C) // Change col
            {
                if (temp == line.R && line.R != line.C && x == true)
                {
                    limit++;
                    y = true;
                    x = false;
                }
                temp = line.C;
            }
            else if (tempY == line.C && tempX != line.R) // Change row
            {
                if (temp == line.C && line.R != line.C && y == true)
                {
                    limit++;
                    x = true;
                    y = false;
                }
                temp = line.R;
            }

            if (tempX == line.R && tempY == line.C)
            {
                limit++;
            }
            else tempX = line.R; tempY = line.C;
        }

        Debug.Log("Kenkai" + limit);
        return limit;
    }

    IEnumerator LineOff()
    {
        yield return new WaitForSeconds(0.5f);
        linePikachu.SetActive(false);
    }
    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1f);
        level++;
        COL = 0;
        ROW = 0;
        Pikachu(Width + 2, Height + 2);
        RandomMap();
        map = ((ROW - 2) * (COL - 2)) / 2;
        CooldownTime.levelUp = 1;
        AudioManager.instance.PlaySFX("change");
    }
    private void Default()
    {
        MAP[POS1.R, POS1.C] = 0;
        MAP[POS2.R, POS2.C] = 0;
        SHIFT_ROOT_POS[POS2.R][POS2.R] = false;
        map_pikachu[POS1.R, POS1.C].GetComponent<SpriteRenderer>().color = defaultColor;
        map_pikachu[POS2.R, POS2.C].GetComponent<SpriteRenderer>().color = defaultColor;
        POS1 = null;
        POS2 = null;
        AudioManager.instance.PlaySFX("e-oh");
    }

    private GameObject AddPikachu(int type, Vector2 pos, int width, int height)
    {
        GameObject g = Instantiate(prefap_pikachu) as GameObject;
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
        for (int i = 1; i < ROW - 1; i++)
            for (int j = 1; j < COL - 1; j++)
            {
                MAP[i, j] = 0;
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
        for (int i = 1; i < ROW - 1; i++)
        {
            for (int j = 1; j < COL - 1; j++)
            {
                map_pikachu[i, j] = AddPikachu(pool[--dem], POS[i][j], CELL_WIDH, CELL_HEIGHT);
            }
        }
    }

    private void Pikachu(int row, int col)
    {
        CELL_HEIGHT = (int)(70f / (ROW - 2));
        CELL_WIDH = (int)(CELL_HEIGHT * 0.7f);

        ROW = row;
        COL = col;
        map_pikachu = new GameObject[ROW, COL];
        MAP = new int[ROW, COL];
        SHIFT = new bool[ROW][];
        SHIFT_ROOT_POS = new bool[ROW][];
        POS = new Vector2[ROW][];

        MIN_X = -(COL) * CELL_WIDH / 2;
        MIN_Y = -(ROW) * CELL_HEIGHT / 2;

        for (int i = 0; i < ROW; i++)
        {
            SHIFT[i] = new bool[COL];
            SHIFT_ROOT_POS[i] = new bool[COL];
            POS[i] = new Vector2[COL];
            for (int j = 0; j < COL; j++)
            {
                SHIFT[i][j] = false;
                MAP[i, j] = -1;
                SHIFT_ROOT_POS[i][j] = false;


                POS[i][j] = new Vector3(0, 0, 0);
                POS[i][j].x = MIN_X + j * CELL_WIDH + CELL_WIDH / 2;
                POS[i][j].y = MIN_Y + i * CELL_HEIGHT + CELL_HEIGHT / 2;
            }
        }
    }

    private List<Vec2> FindPath(Vec2 start, Vec2 end) // AStar
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
            fastDistance = Vec2.FastDistance(open, new Vec2(open.R, COL - 1));
        }
        else // Right
        {
            fastDistance = Vec2.FastDistance(open, new Vec2(open.R, 0));
        }
        for (int i = 0; i <= fastDistance; i++)
        {
            if (MAP[open.R, open.C - (i * horizontal)] == 0)
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
                if (MAP[open.R - (j * vertical), open.C - (i * horizontal)] == 0)
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
                    if (MAP[checkHorizontal.R, checkHorizontal.C - (i * horizontal)] == 0)
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
            if (MAP[open.R, open.C - (i * horizontal)] == 0) // Right Left check
            {
                if (MAP[open.R - 1 * vertical, open.C] == -1) // Check open up or down
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
                    if (MAP[checkVertical.R - (i * vertical), checkVertical.C] == 0)
                    {
                        if (open.R - (1 * tempVertical) >= 0 && open.R - (1 * tempVertical) < MAP.GetLength(0))
                        {
                            if (MAP[open.R - (1 * tempVertical), open.C] == 0)
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


        if (node.R > 0 && MAP[node.R - 1, node.C] == -1) // Up
        {
            neighbor.Add(new Vec2(node.R - 1, node.C));
        }
        if (node.R < MAP.GetLength(0) - 1 && MAP[node.R + 1, node.C] == -1) // Down
        {
            neighbor.Add(new Vec2(node.R + 1, node.C));
        }
        if (node.C > 0 && MAP[node.R, node.C - 1] == -1) // Right
        {
            neighbor.Add(new Vec2(node.R, node.C - 1));
        }
        if (node.C < MAP.GetLength(1) - 1 && MAP[node.R, node.C + 1] == -1) // Left
        {
            neighbor.Add(new Vec2(node.R, node.C + 1));
        }
        return neighbor;
    }
}
