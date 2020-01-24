using UnityEngine;

public class CommonFloatboard<T> where T : MonoBehaviour, IFloatboard
{
    private T floatboard;

    private Rigidbody2D rb2D; // 刚体
    private float torque; // 转矩力
    private float leftAngle; // 当前角度到目标角度的剩余角度，仅针对手机操作
    private Vector2 force; // 移动推力

    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    public CommonFloatboard(T floatboard)
    {
        this.floatboard = floatboard;
    }

    public void Start()
    {
        rb2D = floatboard.GetComponent<Rigidbody2D>();
    }

    // 保持每个乘客在座位上
    public void Update()
    {
        if (floatboard.driver != null) floatboard.driver.transform.position = floatboard.seats[GetSeat(floatboard.driver)].position;
        if (floatboard.passenger != null) floatboard.passenger.transform.position = floatboard.seats[GetSeat(floatboard.passenger)].position;
    }

    // 移动和转向
    public void FixedUpdate()
    {
        if (floatboard.driver == null) return;

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
        float relativeSpeed = collision.relativeVelocity.magnitude;
        if (relativeSpeed < 5) return;

        IState state = collision.gameObject.GetComponent<IState>();
        if (state == null) return;

        state.TakeAttack((int)(relativeSpeed * relativeSpeed / 4f)); // 造成伤害
        if (collision.GetContacts(contacts) > 0) state.DamagedEffect(contacts[0].point); // 产生受伤特效
        state.stun = Mathf.Max(state.stun, Mathf.Sqrt(relativeSpeed)); // 使眩晕
    }

    // 摧毁爆炸
    public GameObject OnDestroy(GameObject explosionPrefab)
    {
        GameObject explosion = Object.Instantiate(explosionPrefab, floatboard.transform.position, Quaternion.identity);
        IExplosion exp = explosion.GetComponent<IExplosion>();
        exp.attack = 70;
        //exp.owner = floatboard.gameObject;
        return explosion;
    }

    /// <summary>
    /// 玩家进入
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>玩家是否是司机</returns>
    public bool Enter(GameObject player)
    {
        float distance0 = Vector2.SqrMagnitude(player.transform.position - floatboard.seats[0].position);
        float distance1 = Vector2.SqrMagnitude(player.transform.position - floatboard.seats[1].position);
        int seat = distance0 <= distance1 ? 0 : 1;

        GameObject previousPlayer = GetPlayer(seat);
        if (previousPlayer != null) floatboard.Leave(previousPlayer);
        
        player.GetComponent<IPlayerController>().vehicle = floatboard.gameObject;
        player.GetComponent<IState>().layer = LayerMask.NameToLayer("Float");

        if (floatboard.driver == null) // 如果没有司机
        {
            floatboard.driver = player; // 玩家成为司机
            floatboard.driverSeatIndex = seat;
            return true;
        }
        else // 如果有司机了
        {
            floatboard.passenger = player; // 玩家只是乘客
            return false;
        }
    }

    /// <summary>
    /// 玩家离开。如果离开的玩家是司机且另一个座位上还有玩家，则另一个座位上的玩家成为司机。
    /// </summary>
    /// <param name="player">离开的玩家</param>
    /// <param name="newDriver">变成司机的玩家，没有新的司机则为null</param>
    /// <returns>离开的玩家是否是司机。</returns>
    public bool Leave(GameObject player)
    {
        player.GetComponent<IPlayerController>().vehicle = null;
        player.GetComponent<IState>().layer = LayerMask.NameToLayer("Player");

        int seat = GetSeat(player);
        if (seat == floatboard.driverSeatIndex) // 离开的是司机
        {
            if (floatboard.passenger != null) // 另一个座位上还有玩家
            {
                floatboard.driver = floatboard.passenger; // 另一个座位的玩家成为司机
                floatboard.passenger = null;
                floatboard.driverSeatIndex = seat == 0 ? 1 : 0;
            }
            else // 另一个座位没有玩家了
            {
                floatboard.driver = null;
                floatboard.driverSeatIndex = -1;
            }
            return true;
        }
        else // 离开的不是司机
        {
            floatboard.passenger = null;
            return false;
        }
    }

    public void LeaveAll()
    {
        if (floatboard.passenger != null) floatboard.Leave(floatboard.passenger);
        if (floatboard.driver != null) floatboard.Leave(floatboard.driver);
    }

    public void Control(GameObject player, Vector2 axis)
    {
        if (player == floatboard.driver) Drive(axis);
    }

    private void Drive(Vector2 axis)
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
                float carAngle = floatboard.transform.eulerAngles.z;
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
    }

    /// <summary>
    /// 转动方向盘
    /// </summary>
    /// <param name="direction">旋转方向，大于0逆时针方向，小于0顺时针方向，等于0不转向</param>
    private void TurnWheel(float direction)
    {
        torque = floatboard.rotateTorque * direction;
    }

    /// <summary>
    /// 踩油门或刹车
    /// </summary>
    /// <param name="k">大于0表示踩油门，小于0表示踩刹车，等于0表示空挡</param>
    private void AcceleratorOrBrake(float k)
    {
        float radianDirection = floatboard.transform.eulerAngles.z * Mathf.Deg2Rad;
        force = floatboard.powerForce * new Vector2(Mathf.Cos(radianDirection), Mathf.Sin(radianDirection)) * k;
    }

    private GameObject GetPlayer(int seat)
    {
        if (seat == floatboard.driverSeatIndex)
            return floatboard.driver;
        else
            return floatboard.passenger;
    }

    private int GetSeat(GameObject player)
    {
        if (player == floatboard.driver)
            return floatboard.driverSeatIndex;
        else
            return floatboard.driverSeatIndex == 0 ? 1 : 0;
    }
}
