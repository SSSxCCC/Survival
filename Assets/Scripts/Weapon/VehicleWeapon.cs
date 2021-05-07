using UnityEngine;
using UnityEngine.Networking;

public class VehicleWeapon : Shooter {
    private Rigidbody2D rb2D;

    private void Start() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // 开火
    [Client]
    public bool Fire(GameObject owner) {
        if (Time.time - lastFireTime < fireInterval)
            return false;

        CreateAmmo(owner);
        lastFireTime = Time.time;
        return true;
    }

    // 创建弹药
    [Client]
    public void CreateAmmo(GameObject owner) {
        CmdCreateAmmo(owner);
    }
    [Command]
    private void CmdCreateAmmo(GameObject owner) {
        var ammo = Instantiate(ammoPrefab, muzzle.position, transform.rotation);
        var ammoAttacker = ammo.GetComponent<AmmoAttacker>();
        ammoAttacker.attack = attack;
        ammoAttacker.owner = owner;
        if (ammoSpeed > 0) {
            float radianAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
            ammo.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)) * ammoSpeed + rb2D.velocity; // 完全惯性
        }
        NetworkServer.Spawn(ammo);
    }
}
