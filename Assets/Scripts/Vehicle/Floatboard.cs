using UnityEngine;
using UnityEngine.Networking;

public class Floatboard : Vehicle {
    [SerializeField] Transform[] m_Seats; // 每个座位的位置（先来的玩家是司机）
    public Transform[] seats { get { return m_Seats; } set { m_Seats = value; } }

    [SyncVar] int m_DriverSeatIndex = -1; // 司机座位下标
    public int driverSeatIndex { get { return m_DriverSeatIndex; } set { m_DriverSeatIndex = value; } }

    [SyncVar] GameObject m_Passenger; // 乘客
    public GameObject passenger { get { return m_Passenger; } set { m_Passenger = value; } }

    [SerializeField] float m_RotateTorque = 1; // 旋转转矩
    public float rotateTorque { get { return m_RotateTorque; } set { m_RotateTorque = value; } }

    [SerializeField] float m_PowerForce = 1; // 汽车动力
    public float powerForce { get { return m_PowerForce; } set { m_PowerForce = value; } }

    public GameObject explosionPrefab;

    private Rigidbody2D rb2D; // 刚体
    private float torque; // 转矩力
    private float leftAngle; // 当前角度到目标角度的剩余角度，仅针对手机操作
    private Vector2 force; // 移动推力
    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    private void Start() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // 保持每个乘客在座位上
    private void Update() {
        if (driver != null) driver.transform.position = seats[GetSeat(driver)].position;
        if (passenger != null) passenger.transform.position = seats[GetSeat(passenger)].position;
    }

    // 移动和转向
    private void FixedUpdate() {
        if (driver == null) return;

        if (SpecificPlatform.IsCurrent(PlatformType.PC)) // 对于PC平台
            rb2D.AddTorque(torque);
        else if (SpecificPlatform.IsCurrent(PlatformType.Phone)) { // 对于手机平台
            if (leftAngle != 0) {
                float nextDeltaAngle = rb2D.angularVelocity * Time.fixedDeltaTime;
                if (Mathf.Abs(nextDeltaAngle) < Mathf.Abs(leftAngle))
                    rb2D.AddTorque(torque);
                else
                    rb2D.angularVelocity = leftAngle / Time.fixedDeltaTime;
            }
        }

        rb2D.AddForce(force);
    }

