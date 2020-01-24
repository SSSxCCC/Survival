using UnityEngine;

namespace Offline
{
    public class Car : Vehicle, ICar
    {
        [SerializeField] Transform[] m_Seats; // 每个座位的位置（座位0是司机的）
        public Transform[] seats { get { return m_Seats; } set { m_Seats = value; } }

        GameObject m_Passenger1;
        GameObject m_Passenger2;
        GameObject m_Passenger3;
        public GameObject passenger1 { get { return m_Passenger1; } set { m_Passenger1 = value; } }
        public GameObject passenger2 { get { return m_Passenger2; } set { m_Passenger2 = value; } }
        public GameObject passenger3 { get { return m_Passenger3; } set { m_Passenger3 = value; } }

        [SerializeField] float m_RotateTorque = 1; // 旋转转矩
        public float rotateTorque { get { return m_RotateTorque; } set { m_RotateTorque = value; } }

        [SerializeField] float m_PowerForce = 1; // 汽车动力
        public float powerForce { get { return m_PowerForce; } set { m_PowerForce = value; } }

        public GameObject explosionPrefab;

        private CommonCar<Car> commonCar; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonCar = new CommonCar<Car>(this);
        }

        private void Start() { commonCar.Start(); }

        // 保持每个乘客在座位上
        private void Update() { commonCar.Update(); }

        // 移动和转向
        private void FixedUpdate() { commonCar.FixedUpdate(); }

        // 撞击单位
        private void OnCollisionEnter2D(Collision2D collision) { commonCar.OnCollisionEnter2D(collision); }

        // 被摧毁时爆炸
        private void OnDestroy() { commonCar.OnDestroy(explosionPrefab); }

        // 玩家进入
        public override void Enter(GameObject player)
        {
            base.Enter(player);
            commonCar.Enter(player);
        }

        // 玩家离开
        public override void Leave(GameObject player)
        {
            base.Leave(player);
            commonCar.Leave(player);
        }

        // 所有玩家离开
        public override void LeaveAll() { commonCar.LeaveAll(); }

        // 控制
        public override void Control(GameObject player, Vector2 axis) { commonCar.Control(player, axis); }

        public override bool Move(GameObject player) { return false; }
        
        public override bool Rotate(GameObject player) { return true; }
        
        public override bool NextWeapon(GameObject player) { return true; }
        
        public override bool Fire(GameObject player) { return true; }
    }
}