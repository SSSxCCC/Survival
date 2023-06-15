using UnityEngine;

/// <summary>
/// 联机模式游戏内选项菜单。
/// </summary>
public class IngameMenu : MonoBehaviour {
    // 打开或关闭游戏内选项菜单界面
    public void OpenOrClose() {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
