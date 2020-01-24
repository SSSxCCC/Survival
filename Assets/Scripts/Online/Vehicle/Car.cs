using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class Car : Vehicle, ICar
    {
        [SerializeField] Transform[] m_Seats; // 每个座位的位置（座位0是司机的）
        public Transform[] seats { get { return m_Seats; } set { m_Seats = value; } }

        [SyncVar] GameObject m_Passenger1;
        [SyncVar] GameObject m_Passenger2;
        [SyncVar] GameObject m_Passenger3;
        public GameObject passenger1 { get { return m_Passenger1; } set { m_Passenger1 = value; } }
        public GameObject passenger2 { get { return m_Passenger2; } set { m_Passenger2 = value; } }
        public GameObject passenger3 { get { return m_Passenger3; } set { m_Passenger3 = value; } }

        [SerializeField] float m_RotateTorque = 1; // 旋转转矩
        public float rotateTorque { get { return m_RotateTorque; } set { m_RotateTorque = value; } }

        [SerializeField] float m_PowerForce = 1; // 前进动力
        public float powerForce { get { return m_PowerForce; } set { m_PowerForce = value; } }

        public GameObject explosionPrefab;

        private CommonCar<Car> commonCar; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonCar = new CommonCar<Car>(this);
        }

        private void Start()
        {
            commonCar.Start();
        }

        // 保持每个乘客在座位上
        private void Update()
        {
            commonCar.Update();
        }

        // 移动和转向
        private void FixedUpdate()
        {
            commonCar.FixedUpdate();
        }

        // 撞击单位
        [ServerCallback]
        private void OnCollisionEnter2D(Collision2D collision)
        {
            commonCar.OnCollisionEnter2D(collision);
        }

        // 被摧毁时爆炸
        [ServerCallback]
        private void OnDestroy()
        {
            NetworkServer.Spawn(commonCar.OnDestroy(explosionPrefab));
        }

        // 玩家进入
        [Server]
        public override void Enter(GameObject player)
        {
            base.Enter(player);
            if (commonCar.Enter(player) == 0) GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 授权
        }

        // 玩家离开
        [Server]
        public override void Leave(GameObject player)
        {
            base.Leave(player);
            if (commonCar.Leave(player) == 0) GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 取消授权
        }

        // 所有玩家离开
        [Server]
        public override void LeaveAll()
        {
            commonCar.LeaveAll();
        }

        // 控制
        [Client]
        public override void Control(GameObject player, Vector2 axis)
        {
            commonCar.Control(player, axis);
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