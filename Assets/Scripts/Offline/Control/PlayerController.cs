using UnityEngine;

namespace Offline
{
    public class PlayerController : Controller, IPlayerController
    {
        GameObject m_Vehicle; // 载具
        public GameObject vehicle { get { return m_Vehicle; }
            set {
                m_Vehicle = value;
                commonPlayerController.OnChangeVehicle();
            }
        }

        private CommonPlayerController<PlayerController> commonPlayerController; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonPlayerController = new CommonPlayerController<PlayerController>(this);

            Camera.main.GetComponent<CameraController>().localPlayer = gameObject; // 要镜头跟着玩家控制的单位
        }

        // 变量初始化
        protected override void Start()
        {
            base.Start();
            commonPlayerController.Start();
        }

        // 玩家操作处理
        private void Update()
        {
            commonPlayerController.Update();
        }

        // 进入或离开
        public void EnterOrLeave()
        {
            commonPlayerController.EnterOrLeave();
        }
    }
}