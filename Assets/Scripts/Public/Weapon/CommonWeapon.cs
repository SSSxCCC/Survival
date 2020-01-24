using UnityEngine;

public class CommonWeapon<T> where T : MonoBehaviour, IWeapon
{
    private T weapon;

    public CommonWeapon(T weapon)
    {
        this.weapon = weapon;
    }

    public void Update()
    {
        if (weapon.owner == null) return;

        weapon.transform.eulerAngles = new Vector3(0, 0, weapon.owner.transform.eulerAngles.z);
        weapon.transform.position = weapon.owner.GetComponent<IWeaponManager>().holdPosition.position;
    }

    public void Fire()
    {
        if (Time.time - weapon.lastFireTime < weapon.fireInterval || weapon.numAmmo <= 0) return;

        weapon.CreateAmmo(weapon.muzzle.position, (weapon.muzzle.position - weapon.transform.position).normalized);
        weapon.lastFireTime = Time.time;
    }

    public GameObject CreateAmmo(Vector2 muzzlePosition, Vector2 normalizedDirection)
    {
        if (weapon.numAmmo != int.MaxValue) // 若此武器弹药不是无限的
        {
            weapon.numAmmo--; // 则减少一发弹药
        }

        //muzzlePosition = weapon.muzzle.position;
        //normalizedDirection = (weapon.muzzle.position - weapon.transform.position).normalized;

        // 创建弹药
        GameObject ammo = Object.Instantiate(weapon.ammoPrefab, muzzlePosition, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(normalizedDirection.y, normalizedDirection.x)));
        IAmmoAttacker ammoAttacker = ammo.GetComponent<IAmmoAttacker>();
        ammoAttacker.attack = weapon.attack;
        ammoAttacker.owner = weapon.owner;
        if (weapon.ammoSpeed > 0) ammo.GetComponent<Rigidbody2D>().velocity = normalizedDirection * weapon.ammoSpeed;

        return ammo;
    }

    public void OnChangeNumAmmo()
    {
        if (weapon.owner != null)
            weapon.owner.GetComponent<IWeaponManager>().ShowWeaponState();
    }

    public void SetVisible(bool visible)
    {
        weapon.weaponRenderer.enabled = visible;
    }
}



// 枚举出每种武器
public enum WeaponType { Pistol, Shotgun, MachineGun, RocketLauncher, LaserGun }