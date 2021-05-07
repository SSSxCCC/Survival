using UnityEngine;
using UnityEngine.Networking;

public class DroppedWeapon : NetworkBehaviour
{
    [SerializeField] GameObject m_WeaponPrefab; // 此掉落武器代表的武器
    public GameObject weaponPrefab { get { return m_WeaponPrefab; } set { m_WeaponPrefab = value; } }

    [SerializeField] int m_NumAmmo = 1; // 掉落武器包含的弹药数量
    public int numAmmo { get { return m_NumAmmo; } set { m_NumAmmo = value; } }

    // 碰到东西了
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // 判断对方是否能捡武器
        var weaponManager = collider.GetComponent<WeaponManager>();
        var playerController = collider.GetComponent<PlayerController>();
        if (weaponManager == null || (playerController != null && playerController.vehicle != null))
            return;
        
        // 判断对方是否已有此武器
        GameObject weapon = weaponManager.GetWeapon(weaponPrefab.GetComponent<Weapon>().weaponType);
        if (weapon == null) { // 如果对方还没有此武器，创造一个此掉落武器对应的武器给予对方
            GameObject newWeapon = CreateWeapon(weaponManager.holdPosition.position, collider.transform.eulerAngles.z);
            weaponManager.PickUpWeapon(newWeapon);
        } else { // 如果对方有此武器了，增加相应武器的弹药数量
            if (weapon.GetComponent<Weapon>().numAmmo != int.MaxValue) // 如果弹药不是无限的
                weapon.GetComponent<Weapon>().numAmmo += numAmmo; // 则增加弹药数量
        }

        NetworkServer.Destroy(gameObject); // 找到主人则摧毁自己
    }

    // 创建此掉落武器代表的武器实例
    [Server]
    public GameObject CreateWeapon(Vector3 position, float angle)
    {
        GameObject newWeapon = Instantiate(weaponPrefab, position, Quaternion.Euler(0, 0, angle));
        newWeapon.GetComponent<Weapon>().numAmmo = numAmmo;
        NetworkServer.Spawn(newWeapon);
        return newWeapon;
    }
}
