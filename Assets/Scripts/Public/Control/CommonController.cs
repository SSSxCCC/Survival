using UnityEngine;

public class CommonController<T> where T : MonoBehaviour, IController
{
    private T controller;

    private Rigidbody2D rb2D; // 刚体
    private float angle; // 应该朝向的角度
    private Vector2 force; // 移动的推力
    private IState state; // 状态

    private RaycastHit2D[] obstacleHits; // 试探障碍物结果

    public CommonController(T controller)
    {
        this.controller = controller;
    }

    public void Start()
    {
        rb2D = controller.GetComponent<Rigidbody2D>();
        state = controller.GetComponent<IState>();

        obstacleHits = new RaycastHit2D[30];
    }

    public int GetObstacleLayerMask()
    {
        switch (LayerMask.LayerToName(controller.gameObject.layer))
        {
            case "EnemyGround":
                return LayerMask.GetMask("Water", "Ground", "Float");
            case "EnemyFloat":
                return LayerMask.GetMask("Ground", "Float");
            case "EnemyAir":
                return LayerMask.GetMask("Air");
            case "Player":
                return LayerMask.GetMask("Ground"); // 这里没有水是因为玩家子弹可以穿过水，而玩家不关心自己是否能通过
            default:
                return LayerMask.GetMask("Water", "Ground", "Float");
        }
    }

    public void FixedUpdate()
    {
        if (state.health <= 0 || state.stun > 0) return;

        if (angle > float.MinValue) rb2D.angularVelocity = GetAngularVelocity(); // angle值为float.MinValue表示不旋转
        rb2D.AddForce(force);
    }

    private float GetAngularVelocity()
    {
        float maxAngularVelocity = Mathf.DeltaAngle(controller.transform.eulerAngles.z, angle) / Time.fixedDeltaTime; // 此时允许的最大角速度

        float k;
        if (maxAngularVelocity > 0) k = 1;
        else if (maxAngularVelocity < 0) k = -1;
        else k = 0;

        return k * Mathf.Min(720f, Mathf.Abs(maxAngularVelocity));
    }

    /// <summary>
    /// 面朝世界中的一个点。
    /// </summary>
    /// <param name="point">面朝的世界中的点的坐标</param>
    public void LookAt(Vector2 point)
    {
        Vector2 startPoint;
        startPoint = controller.transform.position;

        MoveRotation(Mathf.Rad2Deg * Mathf.Atan2(point.y - startPoint.y, point.x - startPoint.x));
    }

    /// <summary>
    /// 面朝矢量方向。
    /// </summary>
    /// <param name="direction">面朝的矢量方向</param>
    public void RotateTo(Vector2 direction)
    {
        if (direction.Equals(Vector2.zero)) return;

        MoveRotation(Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
    }

    /// <summary>
    /// 转向角度
    /// </summary>
    /// <param name="angle">转向的角度</param>
    public void MoveRotation(float angle)
    {
        this.angle = angle;
    }

    /// <summary>
    /// 用力推着自己移动。
    /// </summary>
    /// <param name="forceDirection">推力方向，此矢量长度小于等于1</param>
    public void Move(Vector2 forceDirection)
    {
        force = forceDirection * controller.moveForce;
    }

    /// <summary>
    /// 试探自己与目标之间是否有障碍物
    /// </summary>
    /// <param name="point">目标坐标</param>
    /// <returns>如果自己与目标之间有障碍物，返回true，否则返回false</returns>
    public bool HasObstacleTo(Vector2 point)
    {
        int numHits = Physics2D.LinecastNonAlloc(controller.transform.position, point, obstacleHits, controller.obstacleLayerMask);
        for (int i = 0; i < numHits; i++) if (obstacleHits[i].collider.CompareTag("Obstacle")) return true;

        return false;
    }
}