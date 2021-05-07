using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyController : Controller {
    GameObject m_TargetPlayer; // 这个敌人盯上的玩家
    public GameObject targetPlayer { get { return m_TargetPlayer; } set { m_TargetPlayer = value; } }

    private Agent agent; // 标记自己的位置

    private float lastFindTargetTime; // 上次找目标时间
    private Waypoint targetWaypoint; // 目标所处的路径点

    private int stuckFrame; // 记录卡住的帧数，大于或等于3帧速度为0才判定为卡住
    private float stuckTime; // 记录卡住的时间
    private Vector2 alternativeTarget; // 可选的移动目标

    [Server]
    public override void OnStartServer() {
        agent = GetComponent<Agent>();
        stuckFrame = 0;
        stuckTime = float.MinValue;
    }

    // AI
    [ServerCallback]
    private void Update() {
        if (state.health <= 0) return;

        // 找一个玩家为进攻目标
        FindTarget();
        if (targetPlayer == null) { // 没有玩家了
            return;
        }

        if (Time.time - stuckTime < 1f) { // 如果刚刚被卡住了
            if (agent != null && agent.IsNavigating() && HasObstacleTo(agent.GetNow())) // 如果发现在沿着路径走且不能直达下一个路径点
                agent.destination = null; // 丢弃现有路径

            // 向可选的移动目标走
            GoToward(alternativeTarget);
        } else if (HasObstacleTo(targetPlayer.transform.position) && agent != null) { // 如果和目标玩家之间有障碍物且知道自己位置
            Waypoint playerWaypoint = targetPlayer.GetComponent<Agent>().waypoint; // 得到目标当前位置
            if (agent.IsNavigating() && targetWaypoint == playerWaypoint) // 如果自己正在路上，且目标位置没有改变
                GoToward(agent.GetNext()); // 向下一个路径点前进
            else { // 如果还没有查找路径或目标位置改变了，重新找一条路径
                targetWaypoint = playerWaypoint;
                agent.destination = targetWaypoint;
            }
        } else { // 如果和玩家之间没有障碍物
            GoToward(targetPlayer.transform.position); // 直接冲向玩家
            if (agent != null) agent.destination = null;
        }

        if (rb2D.velocity.magnitude <= 0.01f) GetStuck(); // 发现卡住了
        else stuckFrame = 0;
    }

    // 限制物理运动只由服务器控制
    [ServerCallback]
    protected override void FixedUpdate() {
        base.FixedUpdate();
    }

    // 找一个目标
    [Server]
    private void FindTarget() {
        if (targetPlayer == null || targetPlayer.GetComponent<State>().health <= 0 || Time.time - lastFindTargetTime > 1f) { // 没有当前目标或当前目标死亡或距上次找目标超过了1秒
            targetPlayer = FindNearestPlayer();
        }
    }

    // 找到一个最近玩家
    [Server]
    private GameObject FindNearestPlayer() {
        lastFindTargetTime = Time.time;

        float minSqrDistance = float.MaxValue;
        GameObject nearestPlayer = null;
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in allPlayers) {
            if (player.GetComponent<State>().health > 0) {
                float sqrDistance = Vector2.SqrMagnitude(transform.position - player.transform.position);
                if (sqrDistance < minSqrDistance) {
                    minSqrDistance = sqrDistance;
                    nearestPlayer = player;
                }
            }
        }
        return nearestPlayer;
    }

    // 向目标前进
    [Server]
    private void GoToward(Vector2 targetPosition) {
        LookAt(targetPosition);
        Move(new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y).normalized);
    }

    // 被卡住时，更新被卡住时间，产生随机可选目标
    [Server]
    private void GetStuck() {
        stuckFrame++;
        if (stuckFrame >= 3) {
            stuckTime = Time.time;
            alternativeTarget = new Vector2(Random.Range(transform.position.x - 100f, transform.position.x + 100f), Random.Range(transform.position.y - 100f, transform.position.y + 100f));
            stuckFrame = 0;
        }
    }
}
