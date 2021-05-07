using UnityEngine;
using UnityEngine.Networking;

public abstract class Controller : NetworkBehaviour {
    [SerializeField] float m_MoveForce = 1f; // 移动的推力
    public float moveForce { get { return m_MoveForce; } set { m_MoveForce = value; } }

    public int obstacleLayerMask { get {
        switch (LayerMask.LayerToName(gameObject.layer)) {
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
    } } // 障碍物掩码

    private float angle; // 应该朝向的角度
    private Vector2 force; // 移动的推力

    protected Rigidbody2D rb2D; // 刚体
    protected State state; // 状态
    protected RaycastHit2D[] hits; // 试探障碍物结果

    // 初始化
    protected virtual void Start() {
        rb2D = GetComponent<Rigidbody2D>();
        state = GetComponent<State>();
        hits = new RaycastHit2D[30];
    }

    // 物理移动和旋转
    protected virtual void FixedUpdate() {
        if (state.health <= 0 || state.stun > 0) return;
        if (angle > float.MinValue) rb2D.angularVelocity = GetAngularVelocity(); // angle值为float.MinValue表示不旋转
        rb2D.AddForce(force);
    }

    private float GetAngularVelocity() {
        float maxAngularVelocity = Mathf.DeltaAngle(transform.eulerAngles.z, angle) / Time.fixedDeltaTime; // 此时允许的最大角速度

        float k;
        if (maxAngularVelocity > 0) k = 1;
        else if (maxAngularVelocity < 0) k = -1;
        else k = 0;

        return k * Mathf.Min(720f, Mathf.Abs(maxAngularVelocity));
    }

    // 面朝世界坐标point的位置
    public void LookAt(Vector2 point) {
        Vector2 startPoint;
        startPoint = transform.position;

        MoveRotation(Mathf.Rad2Deg * Mathf.Atan2(point.y - startPoint.y, point.x - startPoint.x));
    }

    // 面朝矢量direction的方向
    public void RotateTo(Vector2 direction) {
        if (direction.Equals(Vector2.zero)) return;

        MoveRotation(Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
    }

    // 转向角度angle
    public void MoveRotation(float angle) {
        this.angle = angle;
    }

    // 推动前进，forceDirection是推力方向，此矢量长度小于等于1
    public void Move(Vector2 forceDirection) {
        force = forceDirection * moveForce;
    }

    // 自己与目标点point之间是否有障碍
    public bool HasObstacleTo(Vector2 point) {
        int numHits = Physics2D.LinecastNonAlloc(transform.position, point, hits, obstacleLayerMask);
        for (int i = 0; i < numHits; i++) if (hits[i].collider.CompareTag("Obstacle")) return true;

        return false;
    }
}
