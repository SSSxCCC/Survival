using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class WeaponManager : NetworkBehaviour, IWeaponManager
    {
        [SerializeField] GameObject m_initialWeaponPrefab; // 初始武器
        public GameObject initialWeaponPrefab { get { return m_initialWeaponPrefab; } set { m_initialWeaponPrefab = value; } }

        [SerializeField] Transform m_HoldPosition; // 握住武器的点
        public Transform holdPosition { get { return m_HoldPosition; } set { m_HoldPosition = value; } }

        [SyncVar] GameObject m_Pistol;
        [SyncVar] GameObject m_Shotgun;
        [SyncVar] GameObject m_MachineGun;
        [SyncVar] GameObject m_RocketLauncher;
        [SyncVar] GameObject m_LaserGun;
        public GameObject pistol { get { return m_Pistol; } set { m_Pistol = value; } }
        public GameObject shotgun { get { return m_Shotgun; } set { m_Shotgun = value; } }
        public GameObject machineGun { get { return m_MachineGun; } set { m_MachineGun = value; } }
        public GameObject rocketLauncher { get { return m_RocketLauncher; } set { m_RocketLauncher = value; } }
        public GameObject laserGun { get { return m_LaserGun; } set { m_LaserGun = value; } }
        
        [SyncVar(hook = "OnChangeWeapon")] int m_CurrentWeapon = -1; // 当前武器下标（初值为-1使其玩家获得初始武器后能马上正确显示剩余弹药数量）
        public int currentWeapon { get { return m_CurrentWeapon; } set { m_CurrentWeapon = value; } }

        private CommonWeaponManager<WeaponManager> commonWeaponManager; // 公共实现

        private void Awake()
        {
            commonWeaponManager = new CommonWeaponManager<WeaponManager>(this);
        }

        // 得到身上weaponType类型的武器
        public GameObject GetWeapon(WeaponType weaponType)
        {
            return commonWeaponManager.GetWeapon(weaponType);
        }

        // 捡起武器weapon
        [Server]
        public void PickUpWeapon(GameObject weapon)
        {
            weapon.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient); // 授权
            commonWeaponManager.PickUpWeapon(weapon);
        }

        // 得到初始武器
        [ServerCallback]
        private void Start()
        {
            GameObject weapon = commonWeaponManager.CreateInitWeapon();
            NetworkServer.Spawn(weapon);
            PickUpWeapon(weapon);
        }

        // 为新来的玩家正常显示已有玩家的当前武器
        [Client]
        public override void OnStartClient()
        {
            for (int i = 0; i < CommonWeaponManager<WeaponManager>.weapons.Length; i++)
            {
                GameObject weapon = GetWeapon(CommonWeaponManager<WeaponManager>.weapons[i]);
                if (weapon != null && i != currentWeapon)
                    weapon.GetComponent<Weapon>().SetVisible(false);
            }
        }

        // 开火
        [Client]
        public void Fire()
        {
            //if (!isLocalPlayer) return;

            commonWeaponManager.Fire();
        }

        // 换下一把武器
        [Client]
        public void NextWeapon()
        {
            CmdNextWeapon();
        }
        [Command]
        private void CmdNextWeapon()
        {
            commonWeaponManager.NextWeapon();
        }

        // 每次改变武器调用
        [Client]
        private void OnChangeWeapon(int currentWeapon)
        {
            m_CurrentWeapon = currentWeapon;
            commonWeaponManager.OnChangeWeapon();
        }

        // 删掉身上除了初始武器的所有武器
        [Server]
        public void RemoveWeapons()
        {
            List<GameObject> weaponsToDestroy = commonWeaponManager.RemoveWeapons();
            foreach (GameObject weapon in weaponsToDestroy) NetworkServer.Destroy(weapon);
        }

        // 更新显示当前武器剩余弹药数量
        [Client]
        public void ShowWeaponState()
        {
            if (!isLocalPlayer) return;

            commonWeaponManager.ShowWeaponState();
            // 对本地玩家显示当前武器的剩余弹药数量
            //int ammoRemain = GetWeapon(CommonWeaponManager<WeaponManager>.weapons[currentWeapon]).GetComponent<Weapon>().numAmmo;
            //GameManager.singleton.bulletText.text = LocalizationManager.instance.GetLocalizedValue("Ammo") + ": " + (ammoRemain == int.MaxValue ? "infinite" : ammoRemain.ToString());
        }
    }
}