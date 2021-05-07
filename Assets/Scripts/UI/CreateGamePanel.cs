using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateGamePanel : MonoBehaviour {
    public Toggle hurtEachOtherToggle; // 互相伤害开关

    // 一开始将选中状态与静态变量保持一致
    private void Start() {
        hurtEachOtherToggle.isOn = OnlineGameSetting.hurtEachOther;
    }

    // 以自己为主机开始游戏
    public void StartGameAsHost() {
        NetworkManager.singleton.StartHost();
    }

    // 互相伤害被选中状态改变时调用
    public void OnHurtEachOtherToggleChanged() {
        OnlineGameSetting.hurtEachOther = hurtEachOtherToggle.isOn;
    }
}
