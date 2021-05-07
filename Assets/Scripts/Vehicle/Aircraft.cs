using UnityEngine;
using UnityEngine.Networking;

public class Aircraft : Vehicle {
    [SerializeField] Transform[] m_Seats; // 每个座位的位置（座位0是司机的）
    public Transform[] seats { get { return m_Seats; } set { m_Seats = value; } }

    [SyncVar] GameObject m_Passenger1;
    [SyncVar] GameObject m_Passenger2;
    public GameObject passenger1 { get { return m_Passenger1; } set { m_Passenger1 = value; } }
    public GameObject passenger2 { get { return m_Passenger2; } set { m_Passenger2 = value; } }

    [SerializeField] float m_RotateTorque = 1; // 旋转转矩
    public float rotateTorque { get { return m_RotateTorque; } set { m_RotateTorque = value; } }

    [SerializeField] float m_PowerForce = 1; // 前进动力
    public float powerForce { get { return m_PowerForce; } set { m_PowerForce = value; } }

    public GameObject explosionPrefab;

    private VehicleWeapon[] weapons;
    private Rigidbody2D rb2D; // 刚体
    private float torque; // 转矩力
    private float leftAngle; // 当前角度到目标角度的剩余角度，仅针对手机操作
    private Vector2 force; // 移动推力
    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    private void Start() {
        weapons = GetComponents<VehicleWeapon>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // 保持每个乘客在座位上
    private void Update()
    {
        if (driver != null) driver.transform.position = seats[GetSeat(driver)].position;
        if (passenger1 != null) passenger1.transform.position = seats[GetSeat(passenger1)].position;
        if (passenger2 != null) passenger2.transform.position = seats[GetSeat(passenger2)].position;
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
        if (EnterAndGetSeat(player) == 0)
            GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 授权
    }

    /// <summary>
    /// 玩家进入载具并得到座位号
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>玩家坐到的座位号</returns>
    [Server]
    private int EnterAndGetSeat(GameObject player) {
        int seat = 0; // 默认坐到司机座位
        if (driver != null) { // 如果已经有司机了，则坐到最近的座位
            float minDistance = Vector2.SqrMagnitude(player.transform.position - seats[0].position);
            for (int i = 1; i < seats.Length; i++) {
                float distance = Vector2.SqrMagnitude(player.transform.position - seats[i].position);
                if (distance < minDistance) {
                    seat = i;
                    minDistance = distance;
                }
            }

            GameObject previousPlayer = GetPlayer(seat);
            if (previousPlayer != null) Leave(previousPlayer);
        }
        
        PutPlayer(player, seat);
        player.GetComponent<PlayerController>().vehicle = gameObject;
        player.GetComponent<State>().layer = LayerMask.NameToLayer("Air");
        return seat;
    }

    // 玩家离开
    [Server]
    public override void Leave(GameObject player)
    {
        base.Leave(player);
        if (LeaveAndGetSeat(player) == 0)
            GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 取消授权
    }

    /// <summary>
    /// 玩家离开载具并得到座位号
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>玩家之前的座位号</returns>
    [Server]
    private int LeaveAndGetSeat(GameObject player) {
        int seat = GetSeat(player);
        if (seat >= 0) {
            PutPlayer(null, seat);
            player.GetComponent<PlayerController>().vehicle = null;
            player.GetComponent<State>().layer = LayerMask.NameToLayer("Player");
        }
        return seat;
    }

    // 所有玩家离开
    [Server]
    public override void LeaveAll() {
        if (driver != null) Leave(driver);
        if (passenger1 != null) Leave(passenger1);
        if (passenger2 != null) Leave(passenger2);
    }

    // 得到武器状态
    [Client]
    public override VehicleWeaponState GetWeaponState(GameObject player) {
        return new VehicleWeaponState(GetComponentInChildren<SpriteRenderer>().sprite, Color.white, int.MaxValue);
    }

    // 控制
    [Client]
    public override void Control(GameObject player, Vector2 axis) {
        if (player == driver) Drive(axis);
    }

    [Client]
    private void Drive(Vector2 axis) {
        if (SpecificPlatform.IsCurrent(PlatformType.PC)) { // 对于PC平台
            if (axis.x > 0) TurnWheel(-1);
            else if (axis.x < 0) TurnWheel(1);
            else TurnWheel(0);

            if (axis.y > 0) AcceleratorOrBrake(1);
            else if (axis.y < 0) AcceleratorOrBrake(-0.5f);
            else AcceleratorOrBrake(0);
        } else if (SpecificPlatform.IsCurrent(PlatformType.Phone)) { // 对于手机平台
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
        player.transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z); // 面朝载具正前方
        return false;
    }

    [Client]
    public override bool NextWeapon(GameObject player) {
        return false;
    }

    [Client]
    public override bool Fire(GameObject player) {
        if (!((SpecificPlatform.IsCurrent(PlatformType.PC) && Input.GetButtonDown("Fire")) || (SpecificPlatform.IsCurrent(PlatformType.Phone) && VirtualInputManager.singleton.fireButton.GetButtonDown())))
            return false;

        if (player == passenger1) {
            weapons[0].Fire(player);
        } else if (player == passenger2) {
            weapons[1].Fire(player);
        } else {
            if (passenger1 == null) {
                if (!weapons[0].Fire(player)) {
                    if (passenger2 == null) {
                        weapons[1].Fire(player);
                    }
                }
            } else if (passenger2 == null) {
                weapons[1].Fire(player);
            }
        }
        return false;
    }

    private GameObject GetPlayer(int seat) {
        switch (seat) {
            case 0:
                return driver;
            case 1:
                return passenger1;
            case 2:
                return passenger2;
            default:
                Debug.LogError("Invalid seat value: " + seat);
                return null;
        }
    }

    private void PutPlayer(GameObject player, int seat) {
        switch (seat) {
            case 0:
                driver = player;
                break;
            case 1:
                passenger1 = player;
                break;
            case 2:
                passenger2 = player;
                break;
            default:
                Debug.LogError("Invalid seat value: " + seat);
                break;
        }
    }

    private int GetSeat(GameObject player)
    {
        if (player == driver) return 0;
        else if (player == passenger1) return 1;
        else if (player == passenger2) return 2;
        else return -1;
    }
}
