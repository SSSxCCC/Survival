using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AStar
{
    private class PathKey
    {
        private Waypoint start;
        private Waypoint end;
        private int obstacleLayerMask;

        public PathKey(Waypoint start, Waypoint end, int obstacleLayerMask)
        {
            this.start = start;
            this.end = end;
            this.obstacleLayerMask = obstacleLayerMask;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            PathKey that = (PathKey)obj;
            if (this.start == that.start && this.end == that.end && this.obstacleLayerMask == that.obstacleLayerMask)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return start.GetHashCode() + end.GetHashCode() + obstacleLayerMask;
        }
    }

    // 保存已计算过的路径，以免重复计算
    private static Dictionary<PathKey, List<Waypoint>> paths = new Dictionary<PathKey, List<Waypoint>>();

    /// <summary>
    /// 得到从起点到终点路径。
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="obstacleLayerMask">障碍物掩码</param>
    /// <returns>成功找到路径则返回路径列表（列表最后一个元素是起点，第一个元素是终点），否则返回null。</returns>
    public static List<Waypoint> GetPath(Waypoint start, Waypoint end, int obstacleLayerMask)
    {
        if (start == null || end == null)
            return null;

        PathKey pathKey = new PathKey(start, end, obstacleLayerMask);
        if (!paths.ContainsKey(pathKey)) // 还没有算过
        {
            paths[pathKey] = GetPathByAStar(start, end, obstacleLayerMask); // 算一下，并保存结果
        }
        return paths[pathKey];
    }

    // 采用A*算法，得到从起点（start路径点）到终点（end路径点）相对最短路径
    private static List<Waypoint> GetPathByAStar(Waypoint start, Waypoint end, int obstacleLayerMask)
    {
        BinaryHeap open = new BinaryHeap(WaypointManager.singleton.waypoints.Length); // open表保存所有已生成而未考察的节点
        HashSet<Waypoint> closed = new HashSet<Waypoint>(); // closed表中记录已访问过的节点id

        // 首先把起始节点放入open表
        open.Add(new Vertex(start, 0, Vector2.Distance(start.center, end.center), null));

        // A*算法主循环
        Vertex vertex = null;
        Vertex nearestVertex = null;
        while (!open.IsEmpty())
        {
            vertex = open.Remove(); // 从open表取出f值最小节点

            // 找到终点了
            if (vertex.waypoint == end)
                break;

            // 记录最近的点
            if (nearestVertex == null || vertex.h < nearestVertex.h)
                nearestVertex = vertex;

            // 遍历与此节点所有相邻路径点
            foreach (Waypoint waypoint in vertex.waypoint.GetAdjacentPoints(obstacleLayerMask))
            {
                // 如果该路径点已在closed表中，不用管
                if (closed.Contains(waypoint))
                {
                    continue;
                }

                // 计算出经过自己到该路径点的情况下，该路径点的g值。
                float newG = vertex.g + Vector2.Distance(vertex.waypoint.center, waypoint.center);

                int index = open.FindVertex(waypoint); // 在open表中查找是否已存在此路径点
                if (index > 0) // 如果该路径点已在open表中
                {
                    // 如果此路径点经过此节点可以有更小的g值
                    if (newG < open.vertexes[index].g)
                    {
                        open.vertexes[index].g = newG; // 则更新其g值
                        open.vertexes[index].parent = vertex; // 此节点成为此路径点的爸爸
                        open.Float(index); // 进行上浮操作保证二叉堆的有序性
                    }
                }
                else // 如果该路径点既不在closed表中，也不在open表中
                {
                    Vertex newVertex = new Vertex(waypoint, newG, Vector2.Distance(waypoint.center, end.center), vertex);
                    open.Add(newVertex);
                }
            }

            // 将此节点插入closed表
            closed.Add(vertex.waypoint);
        }

        // 起点到不了终点
        if (vertex.waypoint != end)
        {
            if (nearestVertex == null)
                return null;
            else
                vertex = nearestVertex;
        }

        // 将路径存在一个列表里面并返回
        List<Waypoint> pathList = new List<Waypoint>();
        while (vertex != null)
        {
            pathList.Add(vertex.waypoint);
            vertex = vertex.parent;
        }
        return pathList;
    }

    public static void Dispose()
    {
        paths = new Dictionary<PathKey, List<Waypoint>>();
    }

    // 对象深拷贝
    /*private static T DeepClone<T>(T obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }*/
}



// 此类代表图上一个节点
public class Vertex
{
    public Waypoint waypoint; // 此节点代表的路径点
    public float g; // 此点到起点的距离
    public float h; // 此点到目标节点的估计距离
    public Vertex parent; // 此节点的父节点

    public Vertex(Waypoint waypoint, float g, float h, Vertex parent)
    {
        this.waypoint = waypoint;
        this.g = g;
        this.h = h;
        this.parent = parent;
    }

    // 起点经过此点到目标节点的估计距离，哪个节点更优取决于谁的估价函数f值更小
    public float f()
    {
        return g + h;
    }
}