using UnityEngine;

/// <summary>
/// 武器管理器接口。
/// </summary>
public interface IWeaponManager
{
    GameObject initialWeaponPrefab { get; set; }
    Transform holdPosition { get; set; }
    GameObject pistol { get; set; }
    GameObject shotgun { get; set; }
    GameObject machineGun { get; set; }
    GameObject rocketLauncher { get; set; }
    GameObject laserGun { get; set; }
    int currentWeapon { get; set; }

    GameObject GetWeapon(WeaponType weaponType);
    void PickUpWeapon(GameObject weapon);
    void Fire();
    void NextWeapon();
    void RemoveWeapons();
    void ShowWeaponState();
}
