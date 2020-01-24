using UnityEngine;

namespace Offline
{
    public abstract class Controller : MonoBehaviour, IController
    {
        [SerializeField] float m_MoveForce = 1f; // 移动的推力
        public float moveForce { get { return m_MoveForce; } set { m_MoveForce = value; } }

        public int obstacleLayerMask { get { return commonController.GetObstacleLayerMask(); } } // 障碍物掩码

        private CommonController<Controller> commonController; // 公共实现

        protected virtual void Awake()
        {
            commonController = new CommonController<Controller>(this);
        }

        // 初始化
        protected virtual void Start()
        {
            commonController.Start();
        }

        // 物理移动和旋转
        private void FixedUpdate()
        {
            commonController.FixedUpdate();
        }

        // 面朝世界坐标point的位置
        public void LookAt(Vector2 point)
        {
            commonController.LookAt(point);
        }

        // 面朝矢量direction的方向
        public void RotateTo(Vector2 direction)
        {
            commonController.RotateTo(direction);
        }

        // 转向角度angle
        public void MoveRotation(float angle)
        {
            commonController.MoveRotation(angle);
        }

        // 推动前进
        public void Move(Vector2 forceDirection)
        {
            commonController.Move(forceDirection);
        }

        // 自己与目标点之间是否有障碍
        public bool HasObstacleTo(Vector2 point)
        {
            return commonController.HasObstacleTo(point);
        }
    }
}