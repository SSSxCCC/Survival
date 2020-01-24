using System.Collections.Generic;
using UnityEngine;

public class CommonWeaponManager<T> where T : MonoBehaviour, IWeaponManager
{
    public static WeaponType[] weapons = new WeaponType[] { WeaponType.Pistol, WeaponType.Shotgun, WeaponType.MachineGun, WeaponType.RocketLauncher, WeaponType.LaserGun }; // 定义武器顺序

    private T weaponManager;

    private int lastWeapon; // 上一把武器下标

    public CommonWeaponManager(T weaponManager)
    {
        this.weaponManager = weaponManager;
    }

    /// <summary>
    /// 根据武器类型，得到相应的玩家拥有的武器。
    /// </summary>
    /// <param name="weaponType">武器类型</param>
    /// <returns>如果玩家有此类型武器，则返回其引用，否则返回null。</returns>
    public GameObject GetWeapon(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                return weaponManager.pistol;
            case WeaponType.Shotgun:
                return weaponManager.shotgun;
            case WeaponType.MachineGun:
                return weaponManager.machineGun;
            case WeaponType.RocketLauncher:
                return weaponManager.rocketLauncher;
            case WeaponType.LaserGun:
                return weaponManager.laserGun;
            default:
                return null;
        }
    }

    public void PickUpWeapon(GameObject weapon)
    {
        weapon.GetComponent<IWeapon>().owner = weaponManager.gameObject; // 设置好武器拥有者
        WeaponType weaponType = weapon.GetComponent<IWeapon>().weaponType; // 得到武器类型

        // 新武器放到合适的位置
        switch (weaponType)
        {
            case WeaponType.Pistol:
                weaponManager.pistol = weapon;
                break;
            case WeaponType.Shotgun:
                weaponManager.shotgun = weapon;
                break;
            case WeaponType.MachineGun:
                weaponManager.machineGun = weapon;
                break;
            case WeaponType.RocketLauncher:
                weaponManager.rocketLauncher = weapon;
                break;
            case WeaponType.LaserGun:
                weaponManager.laserGun = weapon;
                break;
        }

        weaponManager.currentWeapon = GetIndex(weaponType); // 切换到新武器
    }
    
    /// <summary>
    /// 根据武器类型，得到序号（下标）。
    /// </summary>
    /// <param name="weaponType">武器类型</param>
    /// <returns>武器类型对应的序号（下标）</returns>
    private int GetIndex(WeaponType weaponType)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weaponType == weapons[i])
            {
                return i;
            }
        }
        return -1;
    }

    public GameObject CreateInitWeapon()
    {
        GameObject weapon = Object.Instantiate(weaponManager.initialWeaponPrefab);
        weapon.GetComponent<IWeapon>().numAmmo = 50;
        return weapon;
    }

    public void Fire()
    {
        GetWeapon(weapons[weaponManager.currentWeapon]).GetComponent<IWeapon>().Fire();
    }

    public void NextWeapon()
    {
        int nextWeapon = weaponManager.currentWeapon;
        do
        {
            if (++nextWeapon >= weapons.Length) nextWeapon = 0;
        } while (GetWeapon(weapons[nextWeapon]) == null);
        weaponManager.currentWeapon = nextWeapon;
    }

    public void OnChangeWeapon()
    {
        // 在确保上一把武器没有被摧毁的情况下隐藏上一把武器
        GameObject lastWeaponObject = GetWeapon(weapons[lastWeapon]);
        if (lastWeaponObject != null) lastWeaponObject.GetComponent<IWeapon>().SetVisible(false);

        // 显示当前武器
        GetWeapon(weapons[weaponManager.currentWeapon]).GetComponent<IWeapon>().SetVisible(true);
        weaponManager.ShowWeaponState();

        lastWeapon = weaponManager.currentWeapon;
    }

    /// <summary>
    /// 得到除了初始武器以外的所有武器，以便于对它们进行销毁。
    /// </summary>
    /// <returns>除了初始武器以外的所有武器列表</returns>
    public List<GameObject> RemoveWeapons()
    {
        List<GameObject> weaponsToDestroy = new List<GameObject>();

        WeaponType initWeaponType = weaponManager.initialWeaponPrefab.GetComponent<IWeapon>().weaponType; // 得到初始武器类型
        weaponManager.currentWeapon = GetIndex(initWeaponType); // 切换到初始武器

        // 删掉所有其它武器
        for (int i = 0; i < weapons.Length; i++)
        {
            GameObject weapon = GetWeapon(weapons[i]);
            if (weapon != null && weapons[i] != initWeaponType)
                weaponsToDestroy.Add(weapon);
        }

        return weaponsToDestroy;
    }

    public void ShowWeaponState()
    {
        IWeapon weapon = GetWeapon(weapons[weaponManager.currentWeapon]).GetComponent<IWeapon>();
        WeaponDisplay.singleton.ShowWeapon(weapon.weaponSprite, weapon.weaponRenderer.color, weapon.numAmmo);
    }
}