    // 撞击单位
    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision) {
        float relativeSpeed = collision.relativeVelocity.magnitude;
        if (relativeSpeed < 5) return;

        var state = collision.gameObject.GetComponent<State>();
        if (state == null) return;

        state.TakeAttack((int)(relativeSpeed * relativeSpeed / 4f)); // 造成伤害
        if (collision.GetContacts(contacts) > 0) state.DamagedEffect(contacts[0].point); // 产生受伤特效
        state.stun = Mathf.Max(state.stun, Mathf.Sqrt(relativeSpeed)); // 使眩晕
    }

    // 被摧毁时爆炸
    [ServerCallback]
    private void OnDestroy() {
        var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<Explosion>().attack = 70;
        NetworkServer.Spawn(explosion);
    }

    // 玩家进入
    [Server]
    public override void Enter(GameObject player) {
        base.Enter(player);
        if (EnterAndIsDriver(player))
            GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 授权
    }

    /// <summary>
    /// 玩家进入并得到是否是司机
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>玩家是否是司机</returns>
    [Server]
    private bool EnterAndIsDriver(GameObject player) {
        float distance0 = Vector2.SqrMagnitude(player.transform.position - seats[0].position);
        float distance1 = Vector2.SqrMagnitude(player.transform.position - seats[1].position);
        int seat = distance0 <= distance1 ? 0 : 1;

        GameObject previousPlayer = GetPlayer(seat);
        if (previousPlayer != null) Leave(previousPlayer);
        
        player.GetComponent<PlayerController>().vehicle = gameObject;
        player.GetComponent<State>().layer = LayerMask.NameToLayer("Float");

        if (driver == null) { // 如果没有司机
            driver = player; // 玩家成为司机
            driverSeatIndex = seat;
            return true;
        } else { // 如果有司机了
            passenger = player; // 玩家只是乘客
            return false;
        }
    }

    // 玩家离开
    [Server]
    public override void Leave(GameObject player) {
        base.Leave(player);
        if (LeaveAndIsDriver(player)) { // 离开的是司机
            GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 取消授权
            if (driver != null) // 有新的玩家成为司机了
                GetComponent<NetworkIdentity>().AssignClientAuthority(driver.GetComponent<NetworkIdentity>().connectionToClient); // 给新的司机授权
        }
    }

    /// <summary>
    /// 玩家离开。如果离开的玩家是司机且另一个座位上还有玩家，则另一个座位上的玩家成为司机。
    /// </summary>
    /// <param name="player">离开的玩家</param>
    /// <returns>离开的玩家是否是司机。</returns>
    [Server]
    private bool LeaveAndIsDriver(GameObject player) {
        player.GetComponent<PlayerController>().vehicle = null;
        player.GetComponent<State>().layer = LayerMask.NameToLayer("Player");

        int seat = GetSeat(player);
        if (seat == driverSeatIndex) { // 离开的是司机
            if (passenger != null) { // 另一个座位上还有玩家
                driver = passenger; // 另一个座位的玩家成为司机
                passenger = null;
                driverSeatIndex = seat == 0 ? 1 : 0;
            } else { // 另一个座位没有玩家了
                driver = null;
                driverSeatIndex = -1;
            }
            return true;
        } else { // 离开的不是司机
            passenger = null;
            return false;
        }
    }

    // 所有玩家离开
    [Server]
    public override void LeaveAll() {
        if (passenger != null) Leave(passenger);
        if (driver != null) Leave(driver);
    }

    // 控制
    [Client]
    public override void Control(GameObject player, Vector2 axis) {
        if (player == driver) Drive(axis);
    }

    [Client]
    private void Drive(Vector2 axis) {
        if (SpecificPlatform.IsCurrent(PlatformType.PC)) { // 对于PC平台
            if (rb2D.velocity.sqrMagnitude <= 0.001f)
                TurnWheel(0);
            else {
                if (axis.x > 0) TurnWheel(-1);
                else if (axis.x < 0) TurnWheel(1);
                else TurnWheel(0);
            }

            if (axis.y > 0) AcceleratorOrBrake(1);
            else if (axis.y < 0) AcceleratorOrBrake(-0.5f);
            else AcceleratorOrBrake(0);
        }
        else if (SpecificPlatform.IsCurrent(PlatformType.Phone)) { // 对于手机平台
            if (axis.Equals(Vector2.zero)) {
                TurnWheel(0);
                AcceleratorOrBrake(0);
                leftAngle = 0;
            } else {
                float carAngle = transform.eulerAngles.z;
                float axisAngle = Mathf.Rad2Deg * Mathf.Atan2(axis.y, axis.x);
                float deltaAngle = Mathf.DeltaAngle(carAngle, axisAngle);
                if (deltaAngle > 0 && deltaAngle <= 90) {
                    TurnWheel(1);
                    AcceleratorOrBrake(1);
                    leftAngle = deltaAngle;
                } else if (deltaAngle > 90 && deltaAngle < 180) {
                    TurnWheel(-1);
                    AcceleratorOrBrake(-0.5f);
                    leftAngle = Mathf.DeltaAngle(carAngle, axisAngle + 180);
                } else if (deltaAngle < 0 && deltaAngle >= -90) {
                    TurnWheel(-1);
                    AcceleratorOrBrake(1);
                    leftAngle = deltaAngle;
                } else if (deltaAngle < -90 && deltaAngle > -180) {
                    TurnWheel(1);
                    AcceleratorOrBrake(-0.5f);
                    leftAngle = Mathf.DeltaAngle(carAngle, axisAngle + 180);
                } else if (deltaAngle == 0) {
                    TurnWheel(0);
                    AcceleratorOrBrake(1);
                    leftAngle = 0;
                } else { // deltaAngle == +-180
                    TurnWheel(0);
                    AcceleratorOrBrake(-0.5f);
                    leftAngle = 0;
                }
            }
        }
    }

    /// <summary>
    /// 转动方向盘
    /// </summary>
    /// <param name="direction">旋转方向，大于0逆时针方向，小于0顺时针方向，等于0不转向</param>
    [Client]
    private void TurnWheel(float direction) {
        torque = rotateTorque * direction;
    }

    /// <summary>
    /// 踩油门或刹车
    /// </summary>
    /// <param name="k">大于0表示踩油门，小于0表示踩刹车，等于0表示空挡</param>
    [Client]
    private void AcceleratorOrBrake(float k) {
        float radianDirection = transform.eulerAngles.z * Mathf.Deg2Rad;
        force = powerForce * new Vector2(Mathf.Cos(radianDirection), Mathf.Sin(radianDirection)) * k;
    }

    [Client]
    public override bool Move(GameObject player) {
        return false;
    }

    [Client]
    public override bool Rotate(GameObject player) {
        return true;
    }

    [Client]
    public override bool NextWeapon(GameObject player) {
        return true;
    }

    [Client]
    public override bool Fire(GameObject player) {
        return true;
    }

    private GameObject GetPlayer(int seat) {
        if (seat == driverSeatIndex)
            return driver;
        else
            return passenger;
    }

    private int GetSeat(GameObject player) {
        if (player == driver)
            return driverSeatIndex;
        else
            return driverSeatIndex == 0 ? 1 : 0;
    }
}
