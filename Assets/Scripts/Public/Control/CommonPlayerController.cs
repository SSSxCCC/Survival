using UnityEngine;

public class CommonPlayerController<T> where T : MonoBehaviour, IPlayerController
{
    private T playerController;

    private IWeaponManager weaponManager; // 武器管理者
    private IState state; // 状态
    private GameObject vehicleObject; // 目前认为的载具
    private IVehicle vehicleScript; // 目前认为的载具的相应脚本

    private RaycastHit2D[] hits; // 试探结果
    private int enemyLayerMask; // 辨认敌人
    private int vehicleLayerMask; // 辨认载具

    public CommonPlayerController(T playerController)
    {
        this.playerController = playerController;
    }

    public void Start()
    {
        weaponManager = playerController.GetComponent<IWeaponManager>();
        state = playerController.GetComponent<IState>();

        hits = new RaycastHit2D[30];
        enemyLayerMask = LayerMask.GetMask("EnemyGround", "EnemyFloat", "EnemyAir");
        vehicleLayerMask = LayerMask.GetMask("Ground", "Float", "Air");
    }

    public void Update()
    {
        // 保证载具引用正确
        if (vehicleObject != playerController.vehicle)
        { // 这里有个很隐蔽的问题：载具随时可能被摧毁而变为null，导致此if进不来，进而导致vehicleObject变成null时vehicleScript不是null。
            vehicleObject = playerController.vehicle;
            if (vehicleObject != null)
                vehicleScript = playerController.vehicle.GetComponent<IVehicle>();
            //else vehicleScript = null;
        }

        // 玩家单位死亡时
        if (state.health <= 0)
        {
            if (vehicleObject != null)
                vehicleScript.Control(playerController.gameObject, Vector2.zero);

            return;
        }
        
        if (SpecificPlatform.IsCurrent(PlatformType.PC)) // PC端由鼠标键盘手柄操作
        {
            ControlByKeyboardMouseJoystick();
        }
        else if (SpecificPlatform.IsCurrent(PlatformType.Phone)) // 手机端由触屏操作
        {
            ControlByTouch();
        }
    }

    // 键盘/鼠标/手柄操作
    private void ControlByKeyboardMouseJoystick()
    {
        Vector2 controlDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        Move(controlDirection); // 移动

        if (JoystickPresent()) LookAtTarget(controlDirection); // 自动瞄准
        else LookAtMouse(); // 面朝鼠标

        if (Input.GetButtonDown("Next Weapon")) NextWeapon(); // 换下一把武器

        if (Input.GetAxis("Fire") > 0) Fire(); // 开火

        if (Input.GetButtonDown("Enter or Leave")) playerController.EnterOrLeave(); // 进入或离开

        if (vehicleObject != null) vehicleScript.Control(playerController.gameObject, controlDirection); // 控制载具
    }

    /// <summary>
    /// 用一种可能有问题的方式判断系统是否插了手柄。
    /// </summary>
    /// <returns>如果系统插了至少一个手柄，则返回true，否则返回false</returns>
    private bool JoystickPresent()
    {
        string[] joystickNames = Input.GetJoystickNames();
        foreach (string joystickName in joystickNames)
        {
            if (joystickName.Length > 0) return true;
        }
        return false;
    }

    // 触屏操作
    private void ControlByTouch()
    {
        Vector2 joystickDisplacement = VirtualInputManager.singleton.joystick.GetAxis();

        Move(joystickDisplacement); // 移动

        LookAtTarget(joystickDisplacement); // 自动瞄准

        if (VirtualInputManager.singleton.nextWeaponButton.GetButtonDown()) NextWeapon(); // 换下一把武器

        if (VirtualInputManager.singleton.fireButton.GetButton()) Fire(); // 开火

        if (VirtualInputManager.singleton.enterOrLeaveButton.GetButtonDown()) playerController.EnterOrLeave(); // 进入或离开

        if (vehicleObject != null) vehicleScript.Control(playerController.gameObject, joystickDisplacement); // 控制载具
    }

    // 移动
    private void Move(Vector2 direction)
    {
        if (vehicleObject != null && !vehicleScript.Move(playerController.gameObject)) // 在不允许自由移动的载具内
        {
            playerController.Move(Vector2.zero); // 不移动
            return;
        }
        
        playerController.Move(direction);
    }

