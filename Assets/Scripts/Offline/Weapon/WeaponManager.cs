using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class WeaponManager : MonoBehaviour, IWeaponManager
    {
        [SerializeField] GameObject m_InitialWeaponPrefab; // 初始武器
        public GameObject initialWeaponPrefab { get { return m_InitialWeaponPrefab; } set { m_InitialWeaponPrefab = value; } }

        [SerializeField] Transform m_HoldPosition; // 握住武器的点
        public Transform holdPosition { get { return m_HoldPosition; } set { m_HoldPosition = value; } }

        GameObject m_Pistol;
        GameObject m_Shotgun;
        GameObject m_MachineGun;
        GameObject m_RocketLauncher;
        GameObject m_LaserGun;
        public GameObject pistol { get { return m_Pistol; } set { m_Pistol = value; } }
        public GameObject shotgun { get { return m_Shotgun; } set { m_Shotgun = value; } }
        public GameObject machineGun { get { return m_MachineGun; } set { m_MachineGun = value; } }
        public GameObject rocketLauncher { get { return m_RocketLauncher; } set { m_RocketLauncher = value; } }
        public GameObject laserGun { get { return m_LaserGun; } set { m_LaserGun = value; } }

        int m_CurrentWeapon; // 当前武器下标
        public int currentWeapon { get { return m_CurrentWeapon; }
            set {
                m_CurrentWeapon = value;
                commonWeaponManager.OnChangeWeapon();
            }
        }

        private CommonWeaponManager<WeaponManager> commonWeaponManager; // 公共实现

        private void Awake() { commonWeaponManager = new CommonWeaponManager<WeaponManager>(this); }

        // 得到身上weaponType类型的武器
        public GameObject GetWeapon(WeaponType weaponType) { return commonWeaponManager.GetWeapon(weaponType); }

        // 捡起武器weapon
        public void PickUpWeapon(GameObject weapon) { commonWeaponManager.PickUpWeapon(weapon); }

        // 得到初始武器
        private void Start()
        {
            // 由于单机模式会读档，所以不需要初始武器
            //GameObject weapon = commonWeaponManager.CreateInitWeapon();
            //PickUpWeapon(weapon);
        }

        // 开火
        public void Fire() { commonWeaponManager.Fire(); }

        // 换下一把武器
        public void NextWeapon() { commonWeaponManager.NextWeapon(); }

        // 删掉身上除了初始武器的所有武器
        public void RemoveWeapons()
        {
            List<GameObject> weaponsToDestroy = commonWeaponManager.RemoveWeapons();
            foreach (GameObject weapon in weaponsToDestroy) Destroy(weapon);
        }

        // 更新显示当前武器剩余弹药数量
        public void ShowWeaponState()
        {
            commonWeaponManager.ShowWeaponState();
        }
    }
}