using UnityEngine;

namespace Offline
{
    public abstract class Vehicle : MonoBehaviour, IVehicle
    {
        GameObject m_Driver; // 驾驶者
        public GameObject driver { get { return m_Driver; } set { m_Driver = value; } }

        private CommonVehicle<Vehicle> commonVehicle; // 公共实现

        protected virtual void Awake() { commonVehicle = new CommonVehicle<Vehicle>(this); }

        // 玩家进入载具
        public virtual void Enter(GameObject player) { commonVehicle.OnEnter(player); } // 无视此玩家与载具之间的碰撞

        // 玩家离开载具
        public virtual void Leave(GameObject player) { commonVehicle.OnLeave(player); } // 重新使玩家与载具间的碰撞有效

        // 离开所有进入的玩家
        public abstract void LeaveAll();

        // 得到载具上的武器信息
        public virtual VehicleWeaponState GetWeaponState(GameObject player) { return null; }

        // 操作载具
        public abstract void Control(GameObject player, Vector2 axis);

        public abstract bool Move(GameObject player);
        public abstract bool Rotate(GameObject player);
        public abstract bool NextWeapon(GameObject player);
        public abstract bool Fire(GameObject player);

        /// <summary>
        /// 静态方法刷载具
        /// </summary>
        /// <param name="vehiclePrefab">刷的载具</param>
        /// <param name="position">刷的位置</param>
        /// <param name="angle">初始朝向角度</param>
        public static void Create(GameObject vehiclePrefab, Vector2 position, float angle)
        {
            Instantiate(vehiclePrefab, position, Quaternion.Euler(0, 0, angle));
        }
    }
}