using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class Aircraft : Vehicle, IAircraft
    {
        [SerializeField] Transform[] m_Seats; // 每个座位的位置（座位0是司机的）
        public Transform[] seats { get { return m_Seats; } set { m_Seats = value; } }

        [SyncVar] GameObject m_Passenger1;
        [SyncVar] GameObject m_Passenger2;
        public GameObject passenger1 { get { return m_Passenger1; } set { m_Passenger1 = value; } }
        public GameObject passenger2 { get { return m_Passenger2; } set { m_Passenger2 = value; } }

        [SerializeField] float m_RotateTorque = 1; // 旋转转矩
        public float rotateTorque { get { return m_RotateTorque; } set { m_RotateTorque = value; } }

        [SerializeField] float m_PowerForce = 1; // 前进动力
        public float powerForce { get { return m_PowerForce; } set { m_PowerForce = value; } }

        public GameObject explosionPrefab;

        private CommonAircraft<Aircraft> commonAircraft; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonAircraft = new CommonAircraft<Aircraft>(this);
        }

        private void Start()
        {
            commonAircraft.Start();
        }

        // 保持每个乘客在座位上
        private void Update()
        {
            commonAircraft.Update();
        }

        // 移动和转向
        private void FixedUpdate()
        {
            commonAircraft.FixedUpdate();
        }

        // 撞击单位
        [ServerCallback]
        private void OnCollisionEnter2D(Collision2D collision)
        {
            commonAircraft.OnCollisionEnter2D(collision);
        }

        // 被摧毁时爆炸
        [ServerCallback]
        private void OnDestroy()
        {
            NetworkServer.Spawn(commonAircraft.OnDestroy(explosionPrefab));
        }

        // 玩家进入
        [Server]
        public override void Enter(GameObject player)
        {
            base.Enter(player);
            if (commonAircraft.Enter(player) == 0) GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 授权
        }

        // 玩家离开
        [Server]
        public override void Leave(GameObject player)
        {
            base.Leave(player);
            if (commonAircraft.Leave(player) == 0) GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient); // 取消授权
        }

        // 所有玩家离开
        [Server]
        public override void LeaveAll()
        {
            commonAircraft.LeaveAll();
        }

        // 得到武器状态
        [Client]
        public override VehicleWeaponState GetWeaponState(GameObject player)
        {
            return new VehicleWeaponState(GetComponentInChildren<SpriteRenderer>().sprite, Color.white, int.MaxValue);
        }

        // 控制
        [Client]
        public override void Control(GameObject player, Vector2 axis)
        {
            commonAircraft.Control(player, axis);
        }

        [Client]
        public override bool Move(GameObject player)
        {
            return false;
        }

        [Client]
        public override bool Rotate(GameObject player)
        {
            player.transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z); // 面朝载具正前方
            return false;
        }

        [Client]
        public override bool NextWeapon(GameObject player)
        {
            return false;
        }

        [Client]
        public override bool Fire(GameObject player)
        {
            commonAircraft.Fire(player);
            return false;
        }
    }
}