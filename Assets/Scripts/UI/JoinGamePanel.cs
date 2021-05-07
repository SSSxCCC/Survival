using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class JoinGamePanel : MonoBehaviour {
    public InputField hostInputField; // 主机名输入框
    public Dropdown genderDropdown; // 性别选择下拉条

    private void Start() {
        // 开始监听
        MyNetworkDiscovery.singleton.Initialize();
        MyNetworkDiscovery.singleton.StartAsClient();

        // 一开始将选中状态与静态变量保持一致
        genderDropdown.value = OnlineGameSetting.gender;
    }
    
    // 根据主机名输入框的内容，作为客户端加入游戏
    public void JoinGame() {
        NetworkManager.singleton.networkAddress = hostInputField.text; // 得到主机名
        NetworkManager.singleton.StartClient(); // 作为客户端加入游戏
    }

    // 性别改变时调用
    public void OnGenderDropdownChanged() {
        OnlineGameSetting.gender = genderDropdown.value;
    }
}
