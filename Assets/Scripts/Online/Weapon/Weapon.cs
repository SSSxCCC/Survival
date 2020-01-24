using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class Weapon : NetworkBehaviour, IWeapon
    {
        [SerializeField] WeaponType m_WeaponType; // 武器种类
        public WeaponType weaponType { get { return m_WeaponType; } set { m_WeaponType = value; } }

        [SerializeField] GameObject m_AmmoPrefab; // 发射的弹药类型
        public GameObject ammoPrefab { get { return m_AmmoPrefab; } set { m_AmmoPrefab = value; } }

        [SerializeField] Transform m_Muzzle; // 枪口
        public Transform muzzle { get { return m_Muzzle; } set { m_Muzzle = value; } }

        [SerializeField] SpriteRenderer m_WeaponRenderer; // 武器渲染器
        public SpriteRenderer weaponRenderer { get { return m_WeaponRenderer; } set { m_WeaponRenderer = value; } }

        [SerializeField] Sprite m_WeaponSprite; // 武器图片
        public Sprite weaponSprite { get { return m_WeaponSprite; } set { m_WeaponSprite = value; } }

        [SerializeField] int m_Attack = 1; // 攻击力
        public int attack { get { return m_Attack; } set { m_Attack = value; } }

        [SerializeField] float m_FireInterval; // 开火最短间隔时间
        public float fireInterval { get { return m_FireInterval; } set { m_FireInterval = value; } }

        [SerializeField] float m_AmmoSpeed = 1f; // 弹速
        public float ammoSpeed { get { return m_AmmoSpeed; } set { m_AmmoSpeed = value; } }

        [SyncVar(hook = "OnChangeNumAmmo")] int m_NumAmmo; // 剩余弹药数量
        public int numAmmo { get { return m_NumAmmo; } set { m_NumAmmo = value; } }

        [SyncVar] GameObject m_Owner; // 拥有者
        public GameObject owner { get { return m_Owner; } set { m_Owner = value; } }

        float m_LastFireTime; // 上次开火时间
        public float lastFireTime { get { return m_LastFireTime; } set { m_LastFireTime = value; } }

        private CommonWeapon<Weapon> commonWeapon; // 公共实现

        private void Awake()
        {
            commonWeapon = new CommonWeapon<Weapon>(this);
        }

        // 跟随主人
        private void Update()
        {
            commonWeapon.Update();
        }

        // 开火
        [Client]
        public void Fire()
        {
            commonWeapon.Fire();
        }

        // 生成弹药
        [Client]
        public void CreateAmmo(Vector2 muzzlePosition, Vector2 normalizedDirection)
        {
            CmdCreateAmmo(muzzlePosition, normalizedDirection);
        }
        [Command]
        private void CmdCreateAmmo(Vector2 muzzlePosition, Vector2 normalizedDirection)
        {
            GameObject ammo = commonWeapon.CreateAmmo(muzzlePosition, normalizedDirection);
            NetworkServer.Spawn(ammo);
        }

        // 当弹药数量改变时调用
        [Client]
        private void OnChangeNumAmmo(int numAmmo)
        {
            m_NumAmmo = numAmmo;
            commonWeapon.OnChangeNumAmmo();
        }

        // 显示或隐藏
        [Client]
        public void SetVisible(bool visible)
        {
            commonWeapon.SetVisible(visible);
        }
    }
}