    // 自动瞄准（没有目标时面朝移动方向看）
    private void LookAtTarget(Vector2 direction)
    {
        if (vehicleObject != null && !vehicleScript.Rotate(playerController.gameObject)) // 在不允许自由旋转的载具内
        {
            playerController.MoveRotation(float.MinValue); // 不旋转
            return;
        }

        if (!AutoAim()) // 优先自动瞄准，如果没有自动瞄准的目标，则朝前看
        {
            if (vehicleObject == null) playerController.RotateTo(direction); // 没有载具则朝身体前方看
            else
            {   // 有载具则朝载具前方看
                float angle = vehicleObject.transform.eulerAngles.z;
                playerController.MoveRotation(angle);
            }
        }
    }

    // 面朝鼠标
    private void LookAtMouse()
    {
        if (vehicleObject != null && !vehicleScript.Rotate(playerController.gameObject)) // 在不允许自由旋转的载具内
        {
            playerController.MoveRotation(float.MinValue); // 不旋转
            return;
        }

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerController.LookAt(mouseWorldPosition);
    }

    // 切换到下一把武器
    private void NextWeapon()
    {
        if (vehicleObject != null && !vehicleScript.NextWeapon(playerController.gameObject)) return; // 在不允许自由切换武器的载具内

        weaponManager.NextWeapon();
    }

    // 开火
    private void Fire()
    {
        if (vehicleObject != null && !vehicleScript.Fire(playerController.gameObject)) return; // 在不允许自由开火的载具内

        weaponManager.Fire();
    }

    // 进入或离开
    public void EnterOrLeave()
    {
        if (playerController.vehicle != null) // 在载具里面
            playerController.vehicle.GetComponent<IVehicle>().Leave(playerController.gameObject); // 离开载具
        else // 不在载具内
        {
            // 选择距离最近的载具
            Vector2 origin = playerController.transform.position;
            int size = Physics2D.CircleCastNonAlloc(origin, 1, Vector2.zero, hits, float.MaxValue, vehicleLayerMask);
            Transform nearestVehicleTransform = null;
            float minDistance = float.MaxValue;
            for (int i = 0; i < size; i++)
            {
                if (!hits[i].transform.CompareTag("Vehicle") || hits[i].transform.GetComponent<IState>().health <= 0) continue;
                
                float distance = Vector2.SqrMagnitude((Vector2)hits[i].transform.position - origin);
                if (distance < minDistance)
                {
                    nearestVehicleTransform = hits[i].transform;
                    minDistance = distance;
                }
            }

            if (minDistance < float.MaxValue) // 发现最近载具
                nearestVehicleTransform.GetComponent<IVehicle>().Enter(playerController.gameObject); // 则进入此载具
        }
    }
    
    /// <summary>
    /// 自动瞄准，使玩家面朝目标敌方单位。
    /// </summary>
    /// <returns>如果附近找到了适合自动瞄准的单位，则面朝此单位，并返回true，否则返回false。</returns>
    private bool AutoAim()
    {
        // 选择距离最近的敌人
        Vector2 origin = playerController.transform.position;
        int size = Physics2D.CircleCastNonAlloc(origin, 10, Vector2.zero, hits, float.MaxValue, enemyLayerMask);
        Vector2 nearestEnemyPosition = Vector2.zero;
        float minDistance = float.MaxValue;
        for (int i = 0; i < size; i++)
        {
            if (hits[i].transform.GetComponent<IState>().health <= 0 || playerController.HasObstacleTo(hits[i].transform.position)) continue;

            Vector2 enemyPosition = hits[i].transform.position;
            float distance = Vector2.SqrMagnitude(enemyPosition - origin);
            if (distance < minDistance)
            {
                nearestEnemyPosition = enemyPosition;
                minDistance = distance;
            }
        }

        if (minDistance == float.MaxValue) return false; // 没有活着的敌人

        playerController.LookAt(nearestEnemyPosition);
        return true;
    }

    // 载具改变时调用，显示载具武器或自己武器
    public void OnChangeVehicle()
    {
        if (playerController.vehicle == null) // 载具没了
        {
            weaponManager.ShowWeaponState(); // 显示自己武器
        }
        else // 有载具了
        {
            IVehicle vehicle = playerController.vehicle.GetComponent<IVehicle>();
            VehicleWeaponState vehicleWeaponState = vehicle.GetWeaponState(playerController.gameObject);
            if (vehicleWeaponState != null) // 载具上有武器
            {
                WeaponDisplay.singleton.ShowWeapon(vehicleWeaponState.sprite, vehicleWeaponState.color, vehicleWeaponState.numAmmo); // 显示载具上的武器
            }
        }
    }
}
