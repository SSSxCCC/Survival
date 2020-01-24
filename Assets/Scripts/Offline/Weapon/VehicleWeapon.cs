using UnityEngine;

namespace Offline
{
    public class VehicleWeapon : MonoBehaviour, IVehicleWeapon
    {
        [SerializeField] GameObject m_AmmoPrefab; // 发射的弹药类型
        public GameObject ammoPrefab { get { return m_AmmoPrefab; } set { m_AmmoPrefab = value; } }

        [SerializeField] Transform m_Muzzle; // 发射口
        public Transform muzzle { get { return m_Muzzle; } set { m_Muzzle = value; } }

        [SerializeField] int m_Attack = 1; // 攻击力
        public int attack { get { return m_Attack; } set { m_Attack = value; } }

        [SerializeField] float m_FireInterval; // 开火最短间隔时间
        public float fireInterval { get { return m_FireInterval; } set { m_FireInterval = value; } }

        [SerializeField] float m_AmmoSpeed = 1f; // 弹速
        public float ammoSpeed { get { return m_AmmoSpeed; } set { m_AmmoSpeed = value; } }

        float m_LastFireTime; // 上次开火时间
        public float lastFireTime { get { return m_LastFireTime; } set { m_LastFireTime = value; } }

        private CommonVehicleWeapon<VehicleWeapon> commonVehicleWeapon; // 公共实现

        private void Awake()
        {
            commonVehicleWeapon = new CommonVehicleWeapon<VehicleWeapon>(this);
        }

        private void Start() { commonVehicleWeapon.Start(); }

        // 开火
        public bool Fire(GameObject owner) { return commonVehicleWeapon.Fire(owner); }

        // 创建弹药
        public void CreateAmmo(GameObject owner) { commonVehicleWeapon.CreateAmmo(owner); }
    }
}