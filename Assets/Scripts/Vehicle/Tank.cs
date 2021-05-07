using UnityEngine;
using UnityEngine.Networking;

public class Tank : Vehicle {
    [SerializeField] Transform m_Seat; // 座位的位置
    public Transform seat { get { return m_Seat; } set { m_Seat = value; } }

    [SerializeField] Transform m_Cannon; // 炮的位置
    public Transform cannon { get { return m_Cannon; } set { m_Cannon = value; } }

    [SerializeField] float m_RotateTorque = 1; // 旋转转矩
    public float rotateTorque { get { return m_RotateTorque; } set { m_RotateTorque = value; } }

    [SerializeField] float m_PowerForce = 1; // 动力
    public float powerForce { get { return m_PowerForce; } set { m_PowerForce = value; } }

    [SerializeField] Transform m_Muzzle; // 炮口
    public Transform muzzle { get { return m_Muzzle; } set { m_Muzzle = value; } }

    [SerializeField] GameObject m_AmmoPrefab; // 弹药
    public GameObject ammoPrefab { get { return m_AmmoPrefab; } set { m_AmmoPrefab = value; } }

    [SerializeField] int m_Attack; // 攻击力
    public int attack { get { return m_Attack; } set { m_Attack = value; } }

    [SerializeField] float m_FireInterval; // 开火间隔
    public float fireInterval { get { return m_FireInterval; } set { m_FireInterval = value; } }

    [SerializeField] float m_AmmoSpeed; // 弹药射速
    public float ammoSpeed { get { return m_AmmoSpeed; } set { m_AmmoSpeed = value; } }

    public float lastFireTime { get; set; } // 上次开火时间

    public GameObject explosionPrefab;

    private State state;
    private Rigidbody2D rb2D; // 刚体
    private float torque; // 转矩力
    private float leftAngle; // 当前角度到目标角度的剩余角度，仅针对手机操作
    private Vector2 force; // 移动推力
    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    private void Start() {
        state = GetComponent<State>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // 保持玩家在座位上
    private void Update() {
        if (driver != null) driver.transform.position = seat.position;
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
        float selfSpeed = rb2D.velocity.magnitude;
        if (selfSpeed < 1) return;

        var state = collision.gameObject.GetComponent<State>();
        if (state == null) return;

        float sumSpeed = collision.relativeVelocity.magnitude + selfSpeed;

        state.TakeAttack((int)(sumSpeed * sumSpeed / 2f)); // 造成伤害
        if (collision.GetContacts(contacts) > 0) state.DamagedEffect(contacts[0].point); // 产生受伤特效
        state.stun = Mathf.Max(state.stun, Mathf.Sqrt(sumSpeed)); // 使眩晕
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
        if (driver != null) Leave(driver);
        driver = player;
        player.GetComponent<PlayerController>().vehicle = gameObject;
        player.GetComponent<State>().defense += state.defense;
        GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 授权
    }

    // 玩家离开
    [Server]
    public override void Leave(GameObject player) {
        base.Leave(player);
        driver = null;
        player.GetComponent<PlayerController>().vehicle = null;
        player.GetComponent<State>().defense -= state.defense;
        GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 取消授权
    }

    // 所有玩家离开
    [Server]
    public override void LeaveAll() {
        if (driver != null)
            Leave(driver);
    }

    // 得到武器状态
    [Client]
    public override VehicleWeaponState GetWeaponState(GameObject player) {
        return new VehicleWeaponState(cannon.GetComponentInChildren<SpriteRenderer>().sprite, Color.white, int.MaxValue);
    }

    // 控制
    [Client]
    public override void Control(GameObject player, Vector2 axis) {
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

        cannon.rotation = Quaternion.Euler(0, 0, player.transform.eulerAngles.z);
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
        return false;
    }

    [Client]
    public override bool Fire(GameObject player) {
        CmdFire();
        return false;
    }
    [Command]
    private void CmdFire() {
        if (Time.time - lastFireTime < fireInterval) return;
        GameObject ammo = Object.Instantiate(ammoPrefab, muzzle.position, cannon.rotation);
        var ammoAttacker = ammo.GetComponent<AmmoAttacker>();
        ammoAttacker.attack = attack;
        ammoAttacker.owner = driver;
        if (ammoSpeed > 0) {
            float radianAngle = ammo.transform.eulerAngles.z * Mathf.Deg2Rad;
            ammo.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)) * ammoSpeed;
        }
        lastFireTime = Time.time;
        NetworkServer.Spawn(ammo);
    }
}
