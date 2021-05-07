using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 联机模式游戏内菜单
/// </summary>
public class MenuPanel : MonoBehaviour {
    // 退出联机模式，回到联机模式菜单
    public void ExitOnlineGame() {
        NetworkManager.singleton.StopHost(); // 关掉客户端与服务端

        if (MyNetworkDiscovery.singleton.running)
            MyNetworkDiscovery.singleton.StopBroadcast(); // 停止广播

        NetworkTransport.Shutdown(); // 保证网络彻底关闭
    }
}
