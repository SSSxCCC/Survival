using UnityEngine;

public class CommonTank<T> where T : MonoBehaviour, ITank
{
    private T tank;

    private IState state;
    private Rigidbody2D rb2D; // 刚体
    private float torque; // 转矩力
    private float leftAngle; // 当前角度到目标角度的剩余角度，仅针对手机操作
    private Vector2 force; // 移动推力

    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    public CommonTank(T tank)
    {
        this.tank = tank;
    }

    public void Start()
    {
        state = tank.GetComponent<IState>();
        rb2D = tank.GetComponent<Rigidbody2D>();
    }

    // 保持玩家在座位上
    public void Update()
    {
        if (tank.driver != null) tank.driver.transform.position = tank.seat.position;
    }

    // 移动和转向
    public void FixedUpdate()
    {
        if (tank.driver == null) return;

        if (SpecificPlatform.IsCurrent(PlatformType.PC)) // 对于PC平台
            rb2D.AddTorque(torque);
        else if (SpecificPlatform.IsCurrent(PlatformType.Phone)) // 对于手机平台
        {
            if (leftAngle != 0)
            {
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
    public void OnCollisionEnter2D(Collision2D collision)
    {
        float selfSpeed = rb2D.velocity.magnitude;
        if (selfSpeed < 1) return;

        IState state = collision.gameObject.GetComponent<IState>();
        if (state == null) return;

        float sumSpeed = collision.relativeVelocity.magnitude + selfSpeed;

        state.TakeAttack((int)(sumSpeed * sumSpeed / 2f)); // 造成伤害
        if (collision.GetContacts(contacts) > 0) state.DamagedEffect(contacts[0].point); // 产生受伤特效
        state.stun = Mathf.Max(state.stun, Mathf.Sqrt(sumSpeed)); // 使眩晕
    }

    // 摧毁爆炸
    public GameObject OnDestroy(GameObject explosionPrefab)
    {
        GameObject explosion = Object.Instantiate(explosionPrefab, tank.transform.position, Quaternion.identity);
        IExplosion exp = explosion.GetComponent<IExplosion>();
        exp.attack = 70;
        //exp.owner = tank.gameObject;
        return explosion;
    }

    /// <summary>
    /// 玩家进入
    /// </summary>
    /// <param name="player">玩家</param>
    public void Enter(GameObject player)
    {
        if (tank.driver != null) tank.Leave(tank.driver);
        tank.driver = player;
        player.GetComponent<IPlayerController>().vehicle = tank.gameObject;
        player.GetComponent<IState>().defense += state.defense;
    }

    /// <summary>
    /// 玩家离开
    /// </summary>
    /// <param name="player">玩家</param>
    public void Leave(GameObject player)
    {
        tank.driver = null;
        player.GetComponent<IPlayerController>().vehicle = null;
        player.GetComponent<IState>().defense -= state.defense;
    }

    public void LeaveAll()
    {
        if (tank.driver != null)
            tank.Leave(tank.driver);
    }

    public void Control(GameObject player, Vector2 axis)
    {
        if (SpecificPlatform.IsCurrent(PlatformType.PC)) // 对于PC平台
        {
            if (rb2D.velocity.sqrMagnitude <= 0.001f)
                TurnWheel(0);
            else
            {
                if (axis.x > 0) TurnWheel(-1);
                else if (axis.x < 0) TurnWheel(1);
                else TurnWheel(0);
            }

            if (axis.y > 0) AcceleratorOrBrake(1);
            else if (axis.y < 0) AcceleratorOrBrake(-0.5f);
            else AcceleratorOrBrake(0);
        }
        else if (SpecificPlatform.IsCurrent(PlatformType.Phone)) // 对于手机平台
        {
            if (axis.Equals(Vector2.zero))
            {
                TurnWheel(0);
                AcceleratorOrBrake(0);
                leftAngle = 0;
            }
            else
            {
                float carAngle = tank.transform.eulerAngles.z;
                float axisAngle = Mathf.Rad2Deg * Mathf.Atan2(axis.y, axis.x);
                float deltaAngle = Mathf.DeltaAngle(carAngle, axisAngle);
                if (deltaAngle > 0 && deltaAngle <= 90)
                {
                    TurnWheel(1);
                    AcceleratorOrBrake(1);
                    leftAngle = deltaAngle;
                }
                else if (deltaAngle > 90 && deltaAngle < 180)
                {
                    TurnWheel(-1);
                    AcceleratorOrBrake(-0.5f);
                    leftAngle = Mathf.DeltaAngle(carAngle, axisAngle + 180);
                }
                else if (deltaAngle < 0 && deltaAngle >= -90)
                {
                    TurnWheel(-1);
                    AcceleratorOrBrake(1);
                    leftAngle = deltaAngle;
                }
                else if (deltaAngle < -90 && deltaAngle > -180)
                {
                    TurnWheel(1);
                    AcceleratorOrBrake(-0.5f);
                    leftAngle = Mathf.DeltaAngle(carAngle, axisAngle + 180);
                }
                else if (deltaAngle == 0)
                {
                    TurnWheel(0);
                    AcceleratorOrBrake(1);
                    leftAngle = 0;
                }
                else // deltaAngle == +-180
                {
                    TurnWheel(0);
                    AcceleratorOrBrake(-0.5f);
                    leftAngle = 0;
                }
            }
        }

        tank.cannon.rotation = Quaternion.Euler(0, 0, player.transform.eulerAngles.z);
    }

    /// <summary>
    /// 转动方向盘
    /// </summary>
    /// <param name="direction">旋转方向，大于0逆时针方向，小于0顺时针方向，等于0不转向</param>
    private void TurnWheel(float direction)
    {
        torque = tank.rotateTorque * direction;
    }

    /// <summary>
    /// 踩油门或刹车
    /// </summary>
    /// <param name="k">大于0表示踩油门，小于0表示踩刹车，等于0表示空挡</param>
    private void AcceleratorOrBrake(float k)
    {
        float radianDirection = tank.transform.eulerAngles.z * Mathf.Deg2Rad;
        force = tank.powerForce * new Vector2(Mathf.Cos(radianDirection), Mathf.Sin(radianDirection)) * k;
    }

    public GameObject Fire()
    {
        if (Time.time - tank.lastFireTime < tank.fireInterval) return null;
        
        GameObject ammo = Object.Instantiate(tank.ammoPrefab, tank.muzzle.position, tank.cannon.rotation);
        IAmmoAttacker ammoAttacker = ammo.GetComponent<IAmmoAttacker>();
        ammoAttacker.attack = tank.attack;
        ammoAttacker.owner = tank.driver;
        if (tank.ammoSpeed > 0)
        {
            float radianAngle = ammo.transform.eulerAngles.z * Mathf.Deg2Rad;
            ammo.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)) * tank.ammoSpeed;
        }
        tank.lastFireTime = Time.time;

        return ammo;
    }
}
