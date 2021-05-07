using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class MyNetworkMigrationManager : NetworkMigrationManager {
    // 当客户端与主机断开连接时
    protected override void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange) {
        bool gameCanContinue = false; // 游戏是否可以继续

        bool youAreNewHost;
        PeerInfoMessage newHostInfo;
        if (FindNewHost(out newHostInfo, out youAreNewHost)) { // 选一个新主机
            if (youAreNewHost) { // 自己被选为新主机
                gameCanContinue = BecomeNewHost(NetworkManager.singleton.networkPort); // 成为新主机
                /*if (gameCanContinue)
                { // 开始广播，以便其它客户端发现自己。（GameManager里面的OnStartServer会开始广播的）
                    if (MyNetworkDiscovery.singleton.running)
                        MyNetworkDiscovery.singleton.StopBroadcast();
                    MyNetworkDiscovery.singleton.Initialize();
                    MyNetworkDiscovery.singleton.StartAsServer();
                */
            } else { // 自己被选为客户端
                NetworkManager.singleton.networkAddress = newHostInfo.address;
                gameCanContinue = NetworkManager.singleton.client.ReconnectToNewHost(newHostInfo.address, NetworkManager.singleton.networkPort); // 连接新主机
            }
        }

        if (gameCanContinue) { // 因为有新的主机，游戏可以继续
            sceneChange = SceneChangeOption.StayInOnlineScene; // 停留在游戏界面
        } else { // 因为找不到新主机，或创建主机失败，或连不上新主机
            sceneChange = SceneChangeOption.SwitchToOfflineScene; // 回到主界面吧
        }
    }
}
