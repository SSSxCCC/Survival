using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameMenuCanvas : MonoBehaviour {
    public GameObject joinGamePanel;
    public GameObject createGamePanel;

    void Start() {
        switch (MainPanel.lastClickMenu) {
            case GameMenu.Create:
                createGamePanel.SetActive(true);
                joinGamePanel.SetActive(false);
                break;
            case GameMenu.Join:
                createGamePanel.SetActive(false);
                joinGamePanel.SetActive(true);
                break;
        }
    }

    // 返回主菜单
    public void Return() {
        NetworkManager.Shutdown();
        Destroy(MyNetworkDiscovery.singleton.gameObject);
        SceneManager.LoadScene("Main Menu");
    }
}
