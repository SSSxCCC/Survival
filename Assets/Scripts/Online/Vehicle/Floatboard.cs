using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class Floatboard : Vehicle, IFloatboard
    {
        [SerializeField] Transform[] m_Seats; // 每个座位的位置（先来的玩家是司机）
        public Transform[] seats { get { return m_Seats; } set { m_Seats = value; } }

        [SyncVar] int m_DriverSeatIndex = -1; // 司机座位下标
        public int driverSeatIndex { get { return m_DriverSeatIndex; } set { m_DriverSeatIndex = value; } }

        [SyncVar] GameObject m_Passenger; // 乘客
        public GameObject passenger { get { return m_Passenger; } set { m_Passenger = value; } }

        [SerializeField] float m_RotateTorque = 1; // 旋转转矩
        public float rotateTorque { get { return m_RotateTorque; } set { m_RotateTorque = value; } }

        [SerializeField] float m_PowerForce = 1; // 汽车动力
        public float powerForce { get { return m_PowerForce; } set { m_PowerForce = value; } }

        public GameObject explosionPrefab;

        private CommonFloatboard<Floatboard> commonFloatboard; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonFloatboard = new CommonFloatboard<Floatboard>(this);
        }

        private void Start()
        {
            commonFloatboard.Start();
        }

        // 保持每个乘客在座位上
        private void Update()
        {
            commonFloatboard.Update();
        }

        // 移动和转向
        private void FixedUpdate()
        {
            commonFloatboard.FixedUpdate();
        }

        // 撞击单位
        [ServerCallback]
        private void OnCollisionEnter2D(Collision2D collision)
        {
            commonFloatboard.OnCollisionEnter2D(collision);
        }

        // 被摧毁时爆炸
        [ServerCallback]
        private void OnDestroy()
        {
            NetworkServer.Spawn(commonFloatboard.OnDestroy(explosionPrefab));
        }

        // 玩家进入
        [Server]
        public override void Enter(GameObject player)
        {
            base.Enter(player);
            if (commonFloatboard.Enter(player)) GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 授权
        }

        // 玩家离开
        [Server]
        public override void Leave(GameObject player)
        {
            base.Leave(player);
            if (commonFloatboard.Leave(player)) // 离开的是司机
            {
                GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 取消授权
                if (driver != null) // 有新的玩家成为司机了
                    GetComponent<NetworkIdentity>().AssignClientAuthority(driver.GetComponent<NetworkIdentity>().connectionToClient); // 给新的司机授权
            }
        }

        // 所有玩家离开
        [Server]
        public override void LeaveAll()
        {
            commonFloatboard.LeaveAll();
        }

        // 控制
        [Client]
        public override void Control(GameObject player, Vector2 axis)
        {
            commonFloatboard.Control(player, axis);
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
            return true;
        }

        [Client]
        public override bool Fire(GameObject player)
        {
            return true;
        }
    }
}