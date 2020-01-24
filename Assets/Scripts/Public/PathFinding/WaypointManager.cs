using UnityEngine;
using UnityEngine.Tilemaps;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager singleton; // 单例

    public bool awakeBake = true; // 是否在Awake中调用Bake方法
    public Tilemap[] closedTilemaps;
    public Tilemap waterTilemap;
    public GameObject[] waypointPrefabs;

    [HideInInspector] public Waypoint[] waypoints; // 所有路径点数组

    private void Awake()
    {
        singleton = this; // 自己成为单例

        if (awakeBake)
            Bake();
        else
            waypoints = GetComponentsInChildren<Waypoint>();

        //print(waypoints.Length);
    }

    private void OnDisable()
    {
        AStar.Dispose();
    }

    private enum Point { Open, Closed, Passed } // 分别表示此点可通过，无法通行，可通行但已经被路径点区域包括

    // 产生路径点
    public void Bake()
    {
        Clear();

        // 得到边界值
        int xMin = -MapBorder.singleton.mapHalfWidth;
        //int xMax = -xMin - 1;
        int yMin = -MapBorder.singleton.mapHalfHeight;
        //int yMax = -yMin - 1;

        // 用一个二维Point数组表示地图
        Point[,] map = new Point[-xMin * 2, -yMin * 2];

        // 只有除去水和障碍物的区域可通行
        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
                map[x, y] = HasAnyTile(new Vector3Int(xMin + x, yMin + y, 0)) ? Point.Closed : Point.Open;
        Bake(map, xMin, yMin);

        // 只有除去障碍物的水可通行
        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
                map[x, y] = HasWaterWithoutClosedTile(new Vector3Int(xMin + x, yMin + y, 0)) ? Point.Open : Point.Closed;
        Bake(map, xMin, yMin);

        // 在自己的所有子物体中找到所有路径点对象
        waypoints = GetComponentsInChildren<Waypoint>();
    }

    private bool HasAnyTile(Vector3Int position)
    {
        if (waterTilemap.HasTile(position))
            return true;
        return HasAnyClosedTile(position);
    }

    private bool HasAnyClosedTile(Vector3Int position)
    {
        foreach (Tilemap tilemap in closedTilemaps)
            if (tilemap.HasTile(position))
                return true;
        return false;
    }

    private bool HasWaterWithoutClosedTile(Vector3Int position)
    {
        return waterTilemap.HasTile(position) && !HasAnyClosedTile(position);
    }

    private void Bake(Point[,] map, int xMin, int yMin)
    {
        int waypointMaxLength = (int)Mathf.Sqrt(waypointPrefabs.Length);
        for (int x = 0; x < map.GetLength(0); x++) // 从左下角开始遍历
            for (int y = 0; y < map.GetLength(1); y++)
                if (map[x, y] == Point.Open)
                {
                    // 找到一个以(x, y)为左下角可行且较好的路径点长宽值
                    int widthBest = 0, heightBest = 0, cornerBest = 0;
                    int width, height, corner; // corner是被认为左上，右上，右下三个角较好的权值和。当满足(角上不是Closed且至少一邻边是Closed)或(角上是Closed且两邻边是否是Closed的情况一样)时认为此角较好。
                    for (width = waypointMaxLength; width > 0; width--)
                        for (height = waypointMaxLength; height > 0; height--)
                            if (Suitable(map, x, y, width, height))
                            {
                                corner = 0;
                                if (x + width < map.GetLength(0) && y + height < map.GetLength(1) && ( // 右上角
                                     (map[x + width, y + height] != Point.Closed && (map[x, y + height] == Point.Closed || map[x + width, y] == Point.Closed)) ||
                                     (map[x + width, y + height] == Point.Closed && (map[x, y + height] == Point.Closed) == (map[x + width, y] == Point.Closed))
                                   )) corner += 2;
                                if (x > 0 && y + height < map.GetLength(1) && ( // 左上角
                                     (map[x - 1, y + height] != Point.Closed && (map[x, y + height] == Point.Closed || map[x - 1, y] == Point.Closed)) ||
                                     (map[x - 1, y + height] == Point.Closed && (map[x, y + height] == Point.Closed) == (map[x - 1, y] == Point.Closed))
                                   )) corner += 1;
                                if (x + width < map.GetLength(0) && y > 0 && ( // 右下角
                                     (map[x + width, y - 1] != Point.Closed && (map[x, y - 1] == Point.Closed || map[x + width, y] == Point.Closed)) ||
                                     (map[x + width, y - 1] == Point.Closed && (map[x, y - 1] == Point.Closed) == (map[x + width, y] == Point.Closed))
                                   )) corner += 1;

                                if (corner > cornerBest || ((corner == cornerBest) && (width * height > widthBest * heightBest)))
                                {
                                    widthBest = width;
                                    heightBest = height;
                                    cornerBest = corner;
                                }
                            }

                    // 生成路径点
                    GameObject waypointPrefab = waypointPrefabs[(widthBest - 1) * waypointMaxLength + heightBest - 1];
                    Instantiate(waypointPrefab, waterTilemap.GetCellCenterLocal(new Vector3Int(xMin + x, yMin + y, 0)), Quaternion.identity, transform);

                    // 将路径点里面的每个点设为Passed
                    for (int i = x; i < x + widthBest; i++)
                        for (int j = y; j < y + heightBest; j++)
                            map[i, j] = Point.Passed;
                }
    }

    // 检查当前以(x, y)作为左下角，width,height分别作为长和宽的路径点是否合适
    private bool Suitable(Point[,] map, int x, int y, int width, int height)
    {
        // 不能越界
        if (x + width > map.GetLength(0) || y + height > map.GetLength(1))
            return false;

        // 范围内每个点必须都是Open的
        for (int i = x; i < x + width; i++)
            for (int j = y; j < y + height; j++)
                if (map[i, j] != Point.Open)
                    return false;

        // 四个边的外边要么都是Closed，要么都不是Closed
        if (height > 1)
        {
            if (x > 0) // 左
            {
                bool isClosed = map[x - 1, y] == Point.Closed;
                for (int j = y + 1; j < y + height; j++)
                    if ((map[x - 1, j] == Point.Closed) != isClosed)
                        return false;
            }
            if (x + width < map.GetLength(0)) // 右
            {
                bool isClosed = map[x + width, y] == Point.Closed;
                for (int j = y + 1; j < y + height; j++)
                    if ((map[x + width, j] == Point.Closed) != isClosed)
                        return false;
            }
        }
        if (width > 1)
        {
            if (y > 0) // 下
            {
                bool isClosed = map[x, y - 1] == Point.Closed;
                for (int i = x + 1; i < x + width; i++)
                    if ((map[i, y - 1] == Point.Closed) != isClosed)
                        return false;
            }
            if (y + height < map.GetLength(1)) // 上
            {
                bool isClosed = map[x, y + height] == Point.Closed;
                for (int i = x + 1; i < x + width; i++)
                    if ((map[i, y + height] == Point.Closed) != isClosed)
                        return false;
            }
        }

        return true;
    }

    // 清除所有路径点
    public void Clear()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        waypoints = null;
        AStar.Dispose();
    }
}