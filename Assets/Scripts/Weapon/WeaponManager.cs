using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {
    [SerializeField] GameObject m_initialWeaponPrefab; // 初始武器
    public GameObject initialWeaponPrefab { get { return m_initialWeaponPrefab; } set { m_initialWeaponPrefab = value; } }

    [SerializeField] Transform m_HoldPosition; // 握住武器的点
    public Transform holdPosition { get { return m_HoldPosition; } set { m_HoldPosition = value; } }

    [SyncVar] GameObject m_Pistol;
    [SyncVar] GameObject m_Shotgun;
    [SyncVar] GameObject m_MachineGun;
    [SyncVar] GameObject m_RocketLauncher;
    [SyncVar] GameObject m_LaserGun;
    public GameObject pistol { get { return m_Pistol; } set { m_Pistol = value; } }
    public GameObject shotgun { get { return m_Shotgun; } set { m_Shotgun = value; } }
    public GameObject machineGun { get { return m_MachineGun; } set { m_MachineGun = value; } }
    public GameObject rocketLauncher { get { return m_RocketLauncher; } set { m_RocketLauncher = value; } }
    public GameObject laserGun { get { return m_LaserGun; } set { m_LaserGun = value; } }
    
    [SyncVar(hook = "OnChangeWeapon")] int m_CurrentWeapon = -1; // 当前武器下标（初值为-1使其玩家获得初始武器后能马上正确显示剩余弹药数量）
    public int currentWeapon { get { return m_CurrentWeapon; } set { m_CurrentWeapon = value; } }

    public static WeaponType[] weapons = new WeaponType[] { WeaponType.Pistol, WeaponType.Shotgun, WeaponType.MachineGun, WeaponType.RocketLauncher, WeaponType.LaserGun }; // 定义武器顺序

    private int lastWeapon; // 上一把武器下标

    /// <summary>
    /// 根据武器类型，得到相应的玩家拥有的武器。
    /// </summary>
    /// <param name="weaponType">武器类型</param>
    /// <returns>如果玩家有此类型武器，则返回其引用，否则返回null。</returns>
    public GameObject GetWeapon(WeaponType weaponType) {
        switch (weaponType) {
            case WeaponType.Pistol:
                return pistol;
            case WeaponType.Shotgun:
                return shotgun;
            case WeaponType.MachineGun:
                return machineGun;
            case WeaponType.RocketLauncher:
                return rocketLauncher;
            case WeaponType.LaserGun:
                return laserGun;
            default:
                return null;
        }
    }

    // 捡起武器weapon
    [Server]
    public void PickUpWeapon(GameObject weapon) {
        weapon.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient); // 授权
        weapon.GetComponent<Weapon>().owner = gameObject; // 设置好武器拥有者
        WeaponType weaponType = weapon.GetComponent<Weapon>().weaponType; // 得到武器类型

        // 新武器放到合适的位置
        switch (weaponType) {
            case WeaponType.Pistol:
                pistol = weapon;
                break;
            case WeaponType.Shotgun:
                shotgun = weapon;
                break;
            case WeaponType.MachineGun:
                machineGun = weapon;
                break;
            case WeaponType.RocketLauncher:
                rocketLauncher = weapon;
                break;
            case WeaponType.LaserGun:
                laserGun = weapon;
                break;
        }

        currentWeapon = GetIndex(weaponType); // 切换到新武器
    }

    /// <summary>
    /// 根据武器类型，得到序号（下标）。
    /// </summary>
    /// <param name="weaponType">武器类型</param>
    /// <returns>武器类型对应的序号（下标）</returns>
    private int GetIndex(WeaponType weaponType) {
        for (int i = 0; i < weapons.Length; i++) {
            if (weaponType == weapons[i]) {
                return i;
            }
        }
        return -1;
    }

    // 得到初始武器
    [ServerCallback]
    private void Start() {
        var weapon = Instantiate(initialWeaponPrefab);
        weapon.GetComponent<Weapon>().numAmmo = 50;
        NetworkServer.Spawn(weapon);
        PickUpWeapon(weapon);
    }

    // 为新来的玩家正常显示已有玩家的当前武器
    [Client]
    public override void OnStartClient() {
        for (int i = 0; i < weapons.Length; i++) {
            GameObject weapon = GetWeapon(weapons[i]);
            if (weapon != null && i != currentWeapon)
                weapon.GetComponent<Weapon>().SetVisible(false);
        }
    }

    // 开火
    [Client]
    public void Fire() {
        //if (!isLocalPlayer) return;
        GetWeapon(weapons[currentWeapon]).GetComponent<Weapon>().Fire();
    }

    // 换下一把武器
    [Client]
    public void NextWeapon() {
        CmdNextWeapon();
    }
    [Command]
    private void CmdNextWeapon() {
        int nextWeapon = currentWeapon;
        do {
            if (++nextWeapon >= weapons.Length) nextWeapon = 0;
        } while (GetWeapon(weapons[nextWeapon]) == null);
        currentWeapon = nextWeapon;
    }

    // 每次改变武器调用
    [Client]
    private void OnChangeWeapon(int currentWeapon) {
        m_CurrentWeapon = currentWeapon;

        // 在确保上一把武器没有被摧毁的情况下隐藏上一把武器
        GameObject lastWeaponObject = GetWeapon(weapons[lastWeapon]);
        if (lastWeaponObject != null) lastWeaponObject.GetComponent<Weapon>().SetVisible(false);

        // 显示当前武器
        GetWeapon(weapons[currentWeapon]).GetComponent<Weapon>().SetVisible(true);
        ShowWeaponState();

        lastWeapon = currentWeapon;
    }

    // 删掉身上除了初始武器的所有武器
    [Server]
    public void RemoveWeapons() {
        WeaponType initWeaponType = initialWeaponPrefab.GetComponent<Weapon>().weaponType; // 得到初始武器类型
        currentWeapon = GetIndex(initWeaponType); // 切换到初始武器

        // 删掉所有其它武器
        for (int i = 0; i < weapons.Length; i++) {
            GameObject weapon = GetWeapon(weapons[i]);
            if (weapon != null && weapons[i] != initWeaponType)
                NetworkServer.Destroy(weapon);
        }
    }

    // 更新显示当前武器剩余弹药数量
    [Client]
    public void ShowWeaponState() {
        if (!isLocalPlayer) return;

        var weapon = GetWeapon(weapons[currentWeapon]).GetComponent<Weapon>();
        WeaponDisplay.singleton.ShowWeapon(weapon.weaponSprite, weapon.weaponRenderer.color, weapon.numAmmo);
        // 对本地玩家显示当前武器的剩余弹药数量
        //int ammoRemain = GetWeapon(CommonWeaponManager<WeaponManager>.weapons[currentWeapon]).GetComponent<Weapon>().numAmmo;
        //GameManager.singleton.bulletText.text = LocalizationManager.instance.GetLocalizedValue("Ammo") + ": " + (ammoRemain == int.MaxValue ? "infinite" : ammoRemain.ToString());
    }
}
