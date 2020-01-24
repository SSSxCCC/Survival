using UnityEngine;

namespace Offline
{
    public class Aircraft : Vehicle, IAircraft
    {
        [SerializeField] Transform[] m_Seats; // 每个座位的位置（座位0是司机的）
        public Transform[] seats { get { return m_Seats; } set { m_Seats = value; } }

        public GameObject passenger1 { get; set; }
        public GameObject passenger2 { get; set; }

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

        private void Start() { commonAircraft.Start(); }

        // 保持每个乘客在座位上
        private void Update() { commonAircraft.Update(); }

        // 移动和转向
        private void FixedUpdate() { commonAircraft.FixedUpdate(); }

        // 撞击单位
        private void OnCollisionEnter2D(Collision2D collision) { commonAircraft.OnCollisionEnter2D(collision); }

        // 被摧毁时爆炸
        private void OnDestroy() { commonAircraft.OnDestroy(explosionPrefab); }

        // 玩家进入
        public override void Enter(GameObject player)
        {
            base.Enter(player);
            commonAircraft.Enter(player);
        }

        // 玩家离开
        public override void Leave(GameObject player)
        {
            base.Leave(player);
            commonAircraft.Leave(player);
        }

        // 所有玩家离开
        public override void LeaveAll() { commonAircraft.LeaveAll(); }

        // 得到武器状态
        public override VehicleWeaponState GetWeaponState(GameObject player) { return new VehicleWeaponState(GetComponentInChildren<SpriteRenderer>().sprite, Color.white, int.MaxValue); }

        // 控制
        public override void Control(GameObject player, Vector2 axis) { commonAircraft.Control(player, axis); }
        
        public override bool Move(GameObject player) { return false; }
        
        public override bool Rotate(GameObject player)
        {
            player.transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z); // 面朝载具正前方
            return false;
        }

        public override bool NextWeapon(GameObject player) { return false; }

        public override bool Fire(GameObject player)
        {
            commonAircraft.Fire(player);
            return false;
        }
    }
}