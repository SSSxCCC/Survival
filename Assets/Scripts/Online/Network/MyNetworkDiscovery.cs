using UnityEngine.Networking;

namespace Online
{
    public class MyNetworkDiscovery : NetworkDiscovery
    {
        public static MyNetworkDiscovery singleton; // 单例

        private void Start()
        {
            // 单例不能在Awake里面设置，因为当场景切换到Main Menu时，临时产生的本物体本组件的Awake会被调用。
            singleton = this;
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            // 将找到的主机ip显示于界面
            IpManager.singleton.AddIpButton(fromAddress);
        }
    }
}