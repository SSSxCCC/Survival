using UnityEngine;
using UnityEngine.Networking;

public class Weapon : Shooter {
    [SerializeField] WeaponType m_WeaponType; // 武器种类
    public WeaponType weaponType { get { return m_WeaponType; } set { m_WeaponType = value; } }

    [SerializeField] SpriteRenderer m_WeaponRenderer; // 武器渲染器
    public SpriteRenderer weaponRenderer { get { return m_WeaponRenderer; } set { m_WeaponRenderer = value; } }

    [SerializeField] Sprite m_WeaponSprite; // 武器图片
    public Sprite weaponSprite { get { return m_WeaponSprite; } set { m_WeaponSprite = value; } }

    [SyncVar(hook = "OnChangeNumAmmo")] int m_NumAmmo; // 剩余弹药数量
    public int numAmmo { get { return m_NumAmmo; } set { m_NumAmmo = value; } }

    [SyncVar] GameObject m_Owner; // 拥有者
    public GameObject owner { get { return m_Owner; } set { m_Owner = value; } }

    // 跟随主人
    private void Update() {
        if (owner == null) return;

        transform.eulerAngles = new Vector3(0, 0, owner.transform.eulerAngles.z);
        transform.position = owner.GetComponent<WeaponManager>().holdPosition.position;
    }

    // 开火
    [Client]
    public void Fire() {
        if (Time.time - lastFireTime < fireInterval || numAmmo <= 0) return;

        CreateAmmo(muzzle.position, (muzzle.position - transform.position).normalized);
        lastFireTime = Time.time;
    }

    // 生成弹药
    [Client]
    public void CreateAmmo(Vector2 muzzlePosition, Vector2 normalizedDirection)
    {
        CmdCreateAmmo(muzzlePosition, normalizedDirection);
    }
    [Command]
    private void CmdCreateAmmo(Vector2 muzzlePosition, Vector2 normalizedDirection) {
        if (numAmmo != int.MaxValue) { // 若此武器弹药不是无限的
            numAmmo--; // 则减少一发弹药
        }

        //muzzlePosition = muzzle.position;
        //normalizedDirection = (muzzle.position - transform.position).normalized;

        // 创建弹药
        var ammo = Instantiate(ammoPrefab, muzzlePosition, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(normalizedDirection.y, normalizedDirection.x)));
        var ammoAttacker = ammo.GetComponent<AmmoAttacker>();
        ammoAttacker.attack = attack;
        ammoAttacker.owner = owner;
        if (ammoSpeed > 0) ammo.GetComponent<Rigidbody2D>().velocity = normalizedDirection * ammoSpeed;
        NetworkServer.Spawn(ammo);
    }

    // 当弹药数量改变时调用
    [Client]
    private void OnChangeNumAmmo(int numAmmo) {
        m_NumAmmo = numAmmo;
        if (owner != null)
            owner.GetComponent<WeaponManager>().ShowWeaponState();
    }

    // 显示或隐藏
    [Client]
    public void SetVisible(bool visible) {
        weaponRenderer.enabled = visible;
    }
}



// 枚举出每种武器
public enum WeaponType { Pistol, Shotgun, MachineGun, RocketLauncher, LaserGun }
