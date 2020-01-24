using UnityEngine;

public class CommonDroppedWeapon<T> where T : MonoBehaviour, IDroppedWeapon
{
    private T droppedWeapon;

    public CommonDroppedWeapon(T droppedWeapon)
    {
        this.droppedWeapon = droppedWeapon;
    }

    public bool FindOwner(Collider2D collider)
    {
        // 判断对方是否能捡武器
        IWeaponManager weaponManager = collider.GetComponent<IWeaponManager>();
        IPlayerController playerController = collider.GetComponent<IPlayerController>();
        if (weaponManager == null || (playerController != null && playerController.vehicle != null))
            return false;

        // 判断对方是否已有此武器
        GameObject weapon = weaponManager.GetWeapon(droppedWeapon.weaponPrefab.GetComponent<IWeapon>().weaponType);
        if (weapon == null) // 如果对方还没有此武器，创造一个此掉落武器对应的武器给予对方
        {
            GameObject newWeapon = droppedWeapon.CreateWeapon(weaponManager.holdPosition.position, collider.transform.eulerAngles.z);
            weaponManager.PickUpWeapon(newWeapon);
        }
        else // 如果对方有此武器了，增加相应武器的弹药数量
        {
            if (weapon.GetComponent<IWeapon>().numAmmo != int.MaxValue) // 如果弹药不是无限的
                weapon.GetComponent<IWeapon>().numAmmo += droppedWeapon.numAmmo; // 则增加弹药数量
        }

        return true;
    }

    /// <summary>
    /// 创建武器。
    /// </summary>
    /// <param name="position">武器握点位置</param>
    /// <param name="angle">武器朝向</param>
    /// <returns>创建的武器</returns>
    public GameObject CreateWeapon(Vector3 position, float angle)
    {
        GameObject newWeapon = Object.Instantiate(droppedWeapon.weaponPrefab, position, Quaternion.Euler(0, 0, angle));
        newWeapon.GetComponent<IWeapon>().numAmmo = droppedWeapon.numAmmo;
        return newWeapon;
    }
}
