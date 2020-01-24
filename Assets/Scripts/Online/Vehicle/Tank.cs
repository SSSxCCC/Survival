using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class Tank : Vehicle, ITank
    {
        [SerializeField] Transform m_Seat; // 座位的位置
        public Transform seat { get { return m_Seat; } set { m_Seat = value; } }

        [SerializeField] Transform m_Cannon; // 炮的位置
        public Transform cannon { get { return m_Cannon; } set { m_Cannon = value; } }

        [SerializeField] float m_RotateTorque = 1; // 旋转转矩
        public float rotateTorque { get { return m_RotateTorque; } set { m_RotateTorque = value; } }

        [SerializeField] float m_PowerForce = 1; // 动力
        public float powerForce { get { return m_PowerForce; } set { m_PowerForce = value; } }

        [SerializeField] Transform m_Muzzle; // 炮口
        public Transform muzzle { get { return m_Muzzle; } set { m_Muzzle = value; } }

        [SerializeField] GameObject m_AmmoPrefab; // 弹药
        public GameObject ammoPrefab { get { return m_AmmoPrefab; } set { m_AmmoPrefab = value; } }

        [SerializeField] int m_Attack; // 攻击力
        public int attack { get { return m_Attack; } set { m_Attack = value; } }

        [SerializeField] float m_FireInterval; // 开火间隔
        public float fireInterval { get { return m_FireInterval; } set { m_FireInterval = value; } }

        [SerializeField] float m_AmmoSpeed; // 弹药射速
        public float ammoSpeed { get { return m_AmmoSpeed; } set { m_AmmoSpeed = value; } }

        public float lastFireTime { get; set; } // 上次开火时间

        public GameObject explosionPrefab;

        private CommonTank<Tank> commonTank; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonTank = new CommonTank<Tank>(this);
        }

        private void Start()
        {
            commonTank.Start();
        }

        // 保持玩家在座位上
        private void Update()
        {
            commonTank.Update();
        }

        // 移动和转向
        private void FixedUpdate()
        {
            commonTank.FixedUpdate();
        }

        // 撞击单位
        [ServerCallback]
        private void OnCollisionEnter2D(Collision2D collision)
        {
            commonTank.OnCollisionEnter2D(collision);
        }

        // 被摧毁时爆炸
        [ServerCallback]
        private void OnDestroy()
        {
            NetworkServer.Spawn(commonTank.OnDestroy(explosionPrefab));
        }

        // 玩家进入
        [Server]
        public override void Enter(GameObject player)
        {
            base.Enter(player);
            commonTank.Enter(player);
            GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 授权
        }

        // 玩家离开
        [Server]
        public override void Leave(GameObject player)
        {
            base.Leave(player);
            commonTank.Leave(player);
            GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 取消授权
        }

        // 所有玩家离开
        [Server]
        public override void LeaveAll()
        {
            commonTank.LeaveAll();
        }

        // 得到武器状态
        [Client]
        public override VehicleWeaponState GetWeaponState(GameObject player)
        {
            return new VehicleWeaponState(cannon.GetComponentInChildren<SpriteRenderer>().sprite, Color.white, int.MaxValue);
        }

        // 控制
        [Client]
        public override void Control(GameObject player, Vector2 axis)
        {
            commonTank.Control(player, axis);
        }

        [Client]
        public override bool Move(GameObject player)
        {
            return false;
        }

        [Client]
        public override bool Rotate(GameObject player)
        {
            return true;
        }

        [Client]
        public override bool NextWeapon(GameObject player)
        {
            return false;
        }

        [Client]
        public override bool Fire(GameObject player)
        {
            CmdFire();
            return false;
        }
        [Command]
        private void CmdFire()
        {
            GameObject ammo = commonTank.Fire();
            if (ammo != null) NetworkServer.Spawn(ammo);
        }
    }
}