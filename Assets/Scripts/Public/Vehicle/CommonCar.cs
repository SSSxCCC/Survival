using UnityEngine;

public class CommonCar<T> where T : MonoBehaviour, ICar
{
    private T car;

    private Rigidbody2D rb2D; // 刚体
    private float torque; // 转矩力
    private float leftAngle; // 当前角度到目标角度的剩余角度，仅针对手机操作
    private Vector2 force; // 移动推力

    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    public CommonCar(T car)
    {
        this.car = car;
    }

    public void Start()
    {
        rb2D = car.GetComponent<Rigidbody2D>();
    }

    // 保持每个玩家在座位上
    public void Update()
    {
        if (car.driver != null) car.driver.transform.position = car.seats[GetSeat(car.driver)].position;
        if (car.passenger1 != null) car.passenger1.transform.position = car.seats[GetSeat(car.passenger1)].position;
        if (car.passenger2 != null) car.passenger2.transform.position = car.seats[GetSeat(car.passenger2)].position;
        if (car.passenger3 != null) car.passenger3.transform.position = car.seats[GetSeat(car.passenger3)].position;
    }

    // 移动和转向
    public void FixedUpdate()
    {
        if (car.driver == null) return;

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
        GameObject explosion = Object.Instantiate(explosionPrefab, car.transform.position, Quaternion.identity);
        IExplosion exp = explosion.GetComponent<IExplosion>();
        exp.attack = 70;
        //exp.owner = car.gameObject;
        return explosion;
    }

    /// <summary>
    /// 玩家进入
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>玩家坐到的座位号</returns>
    public int Enter(GameObject player)
    {
        float minDistance = Vector2.SqrMagnitude(player.transform.position - car.seats[0].position);
        int seat = 0;
        for (int i = 1; i < car.seats.Length; i++)
        {
            float distance = Vector2.SqrMagnitude(player.transform.position - car.seats[i].position);
            if (distance < minDistance)
            {
                seat = i;
                minDistance = distance;
            }
        }

        GameObject previousPlayer = GetPlayer(seat);
        if (previousPlayer != null) car.Leave(previousPlayer);
        PutPlayer(player, seat);
        player.GetComponent<IPlayerController>().vehicle = car.gameObject;
        return seat;
    }

    /// <summary>
    /// 玩家离开
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>玩家之前的座位号</returns>
    public int Leave(GameObject player)
    {
        int seat = GetSeat(player);
        if (seat >= 0)
        {
            PutPlayer(null, seat);
            player.GetComponent<IPlayerController>().vehicle = null;
        }
        return seat;
    }

    public void LeaveAll()
    {
        if (car.driver != null) car.Leave(car.driver);
        if (car.passenger1 != null) car.Leave(car.passenger1);
        if (car.passenger2 != null) car.Leave(car.passenger2);
        if (car.passenger3 != null) car.Leave(car.passenger3);
    }

    public void Control(GameObject player, Vector2 axis)
    {
        if (player == car.driver) Drive(axis);
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
                float carAngle = car.transform.eulerAngles.z;
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
        torque = car.rotateTorque * direction;
    }

    /// <summary>
    /// 踩油门或刹车
    /// </summary>
    /// <param name="k">大于0表示踩油门，小于0表示踩刹车，等于0表示空挡</param>
    private void AcceleratorOrBrake(float k)
    {
        float radianDirection = car.transform.eulerAngles.z * Mathf.Deg2Rad;
        force = car.powerForce * new Vector2(Mathf.Cos(radianDirection), Mathf.Sin(radianDirection)) * k;
    }

    private GameObject GetPlayer(int seat)
    {
        switch (seat)
        {
            case 0:
                return car.driver;
            case 1:
                return car.passenger1;
            case 2:
                return car.passenger2;
            case 3:
                return car.passenger3;
            default:
                Debug.LogError("Invalid seat value: " + seat);
                return null;
        }
    }

    private void PutPlayer(GameObject player, int seat)
    {
        switch (seat)
        {
            case 0:
                car.driver = player;
                break;
            case 1:
                car.passenger1 = player;
                break;
            case 2:
                car.passenger2 = player;
                break;
            case 3:
                car.passenger3 = player;
                break;
            default:
                Debug.LogError("Invalid seat value: " + seat);
                break;
        }
    }

    private int GetSeat(GameObject player)
    {
        if (player == car.driver) return 0;
        else if (player == car.passenger1) return 1;
        else if (player == car.passenger2) return 2;
        else if (player == car.passenger3) return 3;
        else return -1;
    }
}
