using UnityEngine;

/// <summary>
/// 触屏输入管理脚本。
/// 在这里可以找到每个触屏按钮的引用，方便判断对应按钮是否被按下。
/// </summary>
public class VirtualInputManager : MonoBehaviour {
    public static VirtualInputManager singleton; // 单例

    public VirtualButton fireButton; // 开火按钮
    public VirtualButton nextWeaponButton; // 换下把武器按钮
    public VirtualButton enterOrLeaveButton; // 进入按钮

    public VirtualJoystick joystick; // 虚拟摇杆

    private void Awake() {
        singleton = this; // 成为单例
    }
}
