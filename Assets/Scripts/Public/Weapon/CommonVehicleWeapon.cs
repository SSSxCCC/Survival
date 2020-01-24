using UnityEngine;

public class CommonVehicleWeapon<T> where T : MonoBehaviour, IVehicleWeapon
{
    private T vehicleWeapon;

    private Rigidbody2D rb2D;

    public CommonVehicleWeapon(T vehicleWeapon)
    {
        this.vehicleWeapon = vehicleWeapon;
    }

    public void Start()
    {
        rb2D = vehicleWeapon.GetComponent<Rigidbody2D>();
    }

    public bool Fire(GameObject owner)
    {
        if (Time.time - vehicleWeapon.lastFireTime < vehicleWeapon.fireInterval)
            return false;

        vehicleWeapon.CreateAmmo(owner);
        vehicleWeapon.lastFireTime = Time.time;
        return true;
    }

    public GameObject CreateAmmo(GameObject owner)
    {
        GameObject ammo = Object.Instantiate(vehicleWeapon.ammoPrefab, vehicleWeapon.muzzle.position, vehicleWeapon.transform.rotation);
        IAmmoAttacker ammoAttacker = ammo.GetComponent<IAmmoAttacker>();
        ammoAttacker.attack = vehicleWeapon.attack;
        ammoAttacker.owner = owner;
        if (vehicleWeapon.ammoSpeed > 0)
        {
            float radianAngle = vehicleWeapon.transform.eulerAngles.z * Mathf.Deg2Rad;
            ammo.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)) * vehicleWeapon.ammoSpeed + rb2D.velocity; // 完全惯性
        }
        return ammo;
    }
}
