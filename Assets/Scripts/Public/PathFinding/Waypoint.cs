using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [HideInInspector] public Vector2 center; // 路径点的中心

    private Dictionary<int, List<Waypoint>> obstacleLayerMask_adjacentPoints_dict = new Dictionary<int, List<Waypoint>>(); // 障碍物掩码-相邻路径点列表
    private new Collider2D collider; // 自己的碰撞器
    private RaycastHit2D[] obstacleHits = new RaycastHit2D[30]; // 试探障碍物结果

    private void Awake()
    {
        center = GetComponent<Collider2D>().bounds.center;// 得到自己的中心
        collider = GetComponent<Collider2D>(); // 预先取出自己的碰撞器
    }

    // 捕捉进入本路径点区域的Agent，在其Agent对象里面标记其位置（此函数客户端也会调用，意义在于自己成为新的主机时，每个单位所处在的路径点id不需要同步）
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Agent agent = collider.GetComponent<Agent>();
        if (agent != null)
        {
            Controller controller = agent.GetComponent<Controller>();
            if (controller == null || GetAdjacentPoints(controller.obstacleLayerMask).Count > 0) // 防止电脑寻路时卡死
                agent.waypoint = this;
        }
    }

    public List<Waypoint> GetAdjacentPoints(int obstacleLayerMask)
    {
        if (!obstacleLayerMask_adjacentPoints_dict.ContainsKey(obstacleLayerMask))
        {
            Waypoint[] waypoints = WaypointManager.singleton.waypoints;
            foreach (Waypoint waypoint in waypoints)
                waypoint.obstacleLayerMask_adjacentPoints_dict[obstacleLayerMask] = new List<Waypoint>();

            // 遍历得到每个路径点的邻接表
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                for (int j = i + 1; j < waypoints.Length; j++)
                {
                    if (waypoints[i].IsAdjacent(waypoints[j], obstacleLayerMask))
                    {
                        waypoints[i].obstacleLayerMask_adjacentPoints_dict[obstacleLayerMask].Add(waypoints[j]);
                        waypoints[j].obstacleLayerMask_adjacentPoints_dict[obstacleLayerMask].Add(waypoints[i]);
                    }
                }
            }
        }

        return obstacleLayerMask_adjacentPoints_dict[obstacleLayerMask];
    }

    private bool IsAdjacent(Waypoint that, int obstacleLayerMask)
    {
        if (this.collider.bounds.Intersects(that.collider.bounds))
        {
            int numHits = Physics2D.LinecastNonAlloc(this.center, that.center, obstacleHits, obstacleLayerMask);
            for (int i = 0; i < numHits; i++) if (obstacleHits[i].collider.CompareTag("Obstacle")) return false;
            return true;
        }
        return false;
    }
}