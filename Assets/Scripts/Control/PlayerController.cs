using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : Controller {
    [SyncVar(hook = "OnChangeVehicle")] GameObject m_Vehicle; // 载具
    public GameObject vehicle { get { return m_Vehicle; } set { m_Vehicle = value; } }

    private WeaponManager weaponManager; // 武器管理者
    private GameObject vehicleObject; // 目前认为的载具
    private Vehicle vehicleScript; // 目前认为的载具的相应脚本
    private int enemyLayerMask; // 辨认敌人
    private int vehicleLayerMask; // 辨认载具

    // 要镜头跟着玩家控制的单位
    [Client]
    public override void OnStartLocalPlayer() {
        Camera.main.GetComponent<CameraController>().localPlayer = gameObject;
    }

    // 变量初始化
    protected override void Start() {
        base.Start();

        weaponManager = GetComponent<WeaponManager>();

        enemyLayerMask = LayerMask.GetMask("EnemyGround", "EnemyFloat", "EnemyAir");
        vehicleLayerMask = LayerMask.GetMask("Ground", "Float", "Air");
    }

    // 玩家操作处理
    private void Update() {
        if (!isLocalPlayer) return;

        // 保证载具引用正确
        if (vehicleObject != vehicle) { // 这里有个很隐蔽的问题：载具随时可能被摧毁而变为null，导致此if进不来，进而导致vehicleObject变成null时vehicleScript不是null。
            vehicleObject = vehicle;
            if (vehicleObject != null)
                vehicleScript = vehicle.GetComponent<Vehicle>();
            //else vehicleScript = null;
        }

        // 玩家单位死亡时
        if (state.health <= 0) {
            if (vehicleObject != null)
                vehicleScript.Control(gameObject, Vector2.zero);
            return;
        }
        
        if (SpecificPlatform.IsCurrent(PlatformType.PC)) { // PC端由鼠标键盘手柄操作
            ControlByKeyboardMouseJoystick();
        }
        else if (SpecificPlatform.IsCurrent(PlatformType.Phone)) { // 手机端由触屏操作
            ControlByTouch();
        }
    }

        // 键盘/鼠标/手柄操作
    private void ControlByKeyboardMouseJoystick() {
        Vector2 controlDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        MoveAt(controlDirection); // 移动

        if (JoystickPresent()) LookAtTarget(controlDirection); // 自动瞄准
        else LookAtMouse(); // 面朝鼠标

        if (Input.GetButtonDown("Next Weapon")) NextWeapon(); // 换下一把武器

        if (Input.GetAxis("Fire") > 0) Fire(); // 开火

        if (Input.GetButtonDown("Enter or Leave")) EnterOrLeave(); // 进入或离开

        if (vehicleObject != null) vehicleScript.Control(gameObject, controlDirection); // 控制载具
    }

    /// <summary>
    /// 用一种可能有问题的方式判断系统是否插了手柄。
    /// </summary>
    /// <returns>如果系统插了至少一个手柄，则返回true，否则返回false</returns>
    private bool JoystickPresent() {
        string[] joystickNames = Input.GetJoystickNames();
        foreach (string joystickName in joystickNames) {
            if (joystickName.Length > 0) return true;
        }
        return false;
    }

    // 触屏操作
    private void ControlByTouch() {
        Vector2 joystickDisplacement = VirtualInputManager.singleton.joystick.GetAxis();

        MoveAt(joystickDisplacement); // 移动

        LookAtTarget(joystickDisplacement); // 自动瞄准

        if (VirtualInputManager.singleton.nextWeaponButton.GetButtonDown()) NextWeapon(); // 换下一把武器

        if (VirtualInputManager.singleton.fireButton.GetButton()) Fire(); // 开火

        if (VirtualInputManager.singleton.enterOrLeaveButton.GetButtonDown()) EnterOrLeave(); // 进入或离开

        if (vehicleObject != null) vehicleScript.Control(gameObject, joystickDisplacement); // 控制载具
    }

    // 移动
    private void MoveAt(Vector2 direction) {
        if (vehicleObject != null && !vehicleScript.Move(gameObject)) { // 在不允许自由移动的载具内
            Move(Vector2.zero); // 不移动
            return;
        }

        Move(direction);
    }

    // 自动瞄准（没有目标时面朝移动方向看）
    private void LookAtTarget(Vector2 direction) {
        if (vehicleObject != null && !vehicleScript.Rotate(gameObject)) { // 在不允许自由旋转的载具内
            MoveRotation(float.MinValue); // 不旋转
            return;
        }

        if (!AutoAim()) { // 优先自动瞄准，如果没有自动瞄准的目标，则朝前看
            if (vehicleObject == null) RotateTo(direction); // 没有载具则朝身体前方看
            else { // 有载具则朝载具前方看
                float angle = vehicleObject.transform.eulerAngles.z;
                MoveRotation(angle);
            }
        }
    }

    /// <summary>
    /// 自动瞄准，使玩家面朝目标敌方单位。
    /// </summary>
    /// <returns>如果附近找到了适合自动瞄准的单位，则面朝此单位，并返回true，否则返回false。</returns>
    private bool AutoAim() {
        // 选择距离最近的敌人
        Vector2 origin = transform.position;
        int size = Physics2D.CircleCastNonAlloc(origin, 10, Vector2.zero, hits, float.MaxValue, enemyLayerMask);
        Vector2 nearestEnemyPosition = Vector2.zero;
        float minDistance = float.MaxValue;
        for (int i = 0; i < size; i++) {
            if (hits[i].transform.GetComponent<State>().health <= 0 || HasObstacleTo(hits[i].transform.position)) continue;

            Vector2 enemyPosition = hits[i].transform.position;
            float distance = Vector2.SqrMagnitude(enemyPosition - origin);
            if (distance < minDistance) {
                nearestEnemyPosition = enemyPosition;
                minDistance = distance;
            }
        }

        if (minDistance == float.MaxValue) return false; // 没有活着的敌人

        LookAt(nearestEnemyPosition);
        return true;
    }

    // 面朝鼠标
    private void LookAtMouse() {
        if (vehicleObject != null && !vehicleScript.Rotate(gameObject)) { // 在不允许自由旋转的载具内
            MoveRotation(float.MinValue); // 不旋转
            return;
        }

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        LookAt(mouseWorldPosition);
    }

    // 切换到下一把武器
    private void NextWeapon() {
        if (vehicleObject != null && !vehicleScript.NextWeapon(gameObject)) return; // 在不允许自由切换武器的载具内

        weaponManager.NextWeapon();
    }

    // 开火
    private void Fire() {
        if (vehicleObject != null && !vehicleScript.Fire(gameObject)) return; // 在不允许自由开火的载具内

        weaponManager.Fire();
    }
    
    // 限制物理运动只由操作的玩家控制
    protected override void FixedUpdate() {
        if (!isLocalPlayer) return;

        base.FixedUpdate();
    }

    // 进入或离开
    [Client]
    public void EnterOrLeave() {
        CmdEnterOrLeave();
    }
    [Command]
    private void CmdEnterOrLeave() {
        if (vehicle != null) // 在载具里面
            vehicle.GetComponent<Vehicle>().Leave(gameObject); // 离开载具
        else { // 不在载具内
            // 选择距离最近的载具
            Vector2 origin = transform.position;
            int size = Physics2D.CircleCastNonAlloc(origin, 1, Vector2.zero, hits, float.MaxValue, vehicleLayerMask);
            Transform nearestVehicleTransform = null;
            float minDistance = float.MaxValue;
            for (int i = 0; i < size; i++) {
                if (!hits[i].transform.CompareTag("Vehicle") || hits[i].transform.GetComponent<State>().health <= 0) continue;
                
                float distance = Vector2.SqrMagnitude((Vector2)hits[i].transform.position - origin);
                if (distance < minDistance) {
                    nearestVehicleTransform = hits[i].transform;
                    minDistance = distance;
                }
            }

            if (minDistance < float.MaxValue) // 发现最近载具
                nearestVehicleTransform.GetComponent<Vehicle>().Enter(gameObject); // 则进入此载具
        }
    }

    // 为新来的玩家正确忽视此玩家与载具的碰撞
    [Client]
    public override void OnStartClient() {
        if (vehicle != null) {
            Physics2D.IgnoreCollision(vehicle.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    // 载具改变时调用
    [Client]
    private void OnChangeVehicle(GameObject vehicle) {
        m_Vehicle = vehicle;
        if (isLocalPlayer) {
            if (isServer) StartCoroutine("OnHostChangeVehicle");
            else OnChangeVehicle();
        }
    }
    // 此方法是为了解决问题：服务器调用上个方法时m_Vehicle无论如何不会改变，在下一帧会自动改变。此问题在客户端上没有。
    [Server]
    private IEnumerator OnHostChangeVehicle() {
        yield return null;
        OnChangeVehicle();
    }

    // 载具改变时调用，显示载具武器或自己武器
    private void OnChangeVehicle() {
        if (vehicle == null) { // 载具没了
            weaponManager.ShowWeaponState(); // 显示自己武器
        } else { // 有载具了
            var vehicleScript = vehicle.GetComponent<Vehicle>();
            VehicleWeaponState vehicleWeaponState = vehicleScript.GetWeaponState(gameObject);
            if (vehicleWeaponState != null) { // 载具上有武器
                WeaponDisplay.singleton.ShowWeapon(vehicleWeaponState.sprite, vehicleWeaponState.color, vehicleWeaponState.numAmmo); // 显示载具上的武器
            }
        }
    }
}

/*
#if UNITY_STANDALONE// || UNITY_EDITOR // 对于单机设备以及Unity环境
#elif UNITY_IOS || UNITY_ANDROID // 对于安卓或IOS设备
#endif
*/
