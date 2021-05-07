using UnityEngine;
using UnityEngine.Networking;

public class LoadingDialog : MonoBehaviour {
    public GameObject JoiningDialog;
    public GameObject HostingDialog;
    
    public void Update() {
        if (NetworkClient.active && !NetworkServer.active)
            JoiningDialog.SetActive(true);
        else
            JoiningDialog.SetActive(false);

        if (NetworkServer.active)
            HostingDialog.SetActive(true);
        else
            HostingDialog.SetActive(false);
    }

    // 取消加入游戏
    public void CancelJoining() {
        NetworkManager.singleton.StopClient();
    }

    // 取消创建游戏
    public void CancelHosting() {
        NetworkManager.singleton.StopHost();
    }
}
