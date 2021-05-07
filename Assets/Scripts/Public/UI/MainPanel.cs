using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanel : MonoBehaviour {
    // 通过这个全局变量告诉GameMenu先进哪个Menu
    public static GameMenu lastClickMenu;

    public void StartGame() {
        lastClickMenu = GameMenu.Create;
        SceneManager.LoadScene("Game Menu");
    }

    public void JoinGame() {
        lastClickMenu = GameMenu.Join;
        SceneManager.LoadScene("Game Menu");
    }
	
}



public enum GameMenu { Create, Join }
