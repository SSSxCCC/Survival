using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanel : MonoBehaviour {

    public void OfflineMenu()
    {
        SceneManager.LoadScene("Offline Menu");
    }

    public void OnlineMenu()
    {
        SceneManager.LoadScene("Online Menu");
    }
	
}
