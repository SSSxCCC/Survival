using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [HideInInspector] public Waypoint waypoint; // 正处于的路径点，即目前最后撞到的Waypoint对象

    private Controller controller;

    Waypoint m_Destination; // 目标路径点
    public Waypoint destination
    {
        get { return m_Destination; }
        set
        {
            m_Destination = value;
            UpdatePathList();
        }
    }
    private List<Waypoint> pathList = null; // 前往目标的路径列表
    private int nextWaypointIndex; // 下一个要前往的路径点在路径列表对应的下标

    private void Awake()
    {
        controller = GetComponent<Controller>();
    }

    public Vector2 GetNext()
    {
        if (pathList == null) // 如果自己不在路上
        {
            if (m_Destination == null) // 没有终点
                return transform.position;
            else if (Vector2.Distance(m_Destination.center, transform.position) < 1f) // 已到达终点
            {
                m_Destination = null;
                return transform.position;
            }
            else // 有终点而且还没到，计算一下路径
                if (!UpdatePathList())
                return transform.position;
        }

        if (Vector2.Distance(pathList[nextWaypointIndex].center, transform.position) < 1f) // 如果已经到了下一个路径点接近中心的位置
            if (nextWaypointIndex > 0) // 如果到达的不是终点
                UpdateNextWaypointIndex(); // 则更新下一个路径点
            else // 如果到达的是终点
            {
                m_Destination = null;
                pathList = null;
                return transform.position;
            }

        return pathList[nextWaypointIndex].center; // 返回下一个路径点中心
    }

    public Vector2 GetNow()
    {
        return pathList[nextWaypointIndex].center;
    }

    public bool IsNavigating()
    {
        return pathList != null;
    }

    // 更新路径列表
    private bool UpdatePathList()
    {
        pathList = AStar.GetPath(waypoint, m_Destination, controller.obstacleLayerMask);
        if (pathList == null) // 找不到路径（此处很有可能是因为自己刚刚刷出来还没有撞到任何路径点导致自己的waypoint为null）
            return false;

        nextWaypointIndex = pathList.Count - 1;
        UpdateNextWaypointIndex();
        return true;
    }

    // 更新下一个路径点
    private void UpdateNextWaypointIndex()
    {
        while (nextWaypointIndex > 0)
        {
            nextWaypointIndex--;
            if (controller.HasObstacleTo(pathList[nextWaypointIndex].center))
            {
                nextWaypointIndex++; // 这里有可能一直更新不了
                break;
            }
        }
    }
}