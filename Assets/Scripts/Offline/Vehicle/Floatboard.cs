using UnityEngine;

namespace Offline
{
    public class Floatboard : Vehicle, IFloatboard
    {
        [SerializeField] Transform[] m_Seats; // 每个座位的位置（先来的玩家是司机）
        public Transform[] seats { get { return m_Seats; } set { m_Seats = value; } }

        int m_DriverSeatIndex = -1; // 司机座位下标
        public int driverSeatIndex { get { return m_DriverSeatIndex; } set { m_DriverSeatIndex = value; } }

        GameObject m_Passenger; // 乘客
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

        private void Start() { commonFloatboard.Start(); }

        // 保持每个乘客在座位上
        private void Update() { commonFloatboard.Update(); }

        // 移动和转向
        private void FixedUpdate() { commonFloatboard.FixedUpdate(); }

        // 撞击单位
        private void OnCollisionEnter2D(Collision2D collision) { commonFloatboard.OnCollisionEnter2D(collision); }

        // 被摧毁时爆炸
        private void OnDestroy() { commonFloatboard.OnDestroy(explosionPrefab); }

        // 玩家进入
        public override void Enter(GameObject player)
        {
            base.Enter(player);
            commonFloatboard.Enter(player);
        }

        // 玩家离开
        public override void Leave(GameObject player)
        {
            base.Leave(player);
            commonFloatboard.Leave(player);
        }

        // 所有玩家离开
        public override void LeaveAll() { commonFloatboard.LeaveAll(); }

        // 控制
        public override void Control(GameObject player, Vector2 axis) { commonFloatboard.Control(player, axis); }
        
        public override bool Move(GameObject player) { return false; }
        
        public override bool Rotate(GameObject player) { return true; }
        
        public override bool NextWeapon(GameObject player) { return true; }
        
        public override bool Fire(GameObject player) { return true; }
    }
}