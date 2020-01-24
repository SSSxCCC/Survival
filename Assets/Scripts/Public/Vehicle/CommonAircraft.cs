using UnityEngine;

public class CommonAircraft<T> where T : MonoBehaviour, IAircraft
{
    private T aircraft;

    private IVehicleWeapon[] weapons;
    private Rigidbody2D rb2D; // 刚体
    private float torque; // 转矩力
    private float leftAngle; // 当前角度到目标角度的剩余角度，仅针对手机操作
    private Vector2 force; // 移动推力

    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    public CommonAircraft(T aircraft)
    {
        this.aircraft = aircraft;
    }

    public void Start()
    {
        weapons = aircraft.GetComponents<IVehicleWeapon>();
        rb2D = aircraft.GetComponent<Rigidbody2D>();
    }

    // 保持每个玩家在座位上
    public void Update()
    {
        if (aircraft.driver != null) aircraft.driver.transform.position = aircraft.seats[GetSeat(aircraft.driver)].position;
        if (aircraft.passenger1 != null) aircraft.passenger1.transform.position = aircraft.seats[GetSeat(aircraft.passenger1)].position;
        if (aircraft.passenger2 != null) aircraft.passenger2.transform.position = aircraft.seats[GetSeat(aircraft.passenger2)].position;
    }

    // 移动和转向
    public void FixedUpdate()
    {
        if (aircraft.driver == null) return;

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
        GameObject explosion = Object.Instantiate(explosionPrefab, aircraft.transform.position, Quaternion.identity);
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
        int seat = 0; // 默认坐到司机座位
        if (aircraft.driver != null) // 如果已经有司机了
        { // 则坐到最近的座位
            float minDistance = Vector2.SqrMagnitude(player.transform.position - aircraft.seats[0].position);
            for (int i = 1; i < aircraft.seats.Length; i++)
            {
                float distance = Vector2.SqrMagnitude(player.transform.position - aircraft.seats[i].position);
                if (distance < minDistance)
                {
                    seat = i;
                    minDistance = distance;
                }
            }

            GameObject previousPlayer = GetPlayer(seat);
            if (previousPlayer != null) aircraft.Leave(previousPlayer);
        }
        
        PutPlayer(player, seat);
        player.GetComponent<IPlayerController>().vehicle = aircraft.gameObject;
        player.GetComponent<IState>().layer = LayerMask.NameToLayer("Air");
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
            player.GetComponent<IState>().layer = LayerMask.NameToLayer("Player");
        }
        return seat;
    }

    public void LeaveAll()
    {
        if (aircraft.driver != null) aircraft.Leave(aircraft.driver);
        if (aircraft.passenger1 != null) aircraft.Leave(aircraft.passenger1);
        if (aircraft.passenger2 != null) aircraft.Leave(aircraft.passenger2);
    }

    public void Control(GameObject player, Vector2 axis)
    {
        if (player == aircraft.driver) Drive(axis);
    }

    private void Drive(Vector2 axis)
    {
        if (SpecificPlatform.IsCurrent(PlatformType.PC)) // 对于PC平台
        {
            if (axis.x > 0) TurnWheel(-1);
            else if (axis.x < 0) TurnWheel(1);
            else TurnWheel(0);

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
                float carAngle = aircraft.transform.eulerAngles.z;
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
        torque = aircraft.rotateTorque * direction;
    }

    /// <summary>
    /// 踩油门或刹车
    /// </summary>
    /// <param name="k">大于0表示踩油门，小于0表示踩刹车，等于0表示空挡</param>
    private void AcceleratorOrBrake(float k)
    {
        float radianDirection = aircraft.transform.eulerAngles.z * Mathf.Deg2Rad;
        force = aircraft.powerForce * new Vector2(Mathf.Cos(radianDirection), Mathf.Sin(radianDirection)) * k;
    }

    public void Fire(GameObject player)
    {
        if (!((SpecificPlatform.IsCurrent(PlatformType.PC) && Input.GetButtonDown("Fire")) || (SpecificPlatform.IsCurrent(PlatformType.Phone) && VirtualInputManager.singleton.fireButton.GetButtonDown())))
            return;

        if (player == aircraft.passenger1)
        {
            weapons[0].Fire(player);
        }
        else if (player == aircraft.passenger2)
        {
            weapons[1].Fire(player);
        }
        else
        {
            if (aircraft.passenger1 == null)
            {
                if (!weapons[0].Fire(player))
                {
                    if (aircraft.passenger2 == null)
                    {
                        weapons[1].Fire(player);
                    }
                }
            }
            else if (aircraft.passenger2 == null)
            {
                weapons[1].Fire(player);
            }
        }
    }

    private GameObject GetPlayer(int seat)
    {
        switch (seat)
        {
            case 0:
                return aircraft.driver;
            case 1:
                return aircraft.passenger1;
            case 2:
                return aircraft.passenger2;
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
                aircraft.driver = player;
                break;
            case 1:
                aircraft.passenger1 = player;
                break;
            case 2:
                aircraft.passenger2 = player;
                break;
            default:
                Debug.LogError("Invalid seat value: " + seat);
                break;
        }
    }

    private int GetSeat(GameObject player)
    {
        if (player == aircraft.driver) return 0;
        else if (player == aircraft.passenger1) return 1;
        else if (player == aircraft.passenger2) return 2;
        else return -1;
    }
}
