using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class PlayerController : Controller, IPlayerController
    {
        [SyncVar(hook = "OnChangeVehicle")] GameObject m_Vehicle; // 载具
        public GameObject vehicle { get { return m_Vehicle; } set { m_Vehicle = value; } }

        private CommonPlayerController<PlayerController> commonPlayerController; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonPlayerController = new CommonPlayerController<PlayerController>(this);
        }

        // 要镜头跟着玩家控制的单位
        [Client]
        public override void OnStartLocalPlayer()
        {
            Camera.main.GetComponent<CameraController>().localPlayer = gameObject;
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
            if (!isLocalPlayer) return;

            commonPlayerController.Update();
        }
        
        // 限制物理运动只由操作的玩家控制
        protected override void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            base.FixedUpdate();
        }

        // 进入或离开
        [Client]
        public void EnterOrLeave()
        {
            CmdEnterOrLeave();
        }
        [Command]
        private void CmdEnterOrLeave()
        {
            commonPlayerController.EnterOrLeave();
        }

        // 为新来的玩家正确忽视此玩家与载具的碰撞
        [Client]
        public override void OnStartClient()
        {
            if (vehicle != null)
            {
                Physics2D.IgnoreCollision(vehicle.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }

        // 载具改变时调用
        [Client]
        private void OnChangeVehicle(GameObject vehicle)
        {
            m_Vehicle = vehicle;
            if (isLocalPlayer)
            {
                if (isServer) StartCoroutine("OnHostChangeVehicle");
                else commonPlayerController.OnChangeVehicle();
            }
        }
        // 此方法是为了解决问题：服务器调用上个方法时m_Vehicle无论如何不会改变，在下一帧会自动改变。此问题在客户端上没有。
        [Server]
        private IEnumerator OnHostChangeVehicle()
        {
            yield return null;
            commonPlayerController.OnChangeVehicle();
        }
    }
}



/*
#if UNITY_STANDALONE// || UNITY_EDITOR // 对于单机设备以及Unity环境
#elif UNITY_IOS || UNITY_ANDROID // 对于安卓或IOS设备
#endif
*/